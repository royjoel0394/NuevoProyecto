using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ejercicio2.Repositorio.DTOs.Request;

public class CreateAutomovilDto
{
    public string Color { get; set; } = string.Empty;
    
    [JsonPropertyName("año")]
    public int Año { get; set; }
    
    [JsonPropertyName("ano")]
    public int Ano { get; set; }
    
    public string Fabricante { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    
    public void NormalizeAño()
    {
        if (Año == 0 && Ano > 0)
            Año = Ano;
    }
}

public class UpdateAutomovilDto
{
    public string Color { get; set; } = string.Empty;
    
    [JsonPropertyName("año")]
    public int Año { get; set; }
    
    [JsonPropertyName("ano")]
    public int Ano { get; set; }
    
    public string Fabricante { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    
    public void NormalizeAño()
    {
        if (Año == 0 && Ano > 0)
            Año = Ano;
    }
}

public class CreateAutomovilRequest
{
    public CreateAutomovilDto? Dto { get; set; }
    public CreateAutomovilDto? Directo { get; set; }
}

public class UpdateAutomovilRequest
{
    public UpdateAutomovilDto? Dto { get; set; }
    public UpdateAutomovilDto? Directo { get; set; }
}
