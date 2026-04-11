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
    public class JsonAutomovilRepositorySimplifiedTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly IJsonFileProvider _fileProvider;
        private readonly JsonAutomovilRepository _repository;

        public JsonAutomovilRepositorySimplifiedTests()
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
            _repository = new JsonAutomovilRepository(_fileProvider);
        }

        private void CreateTestJsonFiles()
        {
            var automoviles = new[]
            {
                new { id = 1, color = "Rojo", año = 2020, fabricante = "Toyota", tipo = "Sedan" },
                new { id = 2, color = "Azul", año = 2021, fabricante = "Honda", tipo = "SUV" }
            };

            var parqueos = new object[] { };
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
        public async Task GetAllAsync_ShouldReturnAllAutomoviles()
        {
            var result = await _repository.GetAllAsync();
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectAutomovil()
        {
            var result = await _repository.GetByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Rojo", result.Color);
            Assert.Equal("Toyota", result.Fabricante);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNullForInvalidId()
        {
            var result = await _repository.GetByIdAsync(999);
            Assert.Null(result);
        }

        [Fact]
        public async Task InsertAsync_ShouldAddNewAutomovil()
        {
            var newAutomovil = new PrqAutomovile
            {
                Color = "Verde",
                Año = 2022,
                Fabricante = "Nissan",
                Tipo = "Hatchback"
            };

            await _repository.InsertAsync(newAutomovil);

            var result = await _repository.GetAllAsync();
            Assert.Equal(3, result.Count);

            var inserted = await _repository.GetByIdAsync(3);
            Assert.NotNull(inserted);
            Assert.Equal("Verde", inserted.Color);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingAutomovil()
        {
            var automovil = new PrqAutomovile
            {
                Id = 1,
                Color = "Negro",
                Año = 2020,
                Fabricante = "Toyota",
                Tipo = "Sedan"
            };

            await _repository.UpdateAsync(automovil);

            var result = await _repository.GetByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal("Negro", result.Color);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowForInvalidId()
        {
            var automovil = new PrqAutomovile
            {
                Id = 999,
                Color = "Blanco",
                Año = 2022,
                Fabricante = "BMW",
                Tipo = "Sedan"
            };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _repository.UpdateAsync(automovil)
            );

            Assert.Contains("no existe", ex.Message);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveAutomovil()
        {
            await _repository.DeleteAsync(1);

            var result = await _repository.GetAllAsync();
            Assert.Single(result);

            var deleted = await _repository.GetByIdAsync(1);
            Assert.Null(deleted);
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
