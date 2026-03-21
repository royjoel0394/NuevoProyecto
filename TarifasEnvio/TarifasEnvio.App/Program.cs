using System.Collections.Generic;
using System.Globalization;

public class ZonaNoExisteException : Exception
{
    public ZonaNoExisteException()
    {
    }

    public ZonaNoExisteException(string message)
        : base(message)
    {
    }

    public ZonaNoExisteException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

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
        return CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, tarifasBase, out _);
    }

    public static decimal CalcularTarifaEnvio(decimal cantidadKg, string zonaOrigen, string zonaDestino, 
        Dictionary<(string Origen, string Destino), decimal> tarifasBase, out string log)
    {
        try
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
                log = $"En la fecha {DateTime.Now:dd-MM-yyyy HH:mm:ss} se procesó un envío de {cantidadKg.ToString("F2", CultureInfo.InvariantCulture)} kg desde {zonaOrigen} hacia {zonaDestino}. Costo total calculado: {0.00m.ToString("F2", CultureInfo.InvariantCulture)}.";
                return 0.0m;
            }

            // Buscar tarifa en el diccionario
            var key = (zonaOrigen.ToLower(), zonaDestino.ToLower());

            bool origenExiste = tarifasBase.Keys.Any(k => string.Equals(k.Origen, zonaOrigen, StringComparison.OrdinalIgnoreCase) || string.Equals(k.Destino, zonaOrigen, StringComparison.OrdinalIgnoreCase));
            bool destinoExiste = tarifasBase.Keys.Any(k => string.Equals(k.Origen, zonaDestino, StringComparison.OrdinalIgnoreCase) || string.Equals(k.Destino, zonaDestino, StringComparison.OrdinalIgnoreCase));

            if (!origenExiste)
            {
                log = $"En la fecha {DateTime.Now:dd-MM-yyyy HH:mm:ss} se intentó procesar un envío de {cantidadKg.ToString("F2", CultureInfo.InvariantCulture)} kg desde {zonaOrigen} hacia {zonaDestino}. NO se procesó, ya que la zona origen {zonaOrigen} no existe.";
                throw new ZonaNoExisteException($"La zona origen {zonaOrigen} no existe");
            }

            if (!destinoExiste)
            {
                log = $"En la fecha {DateTime.Now:dd-MM-yyyy HH:mm:ss} se intentó procesar un envío de {cantidadKg.ToString("F2", CultureInfo.InvariantCulture)} kg desde {zonaOrigen} hacia {zonaDestino}. NO se procesó, ya que la zona destino {zonaDestino} no existe.";
                throw new ZonaNoExisteException($"La zona destino {zonaDestino} no existe");
            }

            if (tarifasBase.TryGetValue(key, out decimal tarifaBase))
            {
                decimal total = Math.Round(tarifaBase * cantidadKg, 2, MidpointRounding.AwayFromZero);
                log = $"En la fecha {DateTime.Now:dd-MM-yyyy HH:mm:ss} se procesó un envío de {cantidadKg.ToString("F2", CultureInfo.InvariantCulture)} kg desde {zonaOrigen} hacia {zonaDestino}. Costo total calculado: {total.ToString("F2", CultureInfo.InvariantCulture)}.";
                return total;
            }

            // Ruta directa no encontrada, buscar ruta inversa
            var keyInversa = (zonaDestino.ToLower(), zonaOrigen.ToLower());
            if (tarifasBase.TryGetValue(keyInversa, out decimal tarifaInversa))
            {
                // Calcular costo con tarifa inversa y aplicar recargo del 10% (redondeo paso a paso según política global)
                decimal costoSinRecargoNoRedondeado = tarifaInversa * cantidadKg;
                decimal costoSinRecargo = Math.Round(costoSinRecargoNoRedondeado, 2, MidpointRounding.AwayFromZero);
                decimal recargo = Math.Round(costoSinRecargoNoRedondeado * 0.10m, 2, MidpointRounding.AwayFromZero);
                decimal costoTotal = Math.Round(costoSinRecargo + recargo, 2, MidpointRounding.AwayFromZero);

                log = $"En la fecha {DateTime.Now:dd-MM-yyyy HH:mm:ss} se intentó procesar un envío de {cantidadKg.ToString("F2", CultureInfo.InvariantCulture)} kg desde {zonaOrigen} hacia {zonaDestino}. NO se procesó, ya que no se encontró una ruta directa; sin embargo, se encontró una ruta inversa que tiene un recargo de 10%, por lo que el costo calculado es de {costoSinRecargo.ToString("F2", CultureInfo.InvariantCulture)} + {recargo.ToString("F2", CultureInfo.InvariantCulture)} (10%) = {costoTotal.ToString("F2", CultureInfo.InvariantCulture)}.";
                return costoTotal;
            }

            // Ni ruta directa ni inversa existen, buscar ruta con transbordo
            var rutasTransbordo = new List<(string intermedia, decimal costo1Final, decimal costo2Final, string detalleTramo1, string detalleTramo2)>();

            // Obtener todas las zonas únicas en el diccionario
            var todasLasZonas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var ruta in tarifasBase.Keys)
            {
                todasLasZonas.Add(ruta.Origen);
                todasLasZonas.Add(ruta.Destino);
            }

            // Buscar zonas intermedias
            foreach (var intermedia in todasLasZonas)
            {
                if (intermedia.Equals(zonaOrigen, StringComparison.OrdinalIgnoreCase) || 
                    intermedia.Equals(zonaDestino, StringComparison.OrdinalIgnoreCase))
                    continue;

                // Calcular costo del tramo 1: origen -> intermedia
                var costo1 = CalcularCostoTramo(zonaOrigen, intermedia, cantidadKg, tarifasBase, out string detalleTramo1);
                if (costo1 == null) continue;

                // Calcular costo del tramo 2: intermedia -> destino
                var costo2 = CalcularCostoTramo(intermedia, zonaDestino, cantidadKg, tarifasBase, out string detalleTramo2);
                if (costo2 == null) continue;

                // Ruta con transbordo encontrada
                rutasTransbordo.Add((intermedia, costo1.Value, costo2.Value, detalleTramo1, detalleTramo2));
            }

            // Si se encontraron rutas con transbordo, seleccionar la de menor costo
            if (rutasTransbordo.Count > 0)
            {
                var mejorRuta = rutasTransbordo.OrderBy(r => r.costo1Final + r.costo2Final).First();
                decimal costoTotal = mejorRuta.costo1Final + mejorRuta.costo2Final;
                
                // Construir mensaje de detalles de cada tramo
                string detallesCompletos = $"{mejorRuta.detalleTramo1} + {mejorRuta.detalleTramo2}";
                
                log = $"En la fecha {DateTime.Now:dd-MM-yyyy HH:mm:ss} se intentó procesar un envío de {cantidadKg.ToString("F2", CultureInfo.InvariantCulture)} kg desde {zonaOrigen} hacia {zonaDestino}. NO se procesó, ya que no se encontró una ruta directa; sin embargo, se encontró una ruta con transbordo que va de {zonaOrigen} a {mejorRuta.intermedia} y luego a {zonaDestino}, por lo que el costo calculado es de {detallesCompletos} = {costoTotal.ToString("F2", CultureInfo.InvariantCulture)}.";
                return costoTotal;
            }

            // No existe ninguna ruta (ni directa, ni inversa, ni con transbordo)
            log = $"En la fecha {DateTime.Now:dd-MM-yyyy HH:mm:ss} se intentó procesar un envío de {cantidadKg.ToString("F2", CultureInfo.InvariantCulture)} kg desde {zonaOrigen} hacia {zonaDestino}. NO se procesó, ya que no existe ninguna ruta disponible.";
            throw new ZonaNoExisteException("No existe ruta disponible (directa, inversa, ni con transbordo) para el origen y destino especificados");
        }
        catch (ZonaNoExisteException)
        {
            // El log ya fue asignado antes de lanzar la excepción.
            throw;
        }
        catch (Exception ex)
        {
            log = $"Error: {ex.Message}";
            throw;
        }
    }

    /// <summary>
    /// Calcula el costo de un tramo individual (origen a destino), evaluando ruta directa e inversa.
    /// Retorna el costo final y en 'detalles' incluye información sobre si fue ruta directa o inversa.
    /// </summary>
    private static decimal? CalcularCostoTramo(string origen, string destino, decimal cantidadKg,
        Dictionary<(string Origen, string Destino), decimal> tarifasBase, out string detalles)
    {
        detalles = "";
        var keyDirecta = (origen.ToLower(), destino.ToLower());

        // Intenta ruta directa
        if (tarifasBase.TryGetValue(keyDirecta, out decimal tarifaDirecta))
        {
            decimal costo = tarifaDirecta * cantidadKg;
            costo = Math.Round(costo, 2, MidpointRounding.AwayFromZero);
            detalles = costo.ToString("F2", CultureInfo.InvariantCulture);
            return costo;
        }

        // Intenta ruta inversa
        var keyInversa = (destino.ToLower(), origen.ToLower());
        if (tarifasBase.TryGetValue(keyInversa, out decimal tarifaInversa))
        {
            decimal costoSinRecargoNoRedondeado = tarifaInversa * cantidadKg;
            decimal costoSinRecargo = Math.Round(costoSinRecargoNoRedondeado, 2, MidpointRounding.AwayFromZero);
            decimal recargo = Math.Round(costoSinRecargoNoRedondeado * 0.10m, 2, MidpointRounding.AwayFromZero);
            decimal costoTotal = Math.Round(costoSinRecargo + recargo, 2, MidpointRounding.AwayFromZero);
            detalles = $"{costoSinRecargo.ToString("F2", CultureInfo.InvariantCulture)} + {recargo.ToString("F2", CultureInfo.InvariantCulture)}"; // Mostrar componentes de la ruta inversa
            return costoTotal;
        }

        return null;
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
