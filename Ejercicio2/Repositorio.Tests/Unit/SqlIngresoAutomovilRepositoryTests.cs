using Repositorio.Tests.Fixtures;
using Repositorio.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;
using ParqueoDatabaseExample.Models;
using Ejercicio2.Repositorio.Dtos;
using Ejercicio2.Repositorio.Repositories.Sql;

namespace Repositorio.Tests.Unit;

[Collection("SqlDatabase")]
public class SqlIngresoAutomovilRepositoryTests
{
    private readonly SqlDatabaseFixture _fixture;
    private readonly ITestOutputHelper _output;

    public SqlIngresoAutomovilRepositoryTests(SqlDatabaseFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    private async Task SeedFullContextAsync()
    {
        var auto = new PrqAutomovile { Id = 1, Color = "Rojo", Año = 2022, Fabricante = "Toyota", Tipo = "Sedán" };
        var parqueo = new PrqParqueo { Id = 1, Nombre = "Parqueo Central", NombreDeProvincia = "San José", PrecioPorHora = 10.00m };
        using var context = _fixture.CreateContext();
        context.PrqAutomoviles.Add(auto);
        context.PrqParqueos.Add(parqueo);
        await context.SaveChangesAsync();
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsAllIngresos()
    {
        await SeedFullContextAsync();
        var ingresos = new List<PrqIngresoAutomovile>
        {
            new() { Consecutivo = 1, IdAutomovil = 1, IdParqueo = 1, FechaHoraEntrada = DateTime.Now.AddHours(-2), FechaHoraSalida = DateTime.Now },
            new() { Consecutivo = 2, IdAutomovil = 1, IdParqueo = 1, FechaHoraEntrada = DateTime.Now.AddHours(-1) }
        };
        using var context = _fixture.CreateContext();
        context.PrqIngresoAutomoviles.AddRange(ingresos);
        await context.SaveChangesAsync();

        var repository = new SqlIngresoAutomovilRepository(context);
        var result = await repository.GetAllAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoData()
    {
        using var context = _fixture.CreateContext();
        var repository = new SqlIngresoAutomovilRepository(context);
        var result = await repository.GetAllAsync();

        Assert.Empty(result);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ReturnsIngreso_WhenExists()
    {
        await SeedFullContextAsync();
        var ingreso = new PrqIngresoAutomovile
        {
            Consecutivo = 1,
            IdAutomovil = 1,
            IdParqueo = 1,
            FechaHoraEntrada = DateTime.Now
        };
        using var context = _fixture.CreateContext();
        context.PrqIngresoAutomoviles.Add(ingreso);
        await context.SaveChangesAsync();

        var repository = new SqlIngresoAutomovilRepository(context);
        var result = await repository.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.IdAutomovil);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        using var context = _fixture.CreateContext();
        var repository = new SqlIngresoAutomovilRepository(context);
        var result = await repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    #endregion

    #region GetPrecioPorHoraAsync Tests

    [Fact]
    public async Task GetPrecioPorHoraAsync_ReturnsPrecio_WhenParqueoExists()
    {
        var parqueo = new PrqParqueo { Id = 1, Nombre = "Parqueo Central", NombreDeProvincia = "San José", PrecioPorHora = 15.50m };
        using var context = _fixture.CreateContext();
        context.PrqParqueos.Add(parqueo);
        await context.SaveChangesAsync();

        var repository = new SqlIngresoAutomovilRepository(context);
        var result = await repository.GetPrecioPorHoraAsync(1);

        Assert.Equal(15.50m, result);
    }

    [Fact]
    public async Task GetPrecioPorHoraAsync_ReturnsNull_WhenParqueoNotExists()
    {
        using var context = _fixture.CreateContext();
        var repository = new SqlIngresoAutomovilRepository(context);
        var result = await repository.GetPrecioPorHoraAsync(999);

        Assert.Null(result);
    }

    #endregion

    #region GetIngresosPorTipoAutomovilAsync Tests

    [Fact(Skip = "Requires SQL Server - DateDiffMinute not supported by InMemory provider")]
    public async Task GetIngresosPorTipoAutomovilAsync_ReturnsMatchingIngresos()
    {
        var auto = new PrqAutomovile { Id = 1, Color = "Rojo", Año = 2022, Fabricante = "Toyota", Tipo = "Sedán" };
        var auto2 = new PrqAutomovile { Id = 2, Color = "Azul", Año = 2023, Fabricante = "Honda", Tipo = "SUV" };
        var parqueo = new PrqParqueo { Id = 1, Nombre = "Parqueo Central", NombreDeProvincia = "San José", PrecioPorHora = 10.00m };
        var ingreso1 = new PrqIngresoAutomovile
        {
            Consecutivo = 1,
            IdAutomovil = 1,
            IdParqueo = 1,
            FechaHoraEntrada = DateTime.Now.AddDays(-5),
            FechaHoraSalida = DateTime.Now.AddDays(-5).AddHours(2)
        };
        var ingreso2 = new PrqIngresoAutomovile
        {
            Consecutivo = 2,
            IdAutomovil = 2,
            IdParqueo = 1,
            FechaHoraEntrada = DateTime.Now.AddDays(-3),
            FechaHoraSalida = DateTime.Now.AddDays(-3).AddHours(1)
        };
        using var context = _fixture.CreateContext();
        context.PrqAutomoviles.AddRange(auto, auto2);
        context.PrqParqueos.Add(parqueo);
        context.PrqIngresoAutomoviles.AddRange(ingreso1, ingreso2);
        await context.SaveChangesAsync();

        var repository = new SqlIngresoAutomovilRepository(context);
        var desde = DateTime.Now.AddDays(-10);
        var hasta = DateTime.Now;
        var result = await repository.GetIngresosPorTipoAutomovilAsync("Sedán", desde, hasta);

        Assert.Single(result);
        Assert.Equal("Sedán", result.First().Tipo);
    }

    [Fact]
    public async Task GetIngresosPorTipoAutomovilAsync_ReturnsEmptyList_WhenNoMatch()
    {
        await SeedFullContextAsync();
        using var context = _fixture.CreateContext();
        var repository = new SqlIngresoAutomovilRepository(context);
        var result = await repository.GetIngresosPorTipoAutomovilAsync("Inexistente", DateTime.Now.AddDays(-10), DateTime.Now);

        Assert.Empty(result);
    }

    #endregion

    #region GetIngresosPorProvinciaAsync Tests

    [Fact(Skip = "Requires SQL Server - DateDiffMinute not supported by InMemory provider")]
    public async Task GetIngresosPorProvinciaAsync_ReturnsMatchingIngresos()
    {
        var auto = new PrqAutomovile { Id = 1, Color = "Rojo", Año = 2022, Fabricante = "Toyota", Tipo = "Sedán" };
        var parqueo1 = new PrqParqueo { Id = 1, Nombre = "Parqueo SJ", NombreDeProvincia = "San José", PrecioPorHora = 10.00m };
        var parqueo2 = new PrqParqueo { Id = 2, Nombre = "Parqueo ALA", NombreDeProvincia = "Alajuela", PrecioPorHora = 12.00m };
        var ingreso1 = new PrqIngresoAutomovile
        {
            Consecutivo = 1,
            IdAutomovil = 1,
            IdParqueo = 1,
            FechaHoraEntrada = DateTime.Now.AddDays(-5),
            FechaHoraSalida = DateTime.Now.AddDays(-5).AddHours(2)
        };
        var ingreso2 = new PrqIngresoAutomovile
        {
            Consecutivo = 2,
            IdAutomovil = 1,
            IdParqueo = 2,
            FechaHoraEntrada = DateTime.Now.AddDays(-3),
            FechaHoraSalida = DateTime.Now.AddDays(-3).AddHours(1)
        };
        using var context = _fixture.CreateContext();
        context.PrqAutomoviles.Add(auto);
        context.PrqParqueos.AddRange(parqueo1, parqueo2);
        context.PrqIngresoAutomoviles.AddRange(ingreso1, ingreso2);
        await context.SaveChangesAsync();

        var repository = new SqlIngresoAutomovilRepository(context);
        var desde = DateTime.Now.AddDays(-10);
        var hasta = DateTime.Now;
        var result = await repository.GetIngresosPorProvinciaAsync("San José", desde, hasta);

        Assert.Single(result);
        Assert.Equal("San José", result.First().Provincia);
    }

    [Fact]
    public async Task GetIngresosPorProvinciaAsync_ReturnsEmptyList_WhenNoMatch()
    {
        await SeedFullContextAsync();
        using var context = _fixture.CreateContext();
        var repository = new SqlIngresoAutomovilRepository(context);
        var result = await repository.GetIngresosPorProvinciaAsync("Cartago", DateTime.Now.AddDays(-10), DateTime.Now);

        Assert.Empty(result);
    }

    #endregion

    #region InsertAsync Tests

    [Fact]
    public async Task InsertAsync_AddsIngresoToDatabase()
    {
        await SeedFullContextAsync();
        using var context = _fixture.CreateContext();
        var repository = new SqlIngresoAutomovilRepository(context);
        var ingreso = new PrqIngresoAutomovile
        {
            IdAutomovil = 1,
            IdParqueo = 1,
            FechaHoraEntrada = DateTime.Now
        };

        await repository.InsertAsync(ingreso);

        var result = await context.PrqIngresoAutomoviles.FindAsync(1);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task InsertAsync_GeneratesConsecutivo_WhenNotProvided()
    {
        await SeedFullContextAsync();
        using var context = _fixture.CreateContext();
        var repository = new SqlIngresoAutomovilRepository(context);
        var ingreso = new PrqIngresoAutomovile
        {
            IdAutomovil = 1,
            IdParqueo = 1,
            FechaHoraEntrada = DateTime.Now
        };

        await repository.InsertAsync(ingreso);

        Assert.NotEqual(0, ingreso.Consecutivo);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ModifiesExistingIngreso()
    {
        await SeedFullContextAsync();
        var ingreso = new PrqIngresoAutomovile
        {
            Consecutivo = 1,
            IdAutomovil = 1,
            IdParqueo = 1,
            FechaHoraEntrada = DateTime.Now.AddHours(-2)
        };
        using var context = _fixture.CreateContext();
        context.PrqIngresoAutomoviles.Add(ingreso);
        await context.SaveChangesAsync();

        var repository = new SqlIngresoAutomovilRepository(context);
        ingreso.FechaHoraSalida = DateTime.Now;
        await repository.UpdateAsync(ingreso);

        var result = await context.PrqIngresoAutomoviles.FindAsync(1);
        Assert.NotNull(result.FechaHoraSalida);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsException_WhenIngresoNotExists()
    {
        await SeedFullContextAsync();
        using var context = _fixture.CreateContext();
        var repository = new SqlIngresoAutomovilRepository(context);
        var ingreso = new PrqIngresoAutomovile
        {
            Consecutivo = 999,
            IdAutomovil = 1,
            IdParqueo = 1,
            FechaHoraEntrada = DateTime.Now
        };

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await repository.UpdateAsync(ingreso));
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_RemovesIngresoFromDatabase()
    {
        await SeedFullContextAsync();
        var ingreso = new PrqIngresoAutomovile
        {
            Consecutivo = 1,
            IdAutomovil = 1,
            IdParqueo = 1,
            FechaHoraEntrada = DateTime.Now
        };
        using var context = _fixture.CreateContext();
        context.PrqIngresoAutomoviles.Add(ingreso);
        await context.SaveChangesAsync();

        var repository = new SqlIngresoAutomovilRepository(context);
        await repository.DeleteAsync(1);

        var result = await context.PrqIngresoAutomoviles.FindAsync(1);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsException_WhenIngresoNotExists()
    {
        await SeedFullContextAsync();
        using var context = _fixture.CreateContext();
        var repository = new SqlIngresoAutomovilRepository(context);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await repository.DeleteAsync(999));
    }

    #endregion

    #region ExistsAsync Tests

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenIngresoExists()
    {
        await SeedFullContextAsync();
        var ingreso = new PrqIngresoAutomovile
        {
            Consecutivo = 1,
            IdAutomovil = 1,
            IdParqueo = 1,
            FechaHoraEntrada = DateTime.Now
        };
        using var context = _fixture.CreateContext();
        context.PrqIngresoAutomoviles.Add(ingreso);
        await context.SaveChangesAsync();

        var repository = new SqlIngresoAutomovilRepository(context);
        var result = await repository.ExistsAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenIngresoNotExists()
    {
        await SeedFullContextAsync();
        using var context = _fixture.CreateContext();
        var repository = new SqlIngresoAutomovilRepository(context);
        var result = await repository.ExistsAsync(999);

        Assert.False(result);
    }

    #endregion
}
