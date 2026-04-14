namespace Ejercicio2.Repositorio.DTOs.Request;

public class CreateParqueoDto
{
    public string Nombre { get; set; } = string.Empty;
    public string NombreDeProvincia { get; set; } = string.Empty;
    public decimal PrecioPorHora { get; set; }
}

public class UpdateParqueoDto
{
    public string Nombre { get; set; } = string.Empty;
    public string NombreDeProvincia { get; set; } = string.Empty;
    public decimal PrecioPorHora { get; set; }
}
