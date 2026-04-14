using ParqueoDatabaseExample.Models;

namespace Repositorio.Tests.Helpers;

public static class TestDataBuilder
{
    public static PrqAutomovile CreateAutomovil(int id = 1)
    {
        return new PrqAutomovile
        {
            Id = id,
            Color = "Rojo",
            Año = 2022,
            Fabricante = "Toyota",
            Tipo = "Sedán"
        };
    }

    public static PrqAutomovile CreateAutomovilWithId(int id)
    {
        return new PrqAutomovile
        {
            Id = id,
            Color = "Azul",
            Año = 2023,
            Fabricante = "Honda",
            Tipo = "SUV"
        };
    }

    public static PrqParqueo CreateParqueo(int id = 1)
    {
        return new PrqParqueo
        {
            Id = id,
            Nombre = "Parqueo Central",
            NombreDeProvincia = "San José",
            PrecioPorHora = 10.00m
        };
    }

    public static PrqParqueo CreateParqueoWithId(int id)
    {
        return new PrqParqueo
        {
            Id = id,
            Nombre = "Parqueo Norte",
           NombreDeProvincia = "Alajuela",
            PrecioPorHora = 15.00m
        };
    }

    public static PrqIngresoAutomovile CreateIngreso(int idAutomovil, int idParqueo, int consecutivo = 1)
    {
        return new PrqIngresoAutomovile
        {
            Consecutivo = consecutivo,
            IdAutomovil = idAutomovil,
            IdParqueo = idParqueo,
            FechaHoraEntrada = DateTime.Now.AddHours(-2),
            FechaHoraSalida = DateTime.Now
        };
    }

    public static IEnumerable<PrqAutomovile> CreateMultipleAutomoviles()
    {
        return new List<PrqAutomovile>
        {
            new() { Id = 1, Color = "Rojo", Año = 2022, Fabricante = "Toyota", Tipo = "Sedán" },
            new() { Id = 2, Color = "Azul", Año = 2023, Fabricante = "Honda", Tipo = "SUV" },
            new() { Id = 3, Color = "Verde", Año = 2021, Fabricante = "Ford", Tipo = "Camioneta" },
            new() { Id = 4, Color = "Rojo", Año = 2020, Fabricante = "Toyota", Tipo = "SUV" },
            new() { Id = 5, Color = "Negro", Año = 2023, Fabricante = "BMW", Tipo = "Sedán" }
        };
    }

    public static IEnumerable<PrqParqueo> CreateMultipleParqueos()
    {
        return new List<PrqParqueo>
        {
            new() { Id = 1, Nombre = "Parqueo Central", NombreDeProvincia = "San José", PrecioPorHora = 10.00m },
            new() { Id = 2, Nombre = "Parqueo Norte", NombreDeProvincia = "Alajuela", PrecioPorHora = 15.00m },
            new() { Id = 3, Nombre = "Parqueo Oeste", NombreDeProvincia = "San José", PrecioPorHora = 12.00m }
        };
    }
}
