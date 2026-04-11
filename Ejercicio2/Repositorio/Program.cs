using System;
using System.IO;
using System.Threading.Tasks;
using Ejercicio2.Repositorio.Configuration;
using Ejercicio2.Repositorio.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

static class Program
{
    public static async Task<int> Main()
    {
        var baseDirectory = Directory.GetCurrentDirectory();
        var settingsPath = Path.Combine(baseDirectory, "appsettings.json");
        if (!File.Exists(settingsPath))
        {
            settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(settingsPath) ?? baseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddRepositorio(configuration);

        var provider = services.BuildServiceProvider();

        var automovilRepo = provider.GetRequiredService<IAutomovilRepository>();
        var parqueoRepo = provider.GetRequiredService<IParqueoRepository>();
        var ingresoRepo = provider.GetRequiredService<IIngresoAutomovilRepository>();

        Console.WriteLine("Iniciando pruebas de repositorio...");

        await RunAutomovilTestsAsync(automovilRepo);
        await RunParqueoTestsAsync(parqueoRepo);
        await RunIngresoTestsAsync(ingresoRepo);

        Console.WriteLine("Pruebas completadas.");
        return 0;
    }

    private static async Task RunAutomovilTestsAsync(IAutomovilRepository repository)
    {
        Console.WriteLine("\n-- Pruebas Automóvil --");

        var all = await repository.GetAllAsync();
        Console.WriteLine($"Total automóviles: {all.Count}");

        var byColor = await repository.GetByColorAsync("blanco");
        Console.WriteLine($"Automóviles con color 'blanco': {byColor.Count}");

        var byRange = await repository.GetByYearRangeAsync(2010, 2020);
        Console.WriteLine($"Automóviles entre 2010 y 2020: {byRange.Count}");

        var byManufacturer = await repository.GetByManufacturerAsync("Toyota");
        Console.WriteLine($"Automóviles fabricante 'Toyota': {byManufacturer.Count}");

        var byType = await repository.GetByTypeAsync("Sedan");
        Console.WriteLine($"Automóviles tipo 'Sedan': {byType.Count}");
    }

    private static async Task RunParqueoTestsAsync(IParqueoRepository repository)
    {
        Console.WriteLine("\n-- Pruebas Parqueo --");

        var all = await repository.GetAllAsync();
        Console.WriteLine($"Total parqueos: {all.Count}");

        var byProvincia = await repository.GetByProvinciaAsync("San José");
        Console.WriteLine($"Parqueos en provincia 'San José': {byProvincia.Count}");

        var byNombre = await repository.GetByNombreAsync("Centro");
        Console.WriteLine($"Parqueos con nombre 'Centro': {byNombre.Count}");

        var byPrecio = await repository.GetByPrecioHoraRangeAsync(5m, 20m);
        Console.WriteLine($"Parqueos con precio por hora entre 5 y 20: {byPrecio.Count}");
    }

    private static async Task RunIngresoTestsAsync(IIngresoAutomovilRepository repository)
    {
        Console.WriteLine("\n-- Pruebas IngresoAutomoviles --");

        var all = await repository.GetAllAsync();
        Console.WriteLine($"Total ingresos: {all.Count}");

        if (all.Count > 0)
        {
            var primerIngreso = all[0];
            var precio = await repository.GetPrecioPorHoraAsync(primerIngreso.IdParqueo);
            Console.WriteLine($"Precio por hora del primer ingreso (parqueo {primerIngreso.IdParqueo}): {precio?.ToString("F2") ?? "no encontrado"}");
        }

        var desde = new DateTime(2023, 1, 1);
        var hasta = new DateTime(2025, 12, 31);

        var byTipo = await repository.GetIngresosPorTipoAutomovilAsync("Sedan", desde, hasta);
        Console.WriteLine($"Ingresos por tipo 'Sedan' entre {desde:yyyy-MM-dd} y {hasta:yyyy-MM-dd}: {byTipo.Count}");

        var byProvincia = await repository.GetIngresosPorProvinciaAsync("San José", desde, hasta);
        Console.WriteLine($"Ingresos por provincia 'San José' entre {desde:yyyy-MM-dd} y {hasta:yyyy-MM-dd}: {byProvincia.Count}");
    }
}
