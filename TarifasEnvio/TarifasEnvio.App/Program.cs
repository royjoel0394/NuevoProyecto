using System.Collections.Generic;

public class Program
{
    // Diccionario estático con las tarifas base por trayecto
    private static readonly Dictionary<(string Origen, string Destino), decimal> TarifasBase = new()
    {
        { ("norte", "sur"), 100.0m },
        { ("norte", "este"), 80.0m },
        { ("norte", "oeste"), 90.0m },
        { ("sur", "norte"), 100.0m },
        { ("sur", "este"), 70.0m },
        { ("sur", "oeste"), 85.0m },
        { ("este", "norte"), 80.0m },
        { ("este", "sur"), 70.0m },
        { ("este", "oeste"), 60.0m },
        { ("oeste", "norte"), 90.0m },
        { ("oeste", "sur"), 85.0m },
        { ("oeste", "este"), 60.0m }
    };

    /// <summary>
    /// Calcula la tarifa de envío entre dos zonas geográficas.
    /// </summary>
    /// <param name="cantidadKg">La cantidad de kilogramos a enviar.</param>
    /// <param name="zonaOrigen">La zona de origen.</param>
    /// <param name="zonaDestino">La zona de destino.</param>
    /// <param name="tarifasBase">Diccionario que contiene las tarifas base por trayecto.</param>
    /// <returns>El costo total del envío (tarifaBase * cantidadKg).</returns>
    /// <exception cref="ArgumentException">Si cantidadKg es menor o igual a cero, o si las zonas son nulas o vacías.</exception>
    /// <exception cref="KeyNotFoundException">Si no existe tarifa para la ruta especificada.</exception>
    public static decimal CalcularTarifaEnvio(decimal cantidadKg, string zonaOrigen, string zonaDestino, 
        Dictionary<(string Origen, string Destino), decimal> tarifasBase)
    {
        // Validar cantidad de kilogramos
        if (cantidadKg <= 0)
        {
            throw new ArgumentException("La cantidad debe ser mayor a cero");
        }

        // Validar zona de origen
        if (string.IsNullOrWhiteSpace(zonaOrigen))
        {
            throw new ArgumentException("La zona de origen es requerida");
        }

        // Validar zona de destino
        if (string.IsNullOrWhiteSpace(zonaDestino))
        {
            throw new ArgumentException("La zona de destino es requerida");
        }

        // Si origen y destino son iguales, es envío gratuito
        if (zonaOrigen.Equals(zonaDestino, StringComparison.OrdinalIgnoreCase))
        {
            return 0.0m;
        }

        // Buscar tarifa en el diccionario
        var key = (zonaOrigen.ToLower(), zonaDestino.ToLower());
        if (tarifasBase.TryGetValue(key, out decimal tarifaBase))
        {
            return tarifaBase * cantidadKg;
        }

        // Si no existe la ruta, lanzar excepción
        throw new KeyNotFoundException("No existe tarifa para la ruta especificada");
    }

    static void Main(string[] args)
    {
        Console.WriteLine("=== Calculadora de Tarifas de Envío ===");
        Console.WriteLine();

        try
        {
            // Ejemplo 1: Envío válido de 5kg de norte a sur
            decimal costo1 = CalcularTarifaEnvio(5.0m, "norte", "sur", TarifasBase);
            Console.WriteLine($"Envío de 5kg de norte a sur: ${costo1:F2}");

            // Ejemplo 2: Envío válido de 3.5kg de este a oeste
            decimal costo2 = CalcularTarifaEnvio(3.5m, "este", "oeste", TarifasBase);
            Console.WriteLine($"Envío de 3.5kg de este a oeste: ${costo2:F2}");

            // Ejemplo 3: Envío dentro de la misma zona (gratuito)
            decimal costo3 = CalcularTarifaEnvio(10.0m, "norte", "norte", TarifasBase);
            Console.WriteLine($"Envío de 10kg dentro de la misma zona: ${costo3:F2} (Gratuito)");

            Console.WriteLine();
            Console.WriteLine("Todos los cálculos completados exitosamente.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error de validación: {ex.Message}");
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Error de tarifa: {ex.Message}");
        }
    }
}
