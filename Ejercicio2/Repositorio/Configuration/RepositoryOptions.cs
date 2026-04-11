namespace Ejercicio2.Repositorio.Configuration
{
    public sealed class RepositoryOptions
    {
        public string Provider { get; init; } = "Json";
        public string JsonFilesPath { get; init; } = string.Empty;
        public string JsonAutomovilesFile { get; init; } = string.Empty;
        public string JsonParqueoFile { get; init; } = string.Empty;
        public string JsonIngresosFile { get; init; } = string.Empty;
    }
}
