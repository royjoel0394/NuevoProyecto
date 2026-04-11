using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ejercicio2.Repositorio.Repositories.Json;
using ParqueoDatabaseExample.Models;

namespace Ejercicio2.Repositorio.Repositories.Json
{
    public sealed class JsonParqueoRepository : JsonRepositoryBase<PrqParqueo, int>, IParqueoRepository
    {
        private readonly List<PrqIngresoAutomovile> _ingresos;

        public JsonParqueoRepository(IJsonFileProvider fileProvider)
            : base(fileProvider.ParqueoPath)
        {
            _ingresos = LoadFromFile<PrqIngresoAutomovile>(fileProvider.IngresosPath);
        }

        public override Task<PrqParqueo?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));

        public Task<IReadOnlyList<PrqParqueo>> GetByNombreAsync(string nombre, CancellationToken cancellationToken = default)
            => Task.FromResult(Items.Where(x => x.Nombre?.Contains(nombre, StringComparison.OrdinalIgnoreCase) ?? false).ToList() as IReadOnlyList<PrqParqueo>);

        public Task<IReadOnlyList<PrqParqueo>> GetByPrecioHoraRangeAsync(decimal minPrecio, decimal maxPrecio, CancellationToken cancellationToken = default)
            => Task.FromResult(Items.Where(x => x.PrecioPorHora >= minPrecio && x.PrecioPorHora <= maxPrecio).ToList() as IReadOnlyList<PrqParqueo>);

        public Task<IReadOnlyList<PrqParqueo>> GetByProvinciaAsync(string provincia, CancellationToken cancellationToken = default)
            => Task.FromResult(Items.Where(x => x.NombreDeProvincia?.Contains(provincia, StringComparison.OrdinalIgnoreCase) ?? false).ToList() as IReadOnlyList<PrqParqueo>);

        public override void Insert(PrqParqueo entity)
        {
            entity.Id = Items.Any() ? Items.Max(x => x.Id) + 1 : 1;
            Items.Add(entity);
            SaveChanges();
        }

        public override Task InsertAsync(PrqParqueo entity, CancellationToken cancellationToken = default)
        {
            entity.Id = Items.Any() ? Items.Max(x => x.Id) + 1 : 1;
            Items.Add(entity);
            return SaveChangesAsync(cancellationToken);
        }

        public override void Update(PrqParqueo entity)
        {
            var index = Items.FindIndex(x => x.Id == entity.Id);
            if (index < 0)
            {
                throw new InvalidOperationException($"El parqueo con id {entity.Id} no existe.");
            }

            Items[index] = entity;
            SaveChanges();
        }

        public override Task UpdateAsync(PrqParqueo entity, CancellationToken cancellationToken = default)
        {
            var index = Items.FindIndex(x => x.Id == entity.Id);
            if (index < 0)
            {
                throw new InvalidOperationException($"El parqueo con id {entity.Id} no existe.");
            }

            Items[index] = entity;
            return SaveChangesAsync(cancellationToken);
        }

        public override void Delete(int id)
        {
            var entity = Items.FirstOrDefault(x => x.Id == id);
            if (entity == null)
            {
                throw new InvalidOperationException($"El parqueo con id {id} no existe.");
            }

            if (_ingresos.Any(i => i.IdParqueo == id))
            {
                throw new InvalidOperationException("No se puede eliminar el parqueo porque tiene ingresos asociados.");
            }

            Items.Remove(entity);
            SaveChanges();
        }

        public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            Delete(id);
            return Task.CompletedTask;
        }

        public override bool Exists(int id)
            => Items.Any(x => x.Id == id);

        public override Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(Exists(id));
    }
}
