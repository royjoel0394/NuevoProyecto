using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ejercicio2.Repositorio.Dtos;
using Ejercicio2.Repositorio.Repositories.Json;
using ParqueoDatabaseExample.Models;

namespace Ejercicio2.Repositorio.Repositories.Json
{
    public sealed class JsonIngresoAutomovilRepository : JsonRepositoryBase<PrqIngresoAutomovile, int>, IIngresoAutomovilRepository
    {
        private readonly List<PrqAutomovile> _automoviles;
        private readonly List<PrqParqueo> _parqueos;

        public JsonIngresoAutomovilRepository(IJsonFileProvider fileProvider)
            : base(fileProvider.IngresosPath)
        {
            _automoviles = LoadJsonFile<PrqAutomovile>(fileProvider.AutomovilesPath);
            _parqueos = LoadJsonFile<PrqParqueo>(fileProvider.ParqueoPath);
        }

        private static List<T> LoadJsonFile<T>(string path)
        {
            using var stream = File.OpenRead(path);
            return JsonSerializer.Deserialize<List<T>>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<T>();
        }

        public override Task<PrqIngresoAutomovile?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(Items.FirstOrDefault(x => x.Consecutivo == id));

        public Task<decimal?> GetPrecioPorHoraAsync(int parqueoId, CancellationToken cancellationToken = default)
        {
            var parqueo = _parqueos.FirstOrDefault(x => x.Id == parqueoId);
            return Task.FromResult(parqueo?.PrecioPorHora);
        }

        public Task<IReadOnlyList<IngresoAutomovilDetalleDto>> GetIngresosPorTipoAutomovilAsync(
            string tipoAutomovil,
            DateTime desde,
            DateTime hasta,
            CancellationToken cancellationToken = default)
        {
            var query = from ingreso in Items
                        join automovil in _automoviles on ingreso.IdAutomovil equals automovil.Id
                        join parqueo in _parqueos on ingreso.IdParqueo equals parqueo.Id
                        where ingreso.FechaHoraSalida.HasValue
                              && automovil.Tipo.Contains(tipoAutomovil, StringComparison.OrdinalIgnoreCase)
                              && ingreso.FechaHoraEntrada >= desde
                              && ingreso.FechaHoraSalida <= hasta
                        select MapDetalle(ingreso, automovil, parqueo);

            return Task.FromResult(query.ToList() as IReadOnlyList<IngresoAutomovilDetalleDto>);
        }

        public Task<IReadOnlyList<IngresoPorProvinciaDto>> GetIngresosPorProvinciaAsync(
            string provincia,
            DateTime desde,
            DateTime hasta,
            CancellationToken cancellationToken = default)
        {
            var query = from ingreso in Items
                        join automovil in _automoviles on ingreso.IdAutomovil equals automovil.Id
                        join parqueo in _parqueos on ingreso.IdParqueo equals parqueo.Id
                        where ingreso.FechaHoraSalida.HasValue
                              && parqueo.NombreDeProvincia.Contains(provincia, StringComparison.OrdinalIgnoreCase)
                              && ingreso.FechaHoraEntrada >= desde
                              && ingreso.FechaHoraSalida <= hasta
                        select MapProvincia(ingreso, automovil, parqueo);

            return Task.FromResult(query.ToList() as IReadOnlyList<IngresoPorProvinciaDto>);
        }

        public override void Insert(PrqIngresoAutomovile entity)
        {
            entity.Consecutivo = Items.Any() ? Items.Max(x => x.Consecutivo) + 1 : 1;

            if (!_automoviles.Any(x => x.Id == entity.IdAutomovil))
            {
                throw new InvalidOperationException($"El automóvil con id {entity.IdAutomovil} no existe.");
            }

            if (!_parqueos.Any(x => x.Id == entity.IdParqueo))
            {
                throw new InvalidOperationException($"El parqueo con id {entity.IdParqueo} no existe.");
            }

            Items.Add(entity);
            SaveChanges();
        }

        public override Task InsertAsync(PrqIngresoAutomovile entity, CancellationToken cancellationToken = default)
        {
            entity.Consecutivo = Items.Any() ? Items.Max(x => x.Consecutivo) + 1 : 1;

            if (!_automoviles.Any(x => x.Id == entity.IdAutomovil))
            {
                throw new InvalidOperationException($"El automóvil con id {entity.IdAutomovil} no existe.");
            }

            if (!_parqueos.Any(x => x.Id == entity.IdParqueo))
            {
                throw new InvalidOperationException($"El parqueo con id {entity.IdParqueo} no existe.");
            }

            Items.Add(entity);
            return SaveChangesAsync(cancellationToken);
        }

        public override void Update(PrqIngresoAutomovile entity)
        {
            var index = Items.FindIndex(x => x.Consecutivo == entity.Consecutivo);
            if (index < 0)
            {
                throw new InvalidOperationException($"El ingreso con consecutivo {entity.Consecutivo} no existe.");
            }

            Items[index] = entity;
            SaveChanges();
        }

        public override Task UpdateAsync(PrqIngresoAutomovile entity, CancellationToken cancellationToken = default)
        {
            var index = Items.FindIndex(x => x.Consecutivo == entity.Consecutivo);
            if (index < 0)
            {
                throw new InvalidOperationException($"El ingreso con consecutivo {entity.Consecutivo} no existe.");
            }

            Items[index] = entity;
            return SaveChangesAsync(cancellationToken);
        }

        public override void Delete(int id)
        {
            var entity = Items.FirstOrDefault(x => x.Consecutivo == id);
            if (entity == null)
            {
                throw new InvalidOperationException($"El ingreso con consecutivo {id} no existe.");
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
            => Items.Any(x => x.Consecutivo == id);

        public override Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(Exists(id));

        private static IngresoAutomovilDetalleDto MapDetalle(PrqIngresoAutomovile ingreso, PrqAutomovile automovil, PrqParqueo parqueo)
        {
            var horas = Math.Round((decimal)(ingreso.FechaHoraSalida!.Value - ingreso.FechaHoraEntrada).TotalMinutes / 60m, 2);
            return new IngresoAutomovilDetalleDto
            {
                IdIngreso = ingreso.Consecutivo,
                IdAutomovil = automovil.Id,
                Color = automovil.Color,
                Tipo = automovil.Tipo,
                Fabricante = automovil.Fabricante,
                HoraIngreso = ingreso.FechaHoraEntrada,
                HoraSalida = ingreso.FechaHoraSalida.Value,
                PrecioPorHora = parqueo.PrecioPorHora,
                DuracionHoras = (double)horas,
                MontoAPagar = Math.Round(parqueo.PrecioPorHora * horas, 2),
                NombreParqueo = parqueo.Nombre,
                Provincia = parqueo.NombreDeProvincia
            };
        }

        private static IngresoPorProvinciaDto MapProvincia(PrqIngresoAutomovile ingreso, PrqAutomovile automovil, PrqParqueo parqueo)
        {
            var horas = Math.Round((decimal)(ingreso.FechaHoraSalida!.Value - ingreso.FechaHoraEntrada).TotalMinutes / 60m, 2);
            return new IngresoPorProvinciaDto
            {
                IdIngreso = ingreso.Consecutivo,
                IdAutomovil = automovil.Id,
                Tipo = automovil.Tipo,
                HoraIngreso = ingreso.FechaHoraEntrada,
                HoraSalida = ingreso.FechaHoraSalida.Value,
                PrecioPorHora = parqueo.PrecioPorHora,
                DuracionHoras = (double)horas,
                MontoAPagar = Math.Round(parqueo.PrecioPorHora * horas, 2),
                NombreParqueo = parqueo.Nombre,
                Provincia = parqueo.NombreDeProvincia
            };
        }
    }
}
