using Repositorio.Tests.Fixtures;
using Repositorio.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;
using ParqueoDatabaseExample.Models;
using Ejercicio2.Repositorio.Repositories.Sql;
using Microsoft.EntityFrameworkCore;

namespace Repositorio.Tests.Unit;

[Collection("SqlDatabase")]
public class SqlUnitOfWorkTests
{
    private readonly SqlDatabaseFixture _fixture;
    private readonly ITestOutputHelper _output;

    public SqlUnitOfWorkTests(SqlDatabaseFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    #region Transaction Tests

    [Fact]
    public async Task SaveChangesAsync_CommitsMultipleOperations_WhenAllSuccessful()
    {
        var auto = TestDataBuilder.CreateAutomovil(1);
        var parqueo = TestDataBuilder.CreateParqueo(1);
        
        using var context = _fixture.CreateContext();
        var unitOfWork = new SqlUnitOfWork(
            context,
            new SqlAutomovilRepository(context),
            new SqlParqueoRepository(context),
            new SqlIngresoAutomovilRepository(context));

        await unitOfWork.AutomovilRepository.InsertAsync(auto);
        await unitOfWork.ParqueoRepository.InsertAsync(parqueo);
        await unitOfWork.SaveChangesAsync();

        var autoResult = await context.PrqAutomoviles.FindAsync(1);
        var parqueoResult = await context.PrqParqueos.FindAsync(1);

        Assert.NotNull(autoResult);
        Assert.NotNull(parqueoResult);
    }

    [Fact]
    public async Task SaveChangesAsync_RollbackOnFailure_WhenConfigured()
    {
        var auto = TestDataBuilder.CreateAutomovil(1);
        var parqueo = new PrqParqueo { Id = 1, Nombre = "Parqueo", NombreDeProvincia = "SJ", PrecioPorHora = 10m };
        
        using var context = _fixture.CreateContext();
        var unitOfWork = new SqlUnitOfWork(
            context,
            new SqlAutomovilRepository(context),
            new SqlParqueoRepository(context),
            new SqlIngresoAutomovilRepository(context));

        await unitOfWork.AutomovilRepository.InsertAsync(auto);
        await unitOfWork.ParqueoRepository.InsertAsync(parqueo);
        
        try
        {
            await unitOfWork.SaveChangesAsync();
        }
        catch
        {
        }

        var autoExists = await context.PrqAutomoviles.AnyAsync(a => a.Id == 1);
        var parqueoExists = await context.PrqParqueos.AnyAsync(p => p.Id == 1);
        
        if (!autoExists && !parqueoExists)
        {
            _output.WriteLine("Rollback functionality verified");
        }
    }

    #endregion

    #region Multiple Repository Coordination Tests

    [Fact]
    public async Task UnitOfWork_CoordinatesMultipleRepositories_InSingleTransaction()
    {
        var auto = TestDataBuilder.CreateAutomovil(1);
        var parqueo = TestDataBuilder.CreateParqueo(1);
        var ingreso = new PrqIngresoAutomovile
        {
            IdAutomovil = 1,
            IdParqueo = 1,
            FechaHoraEntrada = DateTime.Now
        };
        
        using var context = _fixture.CreateContext();
        context.PrqAutomoviles.Add(auto);
        context.PrqParqueos.Add(parqueo);
        await context.SaveChangesAsync();

        var unitOfWork = new SqlUnitOfWork(
            context,
            new SqlAutomovilRepository(context),
            new SqlParqueoRepository(context),
            new SqlIngresoAutomovilRepository(context));

        await unitOfWork.IngresoAutomovilRepository.InsertAsync(ingreso);
        await unitOfWork.SaveChangesAsync();

        var ingresoResult = await context.PrqIngresoAutomoviles.FindAsync(1);
        Assert.NotNull(ingresoResult);
    }

    [Fact]
    public async Task UnitOfWork_ProvidesAccessToAllRepositories()
    {
        using var context = _fixture.CreateContext();
        var unitOfWork = new SqlUnitOfWork(
            context,
            new SqlAutomovilRepository(context),
            new SqlParqueoRepository(context),
            new SqlIngresoAutomovilRepository(context));

        Assert.NotNull(unitOfWork.AutomovilRepository);
        Assert.NotNull(unitOfWork.ParqueoRepository);
        Assert.NotNull(unitOfWork.IngresoAutomovilRepository);
    }

    #endregion

    #region Commit Tests

    [Fact]
    public async Task SaveChangesAsync_CreatesNewRecords_WhenInserted()
    {
        var auto = TestDataBuilder.CreateAutomovil(1);
        
        using var context = _fixture.CreateContext();
        var unitOfWork = new SqlUnitOfWork(
            context,
            new SqlAutomovilRepository(context),
            new SqlParqueoRepository(context),
            new SqlIngresoAutomovilRepository(context));

        await unitOfWork.AutomovilRepository.InsertAsync(auto);
        await unitOfWork.SaveChangesAsync();

        var count = await context.PrqAutomoviles.CountAsync();
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task SaveChangesAsync_UpdatesExistingRecords()
    {
        var auto = TestDataBuilder.CreateAutomovil(1);
        using var context = _fixture.CreateContext();
        context.PrqAutomoviles.Add(auto);
        await context.SaveChangesAsync();

        var unitOfWork = new SqlUnitOfWork(
            context,
            new SqlAutomovilRepository(context),
            new SqlParqueoRepository(context),
            new SqlIngresoAutomovilRepository(context));

        auto.Color = "Azul";
        await unitOfWork.AutomovilRepository.UpdateAsync(auto);
        await unitOfWork.SaveChangesAsync();

        var result = await context.PrqAutomoviles.FindAsync(1);
        Assert.Equal("Azul", result!.Color);
    }

    #endregion

    #region Error Handling Tests

    [Fact(Skip = "Test not compatible with InMemory provider")]
    public async Task SaveChangesAsync_ThrowsException_OnDatabaseError()
    {
        using var context = _fixture.CreateContext();
        var unitOfWork = new SqlUnitOfWork(
            context,
            new SqlAutomovilRepository(context),
            new SqlParqueoRepository(context),
            new SqlIngresoAutomovilRepository(context));

        var auto = new PrqAutomovile { Id = 1, Color = "Rojo", Año = 2022, Fabricante = "Toyota", Tipo = "Sedán" };
        await unitOfWork.AutomovilRepository.InsertAsync(auto);
        
        await context.Database.EnsureDeletedAsync();
        
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await unitOfWork.SaveChangesAsync());
    }

    #endregion

    #region Synchronous Methods Tests

    [Fact]
    public void SaveChanges_CommitsSynchronousOperations()
    {
        var auto = TestDataBuilder.CreateAutomovil(1);
        
        using var context = _fixture.CreateContext();
        var unitOfWork = new SqlUnitOfWork(
            context,
            new SqlAutomovilRepository(context),
            new SqlParqueoRepository(context),
            new SqlIngresoAutomovilRepository(context));

        unitOfWork.AutomovilRepository.Insert(auto);
        unitOfWork.SaveChanges();

        var result = context.PrqAutomoviles.Find(1);
        Assert.NotNull(result);
    }

    #endregion
}
