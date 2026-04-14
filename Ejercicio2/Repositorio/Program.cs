using System;
using System.IO;
using System.Threading.Tasks;
using Ejercicio2.Repositorio.Configuration;
using Ejercicio2.Repositorio.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

        var app = builder.Build();

        app.UseAuthorization();
        app.MapControllers();

        Console.WriteLine("Iniciando Parking API...");
        Console.WriteLine("API disponible en: http://localhost:5000");
        
        await app.RunAsync("http://localhost:5000");
    }
}
