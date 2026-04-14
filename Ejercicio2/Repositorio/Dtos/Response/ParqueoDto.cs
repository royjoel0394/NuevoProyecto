namespace Ejercicio2.Repositorio.DTOs.Response;

public class ParqueoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string NombreDeProvincia { get; set; } = string.Empty;
    public decimal PrecioPorHora { get; set; }
}
