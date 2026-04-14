using Repositorio.Tests.Fixtures;
using Repositorio.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;
using ParqueoDatabaseExample.Models;
using Ejercicio2.Repositorio.Repositories.Sql;

namespace Repositorio.Tests.Unit;

[Collection("SqlDatabase")]
public class SqlAutomovilRepositoryTests
{
    private readonly SqlDatabaseFixture _fixture;
    private readonly ITestOutputHelper _output;

    public SqlAutomovilRepositoryTests(SqlDatabaseFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsAllAutomoviles()
    {
        var automoviles = TestDataBuilder.CreateMultipleAutomoviles().ToList();
        using var context = _fixture.CreateContext();
        context.PrqAutomoviles.AddRange(automoviles);
        await context.SaveChangesAsync();

        var repository = new SqlAutomovilRepository(context);
        var result = await repository.GetAllAsync();

        Assert.Equal(5, result.Count);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoData()
    {
        using var context = _fixture.CreateContext();
        var repository = new SqlAutomovilRepository(context);
        var result = await repository.GetAllAsync();

        Assert.Empty(result);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ReturnsAutomovil_WhenExists()
    {
        var auto = TestDataBuilder.CreateAutomovil(1);
        using var context = _fixture.CreateContext();
        context.PrqAutomoviles.Add(auto);
        await context.SaveChangesAsync();

        var repository = new SqlAutomovilRepository(context);
        var result = await repository.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Toyota", result.Fabricante);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        using var context = _fixture.CreateContext();
        var repository = new SqlAutomovilRepository(context);
        var result = await repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    #endregion

    #region GetByColorAsync Tests

    [Fact]
    public async Task GetByColorAsync_ReturnsMatchingAutomoviles()
    {
        var automoviles = TestDataBuilder.CreateMultipleAutomoviles().ToList();
        using var context = _fixture.CreateContext();
        context.PrqAutomoviles.AddRange(automoviles);
        await context.SaveChangesAsync();

        var repository = new SqlAutomovilRepository(context);
        var result = await repository.GetByColorAsync("Rojo");

        Assert.Equal(2, result.Count);
        Assert.All(result, a => Assert.Contains("Rojo", a.Color, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetByColorAsync_ReturnsEmptyList_WhenNoMatch()
    {
        using var context = _fixture.CreateContext();
        var repository = new SqlAutomovilRepository(context);
        var result = await repository.GetByColorAsync("Morado");

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByColorAsync_IsCaseInsensitive()
    {
        var auto = TestDataBuilder.CreateAutomovil(1);
        auto.Color = "ROJO";
        using var context = _fixture.CreateContext();
        context.PrqAutomoviles.Add(auto);
        await context.SaveChangesAsync();

        var repository = new SqlAutomovilRepository(context);
        var result = await repository.GetByColorAsync("rojo");

        Assert.Single(result);
    }

    #endregion

    #region GetByYearRangeAsync Tests

    [Fact]
    public async Task GetByYearRangeAsync_ReturnsMatchingAutomoviles()
    {
        var automoviles = TestDataBuilder.CreateMultipleAutomoviles().ToList();
        using var context = _fixture.CreateContext();
        context.PrqAutomoviles.AddRange(automoviles);
        await context.SaveChangesAsync();

        var repository = new SqlAutomovilRepository(context);
        var result = await repository.GetByYearRangeAsync(2022, 2023);

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetByYearRangeAsync_ReturnsEmptyList_WhenNoMatch()
    {
        using var context = _fixture.CreateContext();
        var repository = new SqlAutomovilRepository(context);
        var result = await repository.GetByYearRangeAsync(2010, 2015);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByYearRangeAsync_IncludesBoundaryYears()
    {
        var auto1 = new PrqAutomovile { Id = 1, Color = "Rojo", Año = 2020, Fabricante = "Toyota", Tipo = "Sedán" };
        var auto2 = new PrqAutomovile { Id = 2, Color = "Azul", Año = 2022, Fabricante = "Honda", Tipo = "SUV" };
        using var context = _fixture.CreateContext();
        context.PrqAutomoviles.AddRange(auto1, auto2);
        await context.SaveChangesAsync();

        var repository = new SqlAutomovilRepository(context);
        var result = await repository.GetByYearRangeAsync(2020, 2020);

        Assert.Single(result);
        Assert.Equal(2020, result.First().Año);
    }

    #endregion

    #region GetByManufacturerAsync Tests

    [Fact]
    public async Task GetByManufacturerAsync_ReturnsMatchingAutomoviles()
    {
        var automoviles = TestDataBuilder.CreateMultipleAutomoviles().ToList();
        using var context = _fixture.CreateContext();
        context.PrqAutomoviles.AddRange(automoviles);
        await context.SaveChangesAsync();

        var repository = new SqlAutomovilRepository(context);
        var result = await repository.GetByManufacturerAsync("Toyota");

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByManufacturerAsync_IsCaseInsensitive()
    {
        var auto = TestDataBuilder.CreateAutomovil(1);
        auto.Fabricante = "TOYOTA";
        using var context = _fixture.CreateContext();
        context.PrqAutomoviles.Add(auto);
        await context.SaveChangesAsync();

        var repository = new SqlAutomovilRepository(context);
        var result = await repository.GetByManufacturerAsync("toyota");

        Assert.Single(result);
    }

    #endregion

    #region GetByTypeAsync Tests

    [Fact]
    public async Task GetByTypeAsync_ReturnsMatchingAutomoviles()
    {
        var automoviles = TestDataBuilder.CreateMultipleAutomoviles().ToList();
        using var context = _fixture.CreateContext();
        context.PrqAutomoviles.AddRange(automoviles);
        await context.SaveChangesAsync();

        var repository = new SqlAutomovilRepository(context);
        var result = await repository.GetByTypeAsync("SUV");

        Assert.Equal(2, result.Count);
    }

    #endregion

    #region InsertAsync Tests

    [Fact]
    public async Task InsertAsync_AddsAutomovilToDatabase()
    {
        using var context = _fixture.CreateContext();
        var repository = new SqlAutomovilRepository(context);
        var auto = TestDataBuilder.CreateAutomovil(1);

        await repository.InsertAsync(auto);

        var result = await context.PrqAutomoviles.FindAsync(1);
        Assert.NotNull(result);
        Assert.Equal("Toyota", result.Fabricante);
    }

    [Fact]
    public async Task InsertAsync_GeneratesId_WhenNotProvided()
    {
        using var context = _fixture.CreateContext();
        var repository = new SqlAutomovilRepository(context);
        var auto = new PrqAutomovile { Color = "Rojo", Año = 2022, Fabricante = "Toyota", Tipo = "Sedán" };

        await repository.InsertAsync(auto);

        Assert.NotEqual(0, auto.Id);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ModifiesExistingAutomovil()
    {
        var auto = TestDataBuilder.CreateAutomovil(1);
        using var context = _fixture.CreateContext();
        context.PrqAutomoviles.Add(auto);
        await context.SaveChangesAsync();

        var repository = new SqlAutomovilRepository(context);
        auto.Color = "Verde";
        await repository.UpdateAsync(auto);

        var result = await context.PrqAutomoviles.FindAsync(1);
        Assert.Equal("Verde", result!.Color);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsException_WhenAutomovilNotExists()
    {
        using var context = _fixture.CreateContext();
        var repository = new SqlAutomovilRepository(context);
        var auto = new PrqAutomovile { Id = 999, Color = "Rojo", Año = 2022, Fabricante = "Toyota", Tipo = "Sedán" };

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await repository.UpdateAsync(auto));
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_RemovesAutomovilFromDatabase()
    {
        var auto = TestDataBuilder.CreateAutomovil(1);
        using var context = _fixture.CreateContext();
        context.PrqAutomoviles.Add(auto);
        await context.SaveChangesAsync();

        var repository = new SqlAutomovilRepository(context);
        await repository.DeleteAsync(1);

        var result = await context.PrqAutomoviles.FindAsync(1);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsException_WhenAutomovilHasIngresos()
    {
        var auto = TestDataBuilder.CreateAutomovil(1);
        var ingreso = new PrqIngresoAutomovile
        {
            Consecutivo = 1,
            IdAutomovil = 1,
            IdParqueo = 1,
            FechaHoraEntrada = DateTime.Now
        };
        using var context = _fixture.CreateContext();
        context.PrqAutomoviles.Add(auto);
        context.PrqIngresoAutomoviles.Add(ingreso);
        await context.SaveChangesAsync();

        var repository = new SqlAutomovilRepository(context);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await repository.DeleteAsync(1));
    }

    [Fact]
    public async Task DeleteAsync_ThrowsException_WhenAutomovilNotExists()
    {
        using var context = _fixture.CreateContext();
        var repository = new SqlAutomovilRepository(context);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await repository.DeleteAsync(999));
    }

    #endregion

    #region ExistsAsync Tests

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenAutomovilExists()
    {
        var auto = TestDataBuilder.CreateAutomovil(1);
        using var context = _fixture.CreateContext();
        context.PrqAutomoviles.Add(auto);
        await context.SaveChangesAsync();

        var repository = new SqlAutomovilRepository(context);
        var result = await repository.ExistsAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenAutomovilNotExists()
    {
        using var context = _fixture.CreateContext();
        var repository = new SqlAutomovilRepository(context);
        var result = await repository.ExistsAsync(999);

        Assert.False(result);
    }

    #endregion
}
