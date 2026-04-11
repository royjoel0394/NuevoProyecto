using System.IO;
using Ejercicio2.Repositorio.Configuration;

namespace Ejercicio2.Repositorio.Repositories.Json
{
    public sealed class JsonFileProvider : IJsonFileProvider
    {
        public JsonFileProvider(string basePath, RepositoryOptions options)
        {
            AutomovilesPath = Path.GetFullPath(Path.Combine(basePath, options.JsonAutomovilesFile));
            ParqueoPath = Path.GetFullPath(Path.Combine(basePath, options.JsonParqueoFile));
            IngresosPath = Path.GetFullPath(Path.Combine(basePath, options.JsonIngresosFile));
        }

        public string AutomovilesPath { get; }
        public string ParqueoPath { get; }
        public string IngresosPath { get; }
    }
}
