using System.Text.Json.Serialization;

namespace Ejercicio2.Repositorio.DTOs.Response;

public class AutomovilDto
{
    public int Id { get; set; }
    public string Color { get; set; } = string.Empty;
    
    [JsonPropertyName("año")]
    public int Año { get; set; }
    
    public string Fabricante { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
}
