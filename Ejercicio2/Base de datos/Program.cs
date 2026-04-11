using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ParqueoDatabaseExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("🚗 Sistema de Parqueo - Conexión a SQL Server en la Nube");
            Console.WriteLine("======================================================");

            try
            {
                // Configurar el host con configuración segura
                var host = Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((context, config) =>
                    {
                        config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                              .AddJsonFile("appsettings.json", optional: true)
                              .AddEnvironmentVariables();
                    })
                    .ConfigureServices((context, services) =>
                    {
                        services.AddSingleton<DatabaseConnection>();
                    })
                    .Build();

                var dbConnection = host.Services.GetRequiredService<DatabaseConnection>();

                Console.WriteLine("🔗 Probando conexión...");
                await dbConnection.TestConnectionAsync();

                Console.WriteLine("\n📊 Consultando datos del parqueo...");
                await dbConnection.ConsultarDatosParqueoAsync();

                Console.WriteLine("\n📈 Generando estadísticas...");
                await dbConnection.MostrarEstadisticasAsync();

                Console.WriteLine("\n✅ Operación completada exitosamente!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.WriteLine("Verifica que las variables de entorno estén configuradas correctamente.");
            }

            Console.WriteLine("\nPresiona cualquier tecla para salir...");
            if (!Console.IsInputRedirected)
            {
                Console.ReadKey();
            }
        }
    }
}