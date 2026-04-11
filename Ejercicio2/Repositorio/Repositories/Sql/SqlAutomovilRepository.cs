using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ejercicio2.Repositorio.Repositories;
using Microsoft.EntityFrameworkCore;
using ParqueoDatabaseExample.Models;

namespace Ejercicio2.Repositorio.Repositories.Sql
{
    public sealed class SqlAutomovilRepository : IAutomovilRepository
    {
        private readonly ParqueoDbContext _context;

        public SqlAutomovilRepository(ParqueoDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<PrqAutomovile>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _context.PrqAutomoviles.AsNoTracking().ToListAsync(cancellationToken);

        public async Task<PrqAutomovile?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => await _context.PrqAutomoviles.FindAsync(new object[] { id }, cancellationToken);

        public async Task<IReadOnlyList<PrqAutomovile>> GetByColorAsync(string color, CancellationToken cancellationToken = default)
            => await _context.PrqAutomoviles
                .Where(x => x.Color.ToLower().Contains(color.ToLower()))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<PrqAutomovile>> GetByYearRangeAsync(int yearFrom, int yearTo, CancellationToken cancellationToken = default)
            => await _context.PrqAutomoviles
                .Where(x => x.Año >= yearFrom && x.Año <= yearTo)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<PrqAutomovile>> GetByManufacturerAsync(string manufacturer, CancellationToken cancellationToken = default)
            => await _context.PrqAutomoviles
                .Where(x => x.Fabricante.ToLower().Contains(manufacturer.ToLower()))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<PrqAutomovile>> GetByTypeAsync(string type, CancellationToken cancellationToken = default)
            => await _context.PrqAutomoviles
                .Where(x => x.Tipo.ToLower().Contains(type.ToLower()))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        public void Insert(PrqAutomovile entity)
        {
            _context.PrqAutomoviles.Add(entity);
            _context.SaveChanges();
        }

        public async Task InsertAsync(PrqAutomovile entity, CancellationToken cancellationToken = default)
        {
            await _context.PrqAutomoviles.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public void Update(PrqAutomovile entity)
        {
            if (!Exists(entity.Id))
            {
                throw new InvalidOperationException($"El automóvil con id {entity.Id} no existe.");
            }

            _context.PrqAutomoviles.Update(entity);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error al actualizar el automóvil.", ex);
            }
        }

        public async Task UpdateAsync(PrqAutomovile entity, CancellationToken cancellationToken = default)
        {
            if (!await ExistsAsync(entity.Id, cancellationToken))
            {
                throw new InvalidOperationException($"El automóvil con id {entity.Id} no existe.");
            }

            _context.PrqAutomoviles.Update(entity);
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error al actualizar el automóvil.", ex);
            }
        }

        public void Delete(int id)
        {
            var hasIngresos = _context.PrqIngresoAutomoviles.Any(i => i.IdAutomovil == id);
            if (hasIngresos)
            {
                throw new InvalidOperationException("No se puede eliminar el automóvil porque tiene ingresos asociados.");
            }

            var entity = _context.PrqAutomoviles.Find(id);
            if (entity == null)
            {
                throw new InvalidOperationException($"El automóvil con id {id} no existe.");
            }

            _context.PrqAutomoviles.Remove(entity);
            _context.SaveChanges();
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var hasIngresos = await _context.PrqIngresoAutomoviles.AnyAsync(i => i.IdAutomovil == id, cancellationToken);
            if (hasIngresos)
            {
                throw new InvalidOperationException("No se puede eliminar el automóvil porque tiene ingresos asociados.");
            }

            var entity = await _context.PrqAutomoviles.FindAsync(new object[] { id }, cancellationToken);
            if (entity == null)
            {
                throw new InvalidOperationException($"El automóvil con id {id} no existe.");
            }

            _context.PrqAutomoviles.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public bool Exists(int id)
            => _context.PrqAutomoviles.Any(x => x.Id == id);

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
            => _context.PrqAutomoviles.AnyAsync(x => x.Id == id, cancellationToken);
    }
}
