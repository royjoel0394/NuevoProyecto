using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ejercicio2.Repositorio.Repositories;
using Microsoft.EntityFrameworkCore;
using ParqueoDatabaseExample.Models;

namespace Ejercicio2.Repositorio.Repositories.Sql
{
    public sealed class SqlParqueoRepository : IParqueoRepository
    {
        private readonly ParqueoDbContext _context;

        public SqlParqueoRepository(ParqueoDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<PrqParqueo>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _context.PrqParqueos.AsNoTracking().ToListAsync(cancellationToken);

        public async Task<PrqParqueo?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => await _context.PrqParqueos.FindAsync(new object[] { id }, cancellationToken);

        public async Task<IReadOnlyList<PrqParqueo>> GetByNombreAsync(string nombre, CancellationToken cancellationToken = default)
            => await _context.PrqParqueos
                .Where(x => x.Nombre.ToLower().Contains(nombre.ToLower()))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<PrqParqueo>> GetByPrecioHoraRangeAsync(decimal minPrecio, decimal maxPrecio, CancellationToken cancellationToken = default)
            => await _context.PrqParqueos
                .Where(x => x.PrecioPorHora >= minPrecio && x.PrecioPorHora <= maxPrecio)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<PrqParqueo>> GetByProvinciaAsync(string provincia, CancellationToken cancellationToken = default)
            => await _context.PrqParqueos
                .Where(x => x.NombreDeProvincia.ToLower().Contains(provincia.ToLower()))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        public void Insert(PrqParqueo entity)
        {
            _context.PrqParqueos.Add(entity);
            _context.SaveChanges();
        }

        public async Task InsertAsync(PrqParqueo entity, CancellationToken cancellationToken = default)
        {
            await _context.PrqParqueos.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public void Update(PrqParqueo entity)
        {
            if (!Exists(entity.Id))
            {
                throw new InvalidOperationException($"El parqueo con id {entity.Id} no existe.");
            }

            _context.PrqParqueos.Update(entity);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error al actualizar el parqueo.", ex);
            }
        }

        public async Task UpdateAsync(PrqParqueo entity, CancellationToken cancellationToken = default)
        {
            if (!await ExistsAsync(entity.Id, cancellationToken))
            {
                throw new InvalidOperationException($"El parqueo con id {entity.Id} no existe.");
            }

            _context.PrqParqueos.Update(entity);
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error al actualizar el parqueo.", ex);
            }
        }

        public void Delete(int id)
        {
            var hasIngresos = _context.PrqIngresoAutomoviles.Any(i => i.IdParqueo == id);
            if (hasIngresos)
            {
                throw new InvalidOperationException("No se puede eliminar el parqueo porque tiene ingresos asociados.");
            }

            var entity = _context.PrqParqueos.Find(id);
            if (entity == null)
            {
                throw new InvalidOperationException($"El parqueo con id {id} no existe.");
            }

            _context.PrqParqueos.Remove(entity);
            _context.SaveChanges();
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var hasIngresos = await _context.PrqIngresoAutomoviles.AnyAsync(i => i.IdParqueo == id, cancellationToken);
            if (hasIngresos)
            {
                throw new InvalidOperationException("No se puede eliminar el parqueo porque tiene ingresos asociados.");
            }

            var entity = await _context.PrqParqueos.FindAsync(new object[] { id }, cancellationToken);
            if (entity == null)
            {
                throw new InvalidOperationException($"El parqueo con id {id} no existe.");
            }

            _context.PrqParqueos.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public bool Exists(int id)
            => _context.PrqParqueos.Any(x => x.Id == id);

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
            => _context.PrqParqueos.AnyAsync(x => x.Id == id, cancellationToken);
    }
}
