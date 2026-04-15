using System;
using System.IO;
using System.Threading.Tasks;
using Ejercicio2.Repositorio.Configuration;
using Ejercicio2.Repositorio.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

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

        builder.Services.AddSingleton<IConfiguration>(configuration);
        builder.Services.AddRepositorio(configuration);
        builder.Services.AddControllers();
        
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(5000);
            options.ListenAnyIP(5001, listenOptions =>
            {
                listenOptions.UseHttps();
            });
        });

        var app = builder.Build();

        app.UseAuthorization();
        app.MapControllers();

        Console.WriteLine("=================================");
        Console.WriteLine("  Parking API iniciada");
        Console.WriteLine("=================================");
        Console.WriteLine("Endpoints disponibles:");
        Console.WriteLine("  - GET    http://localhost:5000/api/automoviles");
        Console.WriteLine("  - GET    http://localhost:5000/api/parqueos");
        Console.WriteLine("  - GET    http://localhost:5000/api/ingresos");
        Console.WriteLine("=================================");
        
        await app.RunAsync();
    }
}
