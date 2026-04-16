using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ejercicio2.Repositorio.Dtos;
using Ejercicio2.Repositorio.Repositories;
using Microsoft.EntityFrameworkCore;
using ParqueoDatabaseExample.Models;

namespace Ejercicio2.Repositorio.Repositories.Sql
{
    public sealed class SqlIngresoAutomovilRepositorySp : IIngresoAutomovilRepository
    {
        private readonly ParqueoDbContext _context;

        public SqlIngresoAutomovilRepositorySp(ParqueoDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<PrqIngresoAutomovile>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _context.PrqIngresoAutomoviles
                .FromSqlRaw("EXEC sp_Ingreso_GetAll")
                .ToListAsync(cancellationToken);

        public async Task<PrqIngresoAutomovile?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var result = await _context.PrqIngresoAutomoviles
                .FromSqlRaw("EXEC sp_Ingreso_GetById @consecutivo = {0}", id)
                .ToListAsync(cancellationToken);
            return result.FirstOrDefault();
        }

        public async Task<decimal?> GetPrecioPorHoraAsync(int parqueoId, CancellationToken cancellationToken = default)
            => await _context.PrqParqueos
                .Where(p => p.Id == parqueoId)
                .Select(p => (decimal?)p.PrecioPorHora)
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<IReadOnlyList<IngresoAutomovilDetalleDto>> GetIngresosPorTipoAutomovilAsync(
            string tipoAutomovil,
            DateTime desde,
            DateTime hasta,
            CancellationToken cancellationToken = default)
        {
            return await _context.PrqIngresoAutomoviles
                .Where(i => i.FechaHoraSalida.HasValue
                            && i.FechaHoraEntrada >= desde
                            && i.FechaHoraSalida <= hasta
                            && i.IdAutomovilNavigation.Tipo.ToLower().Contains(tipoAutomovil.ToLower()))
                .Select(i => new IngresoAutomovilDetalleDto
                {
                    IdIngreso = i.Consecutivo,
                    IdAutomovil = i.IdAutomovil,
                    Color = i.IdAutomovilNavigation.Color,
                    Tipo = i.IdAutomovilNavigation.Tipo,
                    Fabricante = i.IdAutomovilNavigation.Fabricante,
                    HoraIngreso = i.FechaHoraEntrada,
                    HoraSalida = i.FechaHoraSalida!.Value,
                    PrecioPorHora = i.IdParqueoNavigation.PrecioPorHora,
                    DuracionHoras = EF.Functions.DateDiffMinute(i.FechaHoraEntrada, i.FechaHoraSalida!.Value) / 60.0,
                    MontoAPagar = i.IdParqueoNavigation.PrecioPorHora * (EF.Functions.DateDiffMinute(i.FechaHoraEntrada, i.FechaHoraSalida!.Value) / 60.0m),
                    NombreParqueo = i.IdParqueoNavigation.Nombre,
                    Provincia = i.IdParqueoNavigation.NombreDeProvincia
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<IngresoPorProvinciaDto>> GetIngresosPorProvinciaAsync(
            string provincia,
            DateTime desde,
            DateTime hasta,
            CancellationToken cancellationToken = default)
        {
            return await _context.PrqIngresoAutomoviles
                .Where(i => i.FechaHoraSalida.HasValue
                            && i.FechaHoraEntrada >= desde
                            && i.FechaHoraSalida <= hasta
                            && i.IdParqueoNavigation.NombreDeProvincia.ToLower().Contains(provincia.ToLower()))
                .Select(i => new IngresoPorProvinciaDto
                {
                    IdIngreso = i.Consecutivo,
                    IdAutomovil = i.IdAutomovil,
                    Tipo = i.IdAutomovilNavigation.Tipo,
                    HoraIngreso = i.FechaHoraEntrada,
                    HoraSalida = i.FechaHoraSalida!.Value,
                    PrecioPorHora = i.IdParqueoNavigation.PrecioPorHora,
                    DuracionHoras = EF.Functions.DateDiffMinute(i.FechaHoraEntrada, i.FechaHoraSalida!.Value) / 60.0,
                    MontoAPagar = i.IdParqueoNavigation.PrecioPorHora * (EF.Functions.DateDiffMinute(i.FechaHoraEntrada, i.FechaHoraSalida!.Value) / 60.0m),
                    NombreParqueo = i.IdParqueoNavigation.Nombre,
                    Provincia = i.IdParqueoNavigation.NombreDeProvincia
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public void Insert(PrqIngresoAutomovile entity)
        {
            var fechaEntrada = entity.FechaHoraEntrada == default ? DateTime.Now : entity.FechaHoraEntrada;
            
            _context.Database
                .ExecuteSqlRaw("EXEC sp_Ingreso_Insert @id_parqueo = {0}, @id_automovil = {1}, @fecha_hora_entrada = {2}",
                    entity.IdParqueo, entity.IdAutomovil, fechaEntrada);
        }

        public async Task InsertAsync(PrqIngresoAutomovile entity, CancellationToken cancellationToken = default)
        {
            var fechaEntrada = entity.FechaHoraEntrada == default ? DateTime.Now : entity.FechaHoraEntrada;
            
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_Ingreso_Insert @id_parqueo = {0}, @id_automovil = {1}, @fecha_hora_entrada = {2}",
                entity.IdParqueo, entity.IdAutomovil, fechaEntrada,
                cancellationToken);
        }

        public void Update(PrqIngresoAutomovile entity)
        {
            if (entity.FechaHoraSalida.HasValue)
            {
                _context.Database
                    .ExecuteSqlRaw("EXEC sp_Ingreso_UpdateSalida @consecutivo = {0}, @fecha_hora_salida = {1}",
                        entity.Consecutivo, entity.FechaHoraSalida);
            }
        }

        public async Task UpdateAsync(PrqIngresoAutomovile entity, CancellationToken cancellationToken = default)
        {
            if (entity.FechaHoraSalida.HasValue)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Ingreso_UpdateSalida @consecutivo = {0}, @fecha_hora_salida = {1}",
                    entity.Consecutivo, entity.FechaHoraSalida,
                    cancellationToken);
            }
        }

        public void Delete(int id)
        {
            _context.Database
                .ExecuteSqlRaw("EXEC sp_Ingreso_Delete @consecutivo = {0}", id);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_Ingreso_Delete @consecutivo = {0}", id, cancellationToken);
        }

        public bool Exists(int id)
            => _context.PrqIngresoAutomoviles.Any(x => x.Consecutivo == id);

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
            => _context.PrqIngresoAutomoviles.AnyAsync(x => x.Consecutivo == id, cancellationToken);
    }
}