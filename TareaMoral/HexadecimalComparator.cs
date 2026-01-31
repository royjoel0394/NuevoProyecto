using System;

/// <summary>
/// Clase que implementa un comparador de números en hexadecimal.
/// Compara dos valores hexadecimales sin utilizar librerías externas.
/// </summary>
public class HexadecimalComparator
{
    /// <summary>
    /// Convierte una cadena hexadecimal a su valor decimal.
    /// </summary>
    /// <param name="hexString">La cadena hexadecimal a convertir</param>
    /// <returns>El valor decimal del número hexadecimal</returns>
    private static long HexToDecimal(string hexString)
    {
        long result = 0;
        
        // Convertir a mayúsculas para manejar consistentemente
        hexString = hexString.ToUpper();
        
        foreach (char c in hexString)
        {
            result *= 16;
            
            if (c >= '0' && c <= '9')
            {
                result += c - '0';
            }
            else if (c >= 'A' && c <= 'F')
            {
                result += c - 'A' + 10;
            }
            else
            {
                throw new ArgumentException($"Carácter inválido en hexadecimal: '{c}'");
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Compara dos números expresados en hexadecimal.
    /// </summary>
    /// <param name="firstHex">Primer valor hexadecimal como string</param>
    /// <param name="secondHex">Segundo valor hexadecimal como string</param>
    /// <returns>
    /// -1 si firstHex es menor que secondHex
    /// 0 si ambos son iguales
    /// 1 si firstHex es mayor que secondHex
    /// </returns>
    public static int Compare(string firstHex, string secondHex)
    {
        if (string.IsNullOrEmpty(firstHex))
            throw new ArgumentException("El primer valor no puede estar vacío");
        
        if (string.IsNullOrEmpty(secondHex))
            throw new ArgumentException("El segundo valor no puede estar vacío");
        
        long firstValue = HexToDecimal(firstHex);
        long secondValue = HexToDecimal(secondHex);
        
        if (firstValue < secondValue)
            return -1;
        else if (firstValue > secondValue)
            return 1;
        else
            return 0;
    }
}
