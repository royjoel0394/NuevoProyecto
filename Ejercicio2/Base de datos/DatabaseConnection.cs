using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ParqueoDatabaseExample.Models;

namespace ParqueoDatabaseExample
{
    public class DatabaseConnection
    {
        private readonly IConfiguration _configuration;

        public DatabaseConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Método para probar la conexión usando EF Core
        /// </summary>
        public async Task TestConnectionAsync()
        {
            using (var context = new ParqueoDbContext())
            {
                try
                {
                    // Intentar contar los automóviles
                    int count = await context.PrqAutomoviles.CountAsync();
                    Console.WriteLine("✅ Conexión exitosa a SQL Server en la nube");
                    Console.WriteLine($"📊 Total de automóviles registrados: {count}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error de conexión: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"   Inner: {ex.InnerException.Message}");
                    }
                    throw;
                }
            }
        }

        /// <summary>
        /// Método para consultar datos de las tablas del parqueo usando EF Core
        /// </summary>
        public async Task ConsultarDatosParqueoAsync()
        {
            using (var context = new ParqueoDbContext())
            {
                // Consulta de automóviles
                Console.WriteLine("\n🚗 AUTOMÓVILES REGISTRADOS:");
                var automoviles = await context.PrqAutomoviles
                    .OrderBy(a => a.Id)
                    .ToListAsync();

                foreach (var auto in automoviles)
                {
                    Console.WriteLine($"ID: {auto.Id}, {auto.Fabricante} {auto.Tipo} {auto.Color} ({auto.Año})");
                }

                // Consulta de parqueos
                Console.WriteLine("\n🏢 PARQUEOS DISPONIBLES:");
                var parqueos = await context.PrqParqueos
                    .OrderBy(p => p.Id)
                    .ToListAsync();

                foreach (var parqueo in parqueos)
                {
                    Console.WriteLine($"ID: {parqueo.Id}, {parqueo.Nombre} ({parqueo.NombreDeProvincia}) - ₡{parqueo.PrecioPorHora}/hora");
                }

                // Consulta de ingresos activos (sin salida)
                Console.WriteLine("\n🔄 INGRESOS ACTIVOS (AUTOMÓVILES EN PARQUEO):");
                var ingresosActivos = await context.PrqIngresoAutomoviles
                    .Include(i => i.IdAutomovilNavigation)
                    .Include(i => i.IdParqueoNavigation)
                    .Where(i => i.FechaHoraSalida == null)
                    .OrderByDescending(i => i.FechaHoraEntrada)
                    .ToListAsync();

                foreach (var ingreso in ingresosActivos)
                {
                    Console.WriteLine($"#{ingreso.Consecutivo}: {ingreso.IdAutomovilNavigation.Fabricante} {ingreso.IdAutomovilNavigation.Tipo} en {ingreso.IdParqueoNavigation.Nombre} desde {ingreso.FechaHoraEntrada}");
                }
            }
        }

        /// <summary>
        /// Método para calcular estadísticas del parqueo
        /// </summary>
        public async Task MostrarEstadisticasAsync()
        {
            using (var context = new ParqueoDbContext())
            {
                Console.WriteLine("\n📈 ESTADÍSTICAS DEL PARQUEO:");

                // Total de ingresos por parqueo
                var ingresosPorParqueo = await context.PrqIngresoAutomoviles
                    .Include(i => i.IdParqueoNavigation)
                    .GroupBy(i => i.IdParqueoNavigation.Nombre)
                    .Select(g => new { Parqueo = g.Key, TotalIngresos = g.Count() })
                    .OrderByDescending(x => x.TotalIngresos)
                    .ToListAsync();

                Console.WriteLine("🏢 Ingresos por parqueo:");
                foreach (var stat in ingresosPorParqueo)
                {
                    Console.WriteLine($"  {stat.Parqueo}: {stat.TotalIngresos} ingresos");
                }

                // Tipos de vehículos más comunes
                var tiposVehiculos = await context.PrqAutomoviles
                    .GroupBy(a => a.Tipo)
                    .Select(g => new { Tipo = g.Key, Cantidad = g.Count() })
                    .OrderByDescending(x => x.Cantidad)
                    .ToListAsync();

                Console.WriteLine("\n🚗 Tipos de vehículos registrados:");
                foreach (var tipo in tiposVehiculos)
                {
                    Console.WriteLine($"  {tipo.Tipo}: {tipo.Cantidad} unidades");
                }

                // Ingresos del día actual
                var hoy = DateTime.Today;
                var ingresosHoy = await context.PrqIngresoAutomoviles
                    .Where(i => i.FechaHoraEntrada.Date == hoy)
                    .CountAsync();

                Console.WriteLine($"\n📅 Ingresos de hoy ({hoy:yyyy-MM-dd}): {ingresosHoy}");
            }
        }
    }
}