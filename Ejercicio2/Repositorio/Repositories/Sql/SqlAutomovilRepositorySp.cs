using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ejercicio2.Repositorio.Repositories;
using Microsoft.EntityFrameworkCore;
using ParqueoDatabaseExample.Models;

namespace Ejercicio2.Repositorio.Repositories.Sql
{
    public sealed class SqlAutomovilRepositorySp : IAutomovilRepository
    {
        private readonly ParqueoDbContext _context;

        public SqlAutomovilRepositorySp(ParqueoDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<PrqAutomovile>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var result = await _context.PrqAutomoviles
                .FromSqlRaw("EXEC sp_Automovil_GetAll")
                .ToListAsync(cancellationToken);
            return result;
        }

        public async Task<PrqAutomovile?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var result = await _context.PrqAutomoviles
                .FromSqlRaw("EXEC sp_Automovil_GetById @id = {0}", id)
                .ToListAsync(cancellationToken);
            return result.FirstOrDefault();
        }

        public async Task<IReadOnlyList<PrqAutomovile>> GetByColorAsync(string color, CancellationToken cancellationToken = default)
            => await _context.PrqAutomoviles
                .Where(x => x.Color.ToLower().Contains(color.ToLower()))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<PrqAutomovile>> GetByYearRangeAsync(int yearFrom, int yearTo, CancellationToken cancellationToken = default)
            => await _context.PrqAutomoviles
                .Where(x => x.Ano >= yearFrom && x.Ano <= yearTo)
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
            var result = _context.Database
                .ExecuteSqlRaw("EXEC sp_Automovil_Insert @color = {0}, @año = {1}, @fabricante = {2}, @tipo = {3}",
                    entity.Color, entity.Ano, entity.Fabricante, entity.Tipo);
            
            // Obtener el ID insertado
            var idResult = _context.PrqAutomoviles.OrderByDescending(x => x.Id).FirstOrDefault();
            if (idResult != null) entity.Id = idResult.Id;
        }

        public async Task InsertAsync(PrqAutomovile entity, CancellationToken cancellationToken = default)
        {
            var idParam = new Microsoft.Data.SqlClient.SqlParameter("@id", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output };
            
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_Automovil_Insert @color = {0}, @año = {1}, @fabricante = {2}, @tipo = {3}",
                entity.Color, entity.Ano, entity.Fabricante, entity.Tipo,
                cancellationToken);
        }

        public void Update(PrqAutomovile entity)
        {
            try
            {
                _context.Database
                    .ExecuteSqlRaw("EXEC sp_Automovil_Update @id = {0}, @color = {1}, @año = {2}, @fabricante = {3}, @tipo = {4}",
                        entity.Id, entity.Color, entity.Ano, entity.Fabricante, entity.Tipo);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error al actualizar el automóvil.", ex);
            }
        }

        public async Task UpdateAsync(PrqAutomovile entity, CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Automovil_Update @id = {0}, @color = {1}, @año = {2}, @fabricante = {3}, @tipo = {4}",
                    entity.Id, entity.Color, entity.Ano, entity.Fabricante, entity.Tipo,
                    cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error al actualizar el automóvil.", ex);
            }
        }

        public void Delete(int id)
        {
            try
            {
                _context.Database
                    .ExecuteSqlRaw("EXEC sp_Automovil_Delete @id = {0}", id);
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.Contains("ingresos asociados"))
                    throw new InvalidOperationException("El automóvil tiene ingresos asociados.");
                throw;
            }
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Automovil_Delete @id = {0}", id, cancellationToken);
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.Contains("ingresos asociados"))
                    throw new InvalidOperationException("El automóvil tiene ingresos asociados.");
                throw;
            }
        }

        public bool Exists(int id)
            => _context.PrqAutomoviles.Any(x => x.Id == id);

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
            => _context.PrqAutomoviles.AnyAsync(x => x.Id == id, cancellationToken);
    }
}