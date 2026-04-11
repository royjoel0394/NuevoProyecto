using Xunit;
using Ejercicio2.Repositorio.Repositories;
using Ejercicio2.Repositorio.Repositories.Json;
using Ejercicio2.Repositorio.Configuration;
using ParqueoDatabaseExample.Models;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;

namespace Ejercicio2.Repositorio.Tests
{
    public class JsonIngresoAutomovilRepositorySimplifiedTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly IJsonFileProvider _fileProvider;
        private readonly JsonIngresoAutomovilRepository _repository;

        public JsonIngresoAutomovilRepositorySimplifiedTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);

            CreateTestJsonFiles();

            var options = new RepositoryOptions
            {
                JsonAutomovilesFile = "automoviles.json",
                JsonParqueoFile = "parqueos.json",
                JsonIngresosFile = "ingresos.json"
            };

            _fileProvider = new JsonFileProvider(_testDirectory, options);
            _repository = new JsonIngresoAutomovilRepository(_fileProvider);
        }

        private void CreateTestJsonFiles()
        {
            var automoviles = new[]
            {
                new { id = 1, color = "Rojo", año = 2020, fabricante = "Toyota", tipo = "Sedan" }
            };

            var parqueos = new[]
            {
                new { id = 1, nombre_de_provincia = "San José", nombre = "Centro", precio_por_hora = 5.50m }
            };

            var ingresos = new[] 
            {
                new { consecutivo = 1, id_parqueo = 1, id_automovil = 1, fecha_hora_entrada = "2025-01-15T08:00:00", fecha_hora_salida = "2025-01-15T10:00:00" }
            };

            var jsonOptions = new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true };

            File.WriteAllText(
                Path.Combine(_testDirectory, "automoviles.json"),
                JsonSerializer.Serialize(automoviles, jsonOptions)
            );

            File.WriteAllText(
                Path.Combine(_testDirectory, "parqueos.json"),
                JsonSerializer.Serialize(parqueos, jsonOptions)
            );

            File.WriteAllText(
                Path.Combine(_testDirectory, "ingresos.json"),
                JsonSerializer.Serialize(ingresos, jsonOptions)
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllIngresos()
        {
            var result = await _repository.GetAllAsync();

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task InsertAsync_ShouldAddNewIngreso()
        {
            var newIngreso = new PrqIngresoAutomovile
            {
                IdAutomovil = 1,
                IdParqueo = 1,
                FechaHoraEntrada = new DateTime(2025, 1, 16, 09, 0, 0),
                FechaHoraSalida = new DateTime(2025, 1, 16, 11, 30, 0)
            };

            await _repository.InsertAsync(newIngreso);

            var result = await _repository.GetAllAsync();
            Assert.Equal(2, result.Count);

            var inserted = await _repository.GetByIdAsync(2);
            Assert.NotNull(inserted);
            Assert.Equal(2, inserted.Consecutivo);
            Assert.Equal(1, inserted.IdAutomovil);
        }

        [Fact]
        public async Task InsertAsync_ShouldThrowIfAutomovilNotExists()
        {
            var newIngreso = new PrqIngresoAutomovile
            {
                IdAutomovil = 999,
                IdParqueo = 1,
                FechaHoraEntrada = new DateTime(2025, 1, 16, 09, 0, 0),
                FechaHoraSalida = new DateTime(2025, 1, 16, 11, 30, 0)
            };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _repository.InsertAsync(newIngreso)
            );

            Assert.Contains("automóvil", ex.Message, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task InsertAsync_ShouldThrowIfParqueoNotExists()
        {
            var newIngreso = new PrqIngresoAutomovile
            {
                IdAutomovil = 1,
                IdParqueo = 999,
                FechaHoraEntrada = new DateTime(2025, 1, 16, 09, 0, 0),
                FechaHoraSalida = new DateTime(2025, 1, 16, 11, 30, 0)
            };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _repository.InsertAsync(newIngreso)
            );

            Assert.Contains("parqueo", ex.Message, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrueForExistingId()
        {
            var result = await _repository.ExistsAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalseForMissingId()
        {
            var result = await _repository.ExistsAsync(999);

            Assert.False(result);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}
