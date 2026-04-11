namespace Ejercicio2.Repositorio.Dtos
{
    public sealed class IngresoPorProvinciaDto
    {
        public int IdIngreso { get; init; }
        public int IdAutomovil { get; init; }
        public string Tipo { get; init; } = string.Empty;
        public DateTime HoraIngreso { get; init; }
        public DateTime HoraSalida { get; init; }
        public decimal PrecioPorHora { get; init; }
        public decimal MontoAPagar { get; init; }
        public double DuracionHoras { get; init; }
        public string NombreParqueo { get; init; } = string.Empty;
        public string Provincia { get; init; } = string.Empty;
    }
}
