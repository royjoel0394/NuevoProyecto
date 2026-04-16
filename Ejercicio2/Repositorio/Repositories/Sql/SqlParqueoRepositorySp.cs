using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ejercicio2.Repositorio.Repositories;
using Microsoft.EntityFrameworkCore;
using ParqueoDatabaseExample.Models;

namespace Ejercicio2.Repositorio.Repositories.Sql
{
    public sealed class SqlParqueoRepositorySp : IParqueoRepository
    {
        private readonly ParqueoDbContext _context;

        public SqlParqueoRepositorySp(ParqueoDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<PrqParqueo>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _context.PrqParqueos
                .FromSqlRaw("EXEC sp_Parqueo_GetAll")
                .ToListAsync(cancellationToken);

        public async Task<PrqParqueo?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var result = await _context.PrqParqueos
                .FromSqlRaw("EXEC sp_Parqueo_GetById @id = {0}", id)
                .ToListAsync(cancellationToken);
            return result.FirstOrDefault();
        }

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
            _context.Database
                .ExecuteSqlRaw("EXEC sp_Parqueo_Insert @nombre_provincia = {0}, @nombre = {1}, @precio_por_hora = {2}",
                    entity.NombreDeProvincia, entity.Nombre, entity.PrecioPorHora);
        }

        public async Task InsertAsync(PrqParqueo entity, CancellationToken cancellationToken = default)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_Parqueo_Insert @nombre_provincia = {0}, @nombre = {1}, @precio_por_hora = {2}",
                entity.NombreDeProvincia, entity.Nombre, entity.PrecioPorHora,
                cancellationToken);
        }

        public void Update(PrqParqueo entity)
        {
            try
            {
                _context.Database
                    .ExecuteSqlRaw("EXEC sp_Parqueo_Update @id = {0}, @nombre_provincia = {1}, @nombre = {2}, @precio_por_hora = {3}",
                        entity.Id, entity.NombreDeProvincia, entity.Nombre, entity.PrecioPorHora);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error al actualizar el parqueo.", ex);
            }
        }

        public async Task UpdateAsync(PrqParqueo entity, CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Parqueo_Update @id = {0}, @nombre_provincia = {1}, @nombre = {2}, @precio_por_hora = {3}",
                    entity.Id, entity.NombreDeProvincia, entity.Nombre, entity.PrecioPorHora,
                    cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error al actualizar el parqueo.", ex);
            }
        }

        public void Delete(int id)
        {
            try
            {
                _context.Database
                    .ExecuteSqlRaw("EXEC sp_Parqueo_Delete @id = {0}", id);
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.Contains("ingresos asociados"))
                    throw new InvalidOperationException("El parqueo tiene ingresos asociados.");
                throw;
            }
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Parqueo_Delete @id = {0}", id, cancellationToken);
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.Contains("ingresos asociados"))
                    throw new InvalidOperationException("El parqueo tiene ingresos asociados.");
                throw;
            }
        }

        public bool Exists(int id)
            => _context.PrqParqueos.Any(x => x.Id == id);

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
            => _context.PrqParqueos.AnyAsync(x => x.Id == id, cancellationToken);
    }
}