using System.Text.Json.Serialization;

namespace Ejercicio2.Repositorio.DTOs.Request;

public class CreateAutomovilDto
{
    public string Color { get; set; } = string.Empty;
    public int Ano { get; set; }
    public string Fabricante { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
}

public class UpdateAutomovilDto
{
    public string Color { get; set; } = string.Empty;
    public int Ano { get; set; }
    public string Fabricante { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
}