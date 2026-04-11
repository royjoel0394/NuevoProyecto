using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ParqueoDatabaseExample.Models;

namespace ParqueoDatabaseExample
{
    public class EjemplosEFIngresosCalculados
    {
        public async Task EjecutarAsync()
        {
            using var context = new ParqueoDbContext();

            // 1. Traer ingresos y calcularlos en memoria usando propiedades [NotMapped]
            var ingresos = await context.PrqIngresoAutomoviles
                .Include(i => i.IdParqueoNavigation)
                .Include(i => i.IdAutomovilNavigation)
                .OrderBy(i => i.Consecutivo)
                .ToListAsync();

            foreach (var ingreso in ingresos)
            {
                Console.WriteLine($"#{ingreso.Consecutivo}: auto={ingreso.IdAutomovilNavigation.Fabricante} {ingreso.IdAutomovilNavigation.Tipo}, " +
                                  $"entrada={ingreso.FechaHoraEntrada}, salida={ingreso.FechaHoraSalida}, " +
                                  $"minutos={ingreso.DuracionEstadiaMinutos}, horas={ingreso.DuracionEstadiaHoras}, total={ingreso.MontoTotalAPagar}");
            }

            // 2. Filtrar solo registros con salida y monto calculado no nulo
            var ingresosCerrados = ingresos
                .Where(i => i.FechaHoraSalida != null)
                .OrderByDescending(i => i.FechaHoraSalida);

            Console.WriteLine("\nIngresos cerrados:");
            foreach (var ingreso in ingresosCerrados)
            {
                Console.WriteLine($"#{ingreso.Consecutivo}: Duración = {ingreso.DuracionEstadiaHoras} h, Monto = {ingreso.MontoTotalAPagar}");
            }

            // 3. Total cobrado por parqueo (en memoria)
            var totalPorParqueo = ingresos
                .Where(i => i.MontoTotalAPagar.HasValue)
                .GroupBy(i => i.IdParqueoNavigation.Nombre)
                .Select(g => new
                {
                    Parqueo = g.Key,
                    Total = g.Sum(i => i.MontoTotalAPagar.Value)
                });

            Console.WriteLine("\nTotal cobrado por parqueo:");
            foreach (var item in totalPorParqueo)
            {
                Console.WriteLine($"{item.Parqueo}: {item.Total:C2}");
            }
        }
    }
}
