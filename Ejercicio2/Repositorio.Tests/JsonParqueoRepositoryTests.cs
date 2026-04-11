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
    public class JsonParqueoRepositorySimplifiedTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly IJsonFileProvider _fileProvider;
        private readonly JsonParqueoRepository _repository;

        public JsonParqueoRepositorySimplifiedTests()
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
            _repository = new JsonParqueoRepository(_fileProvider);
        }

        private void CreateTestJsonFiles()
        {
            var automoviles = new object[] { };
            var parqueos = new[]
            {
                new { id = 1, nombre_de_provincia = "San José", nombre = "Centro", precio_por_hora = 5.50m },
                new { id = 2, nombre_de_provincia = "Alajuela", nombre = "Mall", precio_por_hora = 3.00m }
            };
            var ingresos = new object[] { };
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
        public async Task GetAllAsync_ShouldReturnAllParqueos()
        {
            var result = await _repository.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectParqueo()
        {
            var result = await _repository.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Centro", result.Nombre);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNullForInvalidId()
        {
            var result = await _repository.GetByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task InsertAsync_ShouldAddNewParqueo()
        {
            var newParqueo = new PrqParqueo
            {
                NombreDeProvincia = "Cartago",
                Nombre = "Centro Cartago",
                PrecioPorHora = 4.00m
            };

            await _repository.InsertAsync(newParqueo);

            var result = await _repository.GetAllAsync();
            Assert.Equal(3, result.Count);

            var inserted = await _repository.GetByIdAsync(3);
            Assert.NotNull(inserted);
            Assert.Equal("Cartago", inserted.NombreDeProvincia);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingParqueo()
        {
            var parqueo = new PrqParqueo
            {
                Id = 1,
                NombreDeProvincia = "San José",
                Nombre = "CentroModificado",
                PrecioPorHora = 6.00m
            };

            await _repository.UpdateAsync(parqueo);

            var result = await _repository.GetByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal("CentroModificado", result.Nombre);
            Assert.Equal(6.00m, result.PrecioPorHora);
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
