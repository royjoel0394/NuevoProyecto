namespace TarifasEnvio.Tests;

[TestClass]
public sealed class CalculoTarifaEnvioTests
{
    private Dictionary<(string Origen, string Destino), decimal> _tarifasBase = new();

    [TestInitialize]
    public void Setup()
    {
        _tarifasBase = new()
        {
            { ("norte", "sur"), 100.0m },
            { ("norte", "este"), 80.0m },
            { ("sur", "este"), 70.0m },
            { ("sur", "norte"), 100.0m },
            { ("este", "norte"), 80.0m },
            { ("este", "sur"), 70.0m }
        };
    }

    [TestMethod]
    public void CalcularTarifaEnvio_ValoresValidos_RetornaCostoCorrecto()
    {
        // Arrange
        decimal cantidadKg = 5.0m;
        string zonaOrigen = "norte";
        string zonaDestino = "sur";

        // Act
        decimal resultado = Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, _tarifasBase);

        // Assert
        Assert.AreEqual(500.0m, resultado);
    }

    [TestMethod]
    public void CalcularTarifaEnvio_MismaZona_RetornaCero()
    {
        // Arrange
        decimal cantidadKg = 10.0m;
        string zonaOrigen = "norte";
        string zonaDestino = "norte";

        // Act
        decimal resultado = Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, _tarifasBase);

        // Assert
        Assert.AreEqual(0.0m, resultado);
    }

    [TestMethod]
    public void CalcularTarifaEnvio_KilogramosNegativos_LanzaArgumentException()
    {
        // Arrange
        decimal cantidadKg = -1.0m;
        string zonaOrigen = "norte";
        string zonaDestino = "sur";
        bool exceptionThrown = false;
        string exceptionMessage = "";

        // Act
        try
        {
            Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, _tarifasBase);
        }
        catch (ArgumentException ex)
        {
            exceptionThrown = true;
            exceptionMessage = ex.Message;
        }

        // Assert
        Assert.IsTrue(exceptionThrown, "Se esperaba ArgumentException");
        Assert.AreEqual("La cantidad debe ser mayor a cero", exceptionMessage);
    }

    [TestMethod]
    public void CalcularTarifaEnvio_KilogramosCero_LanzaArgumentException()
    {
        // Arrange
        decimal cantidadKg = 0.0m;
        string zonaOrigen = "norte";
        string zonaDestino = "sur";
        bool exceptionThrown = false;
        string exceptionMessage = "";

        // Act
        try
        {
            Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, _tarifasBase);
        }
        catch (ArgumentException ex)
        {
            exceptionThrown = true;
            exceptionMessage = ex.Message;
        }

        // Assert
        Assert.IsTrue(exceptionThrown, "Se esperaba ArgumentException");
        Assert.AreEqual("La cantidad debe ser mayor a cero", exceptionMessage);
    }

    [TestMethod]
    public void CalcularTarifaEnvio_ZonaOrigenNula_LanzaArgumentException()
    {
        // Arrange
        decimal cantidadKg = 5.0m;
        string? zonaOrigen = null;
        string zonaDestino = "sur";
        bool exceptionThrown = false;
        string exceptionMessage = "";

        // Act
        try
        {
            Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen!, zonaDestino, _tarifasBase);
        }
        catch (ArgumentException ex)
        {
            exceptionThrown = true;
            exceptionMessage = ex.Message;
        }

        // Assert
        Assert.IsTrue(exceptionThrown, "Se esperaba ArgumentException");
        Assert.AreEqual("La zona de origen es requerida", exceptionMessage);
    }

    [TestMethod]
    public void CalcularTarifaEnvio_ZonaOrigenVacia_LanzaArgumentException()
    {
        // Arrange
        decimal cantidadKg = 5.0m;
        string zonaOrigen = "";
        string zonaDestino = "sur";
        bool exceptionThrown = false;
        string exceptionMessage = "";

        // Act
        try
        {
            Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, _tarifasBase);
        }
        catch (ArgumentException ex)
        {
            exceptionThrown = true;
            exceptionMessage = ex.Message;
        }

        // Assert
        Assert.IsTrue(exceptionThrown, "Se esperaba ArgumentException");
        Assert.AreEqual("La zona de origen es requerida", exceptionMessage);
    }

    [TestMethod]
    public void CalcularTarifaEnvio_ZonaDestinoNula_LanzaArgumentException()
    {
        // Arrange
        decimal cantidadKg = 5.0m;
        string zonaOrigen = "norte";
        string? zonaDestino = null;
        bool exceptionThrown = false;
        string exceptionMessage = "";

        // Act
        try
        {
            Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino!, _tarifasBase);
        }
        catch (ArgumentException ex)
        {
            exceptionThrown = true;
            exceptionMessage = ex.Message;
        }

        // Assert
        Assert.IsTrue(exceptionThrown, "Se esperaba ArgumentException");
        Assert.AreEqual("La zona de destino es requerida", exceptionMessage);
    }

    [TestMethod]
    public void CalcularTarifaEnvio_ZonaDestinoVacia_LanzaArgumentException()
    {
        // Arrange
        decimal cantidadKg = 5.0m;
        string zonaOrigen = "norte";
        string zonaDestino = "";
        bool exceptionThrown = false;
        string exceptionMessage = "";

        // Act
        try
        {
            Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, _tarifasBase);
        }
        catch (ArgumentException ex)
        {
            exceptionThrown = true;
            exceptionMessage = ex.Message;
        }

        // Assert
        Assert.IsTrue(exceptionThrown, "Se esperaba ArgumentException");
        Assert.AreEqual("La zona de destino es requerida", exceptionMessage);
    }

    [TestMethod]
    public void CalcularTarifaEnvio_RutaInexistente_LanzaKeyNotFoundException()
    {
        // Arrange
        decimal cantidadKg = 5.0m;
        string zonaOrigen = "norte";
        string zonaDestino = "oeste"; // No existe en el diccionario
        bool exceptionThrown = false;
        string exceptionMessage = "";

        // Act
        try
        {
            Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, _tarifasBase);
        }
        catch (KeyNotFoundException ex)
        {
            exceptionThrown = true;
            exceptionMessage = ex.Message;
        }

        // Assert
        Assert.IsTrue(exceptionThrown, "Se esperaba KeyNotFoundException");
        Assert.AreEqual("No existe tarifa para la ruta especificada", exceptionMessage);
    }

    [TestMethod]
    public void CalcularTarifaEnvio_ValoresDecimales_RedondeoCorreto()
    {
        // Arrange
        decimal cantidadKg = 1.5m;
        string zonaOrigen = "norte";
        string zonaDestino = "este"; // Tarifa 80.0m

        // Act
        decimal resultado = Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, _tarifasBase);

        // Assert
        Assert.AreEqual(120.0m, resultado); // 1.5 * 80 = 120
    }

    [TestMethod]
    public void CalcularTarifaEnvio_CaseInsensitive_ReconoceZonas()
    {
        // Arrange
        decimal cantidadKg = 2.0m;
        string zonaOrigen = "NORTE";
        string zonaDestino = "SUR";

        // Act
        decimal resultado = Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, _tarifasBase);

        // Assert
        Assert.AreEqual(200.0m, resultado); // 2.0 * 100 = 200
    }
}
