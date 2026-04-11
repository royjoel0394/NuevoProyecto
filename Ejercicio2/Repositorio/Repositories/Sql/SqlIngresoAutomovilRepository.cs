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
    public sealed class SqlIngresoAutomovilRepository : IIngresoAutomovilRepository
    {
        private readonly ParqueoDbContext _context;

        public SqlIngresoAutomovilRepository(ParqueoDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<PrqIngresoAutomovile>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _context.PrqIngresoAutomoviles.AsNoTracking().ToListAsync(cancellationToken);

        public async Task<PrqIngresoAutomovile?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => await _context.PrqIngresoAutomoviles.FindAsync(new object[] { id }, cancellationToken);

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
            entity.Consecutivo = _context.PrqIngresoAutomoviles.Any()
                ? _context.PrqIngresoAutomoviles.Max(i => i.Consecutivo) + 1
                : 1;

            _context.PrqIngresoAutomoviles.Add(entity);
            _context.SaveChanges();
        }

        public async Task InsertAsync(PrqIngresoAutomovile entity, CancellationToken cancellationToken = default)
        {
            entity.Consecutivo = await _context.PrqIngresoAutomoviles.AnyAsync(cancellationToken)
                ? await _context.PrqIngresoAutomoviles.MaxAsync(i => i.Consecutivo, cancellationToken) + 1
                : 1;

            await _context.PrqIngresoAutomoviles.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public void Update(PrqIngresoAutomovile entity)
        {
            if (!Exists(entity.Consecutivo))
            {
                throw new InvalidOperationException($"El ingreso con consecutivo {entity.Consecutivo} no existe.");
            }

            _context.PrqIngresoAutomoviles.Update(entity);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error al actualizar el ingreso.", ex);
            }
        }

        public async Task UpdateAsync(PrqIngresoAutomovile entity, CancellationToken cancellationToken = default)
        {
            if (!await ExistsAsync(entity.Consecutivo, cancellationToken))
            {
                throw new InvalidOperationException($"El ingreso con consecutivo {entity.Consecutivo} no existe.");
            }

            _context.PrqIngresoAutomoviles.Update(entity);
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error al actualizar el ingreso.", ex);
            }
        }

        public void Delete(int id)
        {
            var entity = _context.PrqIngresoAutomoviles.Find(id);
            if (entity == null)
            {
                throw new InvalidOperationException($"El ingreso con consecutivo {id} no existe.");
            }

            _context.PrqIngresoAutomoviles.Remove(entity);
            _context.SaveChanges();
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.PrqIngresoAutomoviles.FindAsync(new object[] { id }, cancellationToken);
            if (entity == null)
            {
                throw new InvalidOperationException($"El ingreso con consecutivo {id} no existe.");
            }

            _context.PrqIngresoAutomoviles.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public bool Exists(int id)
            => _context.PrqIngresoAutomoviles.Any(x => x.Consecutivo == id);

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
            => _context.PrqIngresoAutomoviles.AnyAsync(x => x.Consecutivo == id, cancellationToken);
    }
}
