using System;

/// <summary>
/// Programa principal que permite comparar números hexadecimales de forma interactiva.
/// </summary>
class Program
{
    static void Main()
    {
        Console.WriteLine("=== Comparador de Números Hexadecimales ===\n");
        
        bool continuar = true;
        
        while (continuar)
        {
            try
            {
                // Solicitar el primer número hexadecimal
                Console.Write("Ingresa el primer número hexadecimal: ");
                string primerNumero = Console.ReadLine();
                
                // Solicitar el segundo número hexadecimal
                Console.Write("Ingresa el segundo número hexadecimal: ");
                string segundoNumero = Console.ReadLine();
                
                // Realizar la comparación
                int resultado = HexadecimalComparator.Compare(primerNumero, segundoNumero);
                
                // Mostrar el resultado
                string simbolo = resultado switch
                {
                    -1 => "<",
                    0 => "=",
                    1 => ">",
                    _ => "?"
                };
                
                string textoResultado = resultado switch
                {
                    -1 => "es menor que",
                    0 => "es igual a",
                    1 => "es mayor que",
                    _ => "no se puede comparar"
                };
                
                Console.WriteLine();
                Console.WriteLine($"Resultado: {primerNumero} {simbolo} {segundoNumero}");
                Console.WriteLine($"  {primerNumero} {textoResultado} {segundoNumero}");
                
                // Mostrar valores decimales para referencia
                long val1 = ConvertHexToDecimal(primerNumero);
                long val2 = ConvertHexToDecimal(segundoNumero);
                Console.WriteLine($"  Valores decimales: {val1} {simbolo} {val2}");
                Console.WriteLine();
                
                // Preguntar si desea continuar
                Console.Write("¿Deseas hacer otra comparación? (s/n): ");
                string respuesta = Console.ReadLine();
                continuar = respuesta.ToLower() == "s" || respuesta.ToLower() == "si";
                Console.WriteLine();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}\n");
            }
        }
        
        Console.WriteLine("¡Hasta luego!");
    }
    
    /// <summary>
    /// Función auxiliar para mostrar el valor decimal.
    /// </summary>
    static long ConvertHexToDecimal(string hexString)
    {
        long result = 0;
        hexString = hexString.ToUpper();
        
        foreach (char c in hexString)
        {
            result *= 16;
            if (c >= '0' && c <= '9')
                result += c - '0';
            else if (c >= 'A' && c <= 'F')
                result += c - 'A' + 10;
        }
        
        return result;
    }
}
