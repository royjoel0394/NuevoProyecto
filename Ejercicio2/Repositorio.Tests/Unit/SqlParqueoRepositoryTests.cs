using Repositorio.Tests.Fixtures;
using Repositorio.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;
using ParqueoDatabaseExample.Models;
using Ejercicio2.Repositorio.Repositories.Sql;

namespace Repositorio.Tests.Unit;

[Collection("SqlDatabase")]
public class SqlParqueoRepositoryTests
{
    private readonly SqlDatabaseFixture _fixture;
    private readonly ITestOutputHelper _output;

    public SqlParqueoRepositoryTests(SqlDatabaseFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsAllParqueos()
    {
        var parqueos = TestDataBuilder.CreateMultipleParqueos().ToList();
        using var context = _fixture.CreateContext();
        context.PrqParqueos.AddRange(parqueos);
        await context.SaveChangesAsync();

        var repository = new SqlParqueoRepository(context);
        var result = await repository.GetAllAsync();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoData()
    {
        using var context = _fixture.CreateContext();
        var repository = new SqlParqueoRepository(context);
        var result = await repository.GetAllAsync();

        Assert.Empty(result);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ReturnsParqueo_WhenExists()
    {
        var parqueo = TestDataBuilder.CreateParqueo(1);
        using var context = _fixture.CreateContext();
        context.PrqParqueos.Add(parqueo);
        await context.SaveChangesAsync();

        var repository = new SqlParqueoRepository(context);
        var result = await repository.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Parqueo Central", result.Nombre);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        using var context = _fixture.CreateContext();
        var repository = new SqlParqueoRepository(context);
        var result = await repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    #endregion

    #region GetByNombreAsync Tests

    [Fact]
    public async Task GetByNombreAsync_ReturnsMatchingParqueos()
    {
        var parqueos = TestDataBuilder.CreateMultipleParqueos().ToList();
        using var context = _fixture.CreateContext();
        context.PrqParqueos.AddRange(parqueos);
        await context.SaveChangesAsync();

        var repository = new SqlParqueoRepository(context);
        var result = await repository.GetByNombreAsync("Central");

        Assert.Single(result);
        Assert.Equal("Parqueo Central", result.First().Nombre);
    }

    [Fact]
    public async Task GetByNombreAsync_ReturnsEmptyList_WhenNoMatch()
    {
        using var context = _fixture.CreateContext();
        var repository = new SqlParqueoRepository(context);
        var result = await repository.GetByNombreAsync("Inexistente");

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByNombreAsync_IsCaseInsensitive()
    {
        var parqueo = new PrqParqueo { Id = 1, Nombre = "PARQUEO CENTRAL", NombreDeProvincia = "San José", PrecioPorHora = 10.00m };
        using var context = _fixture.CreateContext();
        context.PrqParqueos.Add(parqueo);
        await context.SaveChangesAsync();

        var repository = new SqlParqueoRepository(context);
        var result = await repository.GetByNombreAsync("central");

        Assert.Single(result);
    }

    #endregion

    #region GetByPrecioHoraRangeAsync Tests

    [Fact]
    public async Task GetByPrecioHoraRangeAsync_ReturnsMatchingParqueos()
    {
        var parqueos = TestDataBuilder.CreateMultipleParqueos().ToList();
        using var context = _fixture.CreateContext();
        context.PrqParqueos.AddRange(parqueos);
        await context.SaveChangesAsync();

        var repository = new SqlParqueoRepository(context);
        var result = await repository.GetByPrecioHoraRangeAsync(10.00m, 15.00m);

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetByPrecioHoraRangeAsync_ReturnsEmptyList_WhenNoMatch()
    {
        using var context = _fixture.CreateContext();
        var repository = new SqlParqueoRepository(context);
        var result = await repository.GetByPrecioHoraRangeAsync(100.00m, 200.00m);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByPrecioHoraRangeAsync_IncludesBoundaries()
    {
        var p1 = new PrqParqueo { Id = 1, Nombre = "Barato", NombreDeProvincia = "San José", PrecioPorHora = 10.00m };
        var p2 = new PrqParqueo { Id = 2, Nombre = "Caro", NombreDeProvincia = "San José", PrecioPorHora = 20.00m };
        using var context = _fixture.CreateContext();
        context.PrqParqueos.AddRange(p1, p2);
        await context.SaveChangesAsync();

        var repository = new SqlParqueoRepository(context);
        var result = await repository.GetByPrecioHoraRangeAsync(10.00m, 10.00m);

        Assert.Single(result);
    }

    #endregion

    #region GetByProvinciaAsync Tests

    [Fact]
    public async Task GetByProvinciaAsync_ReturnsMatchingParqueos()
    {
        var parqueos = TestDataBuilder.CreateMultipleParqueos().ToList();
        using var context = _fixture.CreateContext();
        context.PrqParqueos.AddRange(parqueos);
        await context.SaveChangesAsync();

        var repository = new SqlParqueoRepository(context);
        var result = await repository.GetByProvinciaAsync("San José");

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByProvinciaAsync_IsCaseInsensitive()
    {
        var parqueo = new PrqParqueo { Id = 1, Nombre = "Parqueo", NombreDeProvincia = "San Jose", PrecioPorHora = 10.00m };
        using var context = _fixture.CreateContext();
        context.PrqParqueos.Add(parqueo);
        await context.SaveChangesAsync();

        var repository = new SqlParqueoRepository(context);
        var result = await repository.GetByProvinciaAsync("san jose");

        Assert.Single(result);
    }

    #endregion

    #region InsertAsync Tests

    [Fact]
    public async Task InsertAsync_AddsParqueoToDatabase()
    {
        using var context = _fixture.CreateContext();
        var repository = new SqlParqueoRepository(context);
        var parqueo = TestDataBuilder.CreateParqueo(1);

        await repository.InsertAsync(parqueo);

        var result = await context.PrqParqueos.FindAsync(1);
        Assert.NotNull(result);
        Assert.Equal("Parqueo Central", result.Nombre);
    }

    [Fact]
    public async Task InsertAsync_GeneratesId_WhenNotProvided()
    {
        using var context = _fixture.CreateContext();
        var repository = new SqlParqueoRepository(context);
        var parqueo = new PrqParqueo { Nombre = "Parqueo Test", NombreDeProvincia = "San José", PrecioPorHora = 10.00m };

        await repository.InsertAsync(parqueo);

        Assert.NotEqual(0, parqueo.Id);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ModifiesExistingParqueo()
    {
        var parqueo = TestDataBuilder.CreateParqueo(1);
        using var context = _fixture.CreateContext();
        context.PrqParqueos.Add(parqueo);
        await context.SaveChangesAsync();

        var repository = new SqlParqueoRepository(context);
        parqueo.PrecioPorHora = 20.00m;
        await repository.UpdateAsync(parqueo);

        var result = await context.PrqParqueos.FindAsync(1);
        Assert.Equal(20.00m, result!.PrecioPorHora);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsException_WhenParqueoNotExists()
    {
        using var context = _fixture.CreateContext();
        var repository = new SqlParqueoRepository(context);
        var parqueo = new PrqParqueo { Id = 999, Nombre = "Parqueo Test", NombreDeProvincia = "San José", PrecioPorHora = 10.00m };

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await repository.UpdateAsync(parqueo));
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_RemovesParqueoFromDatabase()
    {
        var parqueo = TestDataBuilder.CreateParqueo(1);
        using var context = _fixture.CreateContext();
        context.PrqParqueos.Add(parqueo);
        await context.SaveChangesAsync();

        var repository = new SqlParqueoRepository(context);
        await repository.DeleteAsync(1);

        var result = await context.PrqParqueos.FindAsync(1);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsException_WhenParqueoHasIngresos()
    {
        var parqueo = TestDataBuilder.CreateParqueo(1);
        var ingreso = new PrqIngresoAutomovile
        {
            Consecutivo = 1,
            IdAutomovil = 1,
            IdParqueo = 1,
            FechaHoraEntrada = DateTime.Now
        };
        using var context = _fixture.CreateContext();
        context.PrqParqueos.Add(parqueo);
        context.PrqIngresoAutomoviles.Add(ingreso);
        await context.SaveChangesAsync();

        var repository = new SqlParqueoRepository(context);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await repository.DeleteAsync(1));
    }

    [Fact]
    public async Task DeleteAsync_ThrowsException_WhenParqueoNotExists()
    {
        using var context = _fixture.CreateContext();
        var repository = new SqlParqueoRepository(context);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await repository.DeleteAsync(999));
    }

    #endregion

    #region ExistsAsync Tests

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenParqueoExists()
    {
        var parqueo = TestDataBuilder.CreateParqueo(1);
        using var context = _fixture.CreateContext();
        context.PrqParqueos.Add(parqueo);
        await context.SaveChangesAsync();

        var repository = new SqlParqueoRepository(context);
        var result = await repository.ExistsAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenParqueoNotExists()
    {
        using var context = _fixture.CreateContext();
        var repository = new SqlParqueoRepository(context);
        var result = await repository.ExistsAsync(999);

        Assert.False(result);
    }

    #endregion
}
