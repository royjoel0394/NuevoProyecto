using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ejercicio2.Repositorio.Repositories.Json;
using ParqueoDatabaseExample.Models;

namespace Ejercicio2.Repositorio.Repositories.Json
{
    public sealed class JsonAutomovilRepository : JsonRepositoryBase<PrqAutomovile, int>, IAutomovilRepository
    {
        private readonly List<PrqIngresoAutomovile> _ingresos;

        public JsonAutomovilRepository(IJsonFileProvider fileProvider)
            : base(fileProvider.AutomovilesPath)
        {
            _ingresos = LoadFromFile<PrqIngresoAutomovile>(fileProvider.IngresosPath);
        }

        public override Task<PrqAutomovile?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));

        public Task<IReadOnlyList<PrqAutomovile>> GetByColorAsync(string color, CancellationToken cancellationToken = default)
            => Task.FromResult(Items.Where(x => x.Color.Contains(color, StringComparison.OrdinalIgnoreCase)).ToList() as IReadOnlyList<PrqAutomovile>);

        public Task<IReadOnlyList<PrqAutomovile>> GetByYearRangeAsync(int yearFrom, int yearTo, CancellationToken cancellationToken = default)
            => Task.FromResult(Items.Where(x => x.Ano >= yearFrom && x.Ano <= yearTo).ToList() as IReadOnlyList<PrqAutomovile>);

        public Task<IReadOnlyList<PrqAutomovile>> GetByManufacturerAsync(string manufacturer, CancellationToken cancellationToken = default)
            => Task.FromResult(Items.Where(x => x.Fabricante.Contains(manufacturer, StringComparison.OrdinalIgnoreCase)).ToList() as IReadOnlyList<PrqAutomovile>);

        public Task<IReadOnlyList<PrqAutomovile>> GetByTypeAsync(string type, CancellationToken cancellationToken = default)
            => Task.FromResult(Items.Where(x => x.Tipo.Contains(type, StringComparison.OrdinalIgnoreCase)).ToList() as IReadOnlyList<PrqAutomovile>);

        public override void Insert(PrqAutomovile entity)
        {
            entity.Id = Items.Any() ? Items.Max(x => x.Id) + 1 : 1;
            Items.Add(entity);
            SaveChanges();
        }

        public override Task InsertAsync(PrqAutomovile entity, CancellationToken cancellationToken = default)
        {
            entity.Id = Items.Any() ? Items.Max(x => x.Id) + 1 : 1;
            Items.Add(entity);
            return SaveChangesAsync(cancellationToken);
        }

        public override void Update(PrqAutomovile entity)
        {
            var index = Items.FindIndex(x => x.Id == entity.Id);
            if (index < 0)
            {
                throw new InvalidOperationException($"El automóvil con id {entity.Id} no existe.");
            }

            Items[index] = entity;
            SaveChanges();
        }

        public override Task UpdateAsync(PrqAutomovile entity, CancellationToken cancellationToken = default)
        {
            var index = Items.FindIndex(x => x.Id == entity.Id);
            if (index < 0)
            {
                throw new InvalidOperationException($"El automóvil con id {entity.Id} no existe.");
            }

            Items[index] = entity;
            return SaveChangesAsync(cancellationToken);
        }

        public override void Delete(int id)
        {
            var entity = Items.FirstOrDefault(x => x.Id == id);
            if (entity == null)
            {
                throw new InvalidOperationException($"El automóvil con id {id} no existe.");
            }

            if (_ingresos.Any(i => i.IdAutomovil == id))
            {
                throw new InvalidOperationException("No se puede eliminar el automóvil porque tiene ingresos asociados.");
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
