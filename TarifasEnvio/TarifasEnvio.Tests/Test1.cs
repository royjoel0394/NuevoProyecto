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
    public void CalcularTarifaEnvio_ConLog_ValoresValidos_GeneraLogCorrecto()
    {
        // Arrange
        decimal cantidadKg = 5.0m;
        string zonaOrigen = "norte";
        string zonaDestino = "sur";

        // Act
        string log;
        decimal resultado = Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, _tarifasBase, out log);

        // Assert
        Assert.AreEqual(500.0m, resultado);
        Assert.IsFalse(string.IsNullOrWhiteSpace(log));
        StringAssert.StartsWith(log, "En la fecha ");
        StringAssert.Contains(log, "se procesó un envío de 5");
        StringAssert.Contains(log, "kg desde norte hacia sur.");
        StringAssert.Contains(log, "Costo total calculado: 500");
    }

    [TestMethod]
    public void CalcularTarifaEnvio_LogFormatoExacto_DdMmYyyy_Hh24MiSs_F2()
    {
        // Arrange
        decimal cantidadKg = 5.0m;
        string zonaOrigen = "norte";
        string zonaDestino = "sur";

        // Act
        string log;
        Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, _tarifasBase, out log);

        // Assert: fecha dd-mm-yyyy hora hh24:mi:ss y números con 2 decimales
        var regex = new System.Text.RegularExpressions.Regex(
            @"^En la fecha \d{2}-\d{2}-\d{4} \d{2}:\d{2}:\d{2} se procesó un envío de \d+\.\d{2} kg desde norte hacia sur\. Costo total calculado: \d+\.\d{2}\.$");

        Assert.IsTrue(regex.IsMatch(log), $"Formato de log no coincide: {log}");
    }

    [TestMethod]
    public void CalcularTarifaEnvio_ConLog_CantidadNoValida_GeneraLogError()
    {
        // Arrange
        decimal cantidadKg = 0.0m;
        string zonaOrigen = "norte";
        string zonaDestino = "sur";

        // Act
        string log = null!;
        try
        {
            Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, _tarifasBase, out log);
            Assert.Fail("Se esperaba ArgumentException");
        }
        catch (ArgumentException ex)
        {
            // Assert
            Assert.AreEqual("La cantidad debe ser mayor a cero", ex.Message);
            Assert.AreEqual("Error: La cantidad debe ser mayor a cero", log);
        }
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
    public void CalcularTarifaEnvio_RutaInexistente_LanzaZonaNoExisteException_Y_LogDirecto()
    {
        // Arrange
        decimal cantidadKg = 5.0m;
        string zonaOrigen = "norte";
        string zonaDestino = "oeste";

        var dict = new Dictionary<(string Origen, string Destino), decimal>
        {
            { ("norte", "sur"), 100.0m },
            { ("sur", "este"), 70.0m },
            { ("este", "norte"), 80.0m }
            // ouest no aparece en ninguna ruta, por lo que debe fallar en la validación
        };

        // Act
        string log = null!;
        try
        {
            Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, dict, out log);
            Assert.Fail("Se esperaba ZonaNoExisteException");
        }
        catch (ZonaNoExisteException ex)
        {
            // Assert
            StringAssert.Contains(ex.Message, "zona");
            Assert.IsNotNull(log);
            StringAssert.Contains(log, "NO se procesó");
        }
    }

    [TestMethod]
    public void CalcularTarifaEnvio_ZonaOrigenInexistente_LanzaZonaNoExisteException_Y_LogEspecifico()
    {
        // Arrange
        decimal cantidadKg = 15.5m;
        string zonaOrigen = "xyz";
        string zonaDestino = "sur";

        // Act
        string log = null!;
        try
        {
            Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, _tarifasBase, out log);
            Assert.Fail("Se esperaba ZonaNoExisteException");
        }
        catch (ZonaNoExisteException ex)
        {
            // Assert
            Assert.AreEqual("La zona origen xyz no existe", ex.Message);
            Assert.IsNotNull(log);
            StringAssert.Contains(log, "NO se procesó, ya que la zona origen xyz no existe.");
        }
    }

    [TestMethod]
    public void CalcularTarifaEnvio_ZonaDestinoInexistente_LanzaZonaNoExisteException_Y_LogEspecifico()
    {
        // Arrange
        decimal cantidadKg = 15.5m;
        string zonaOrigen = "norte";
        string zonaDestino = "xyz";

        // Act
        string log = null!;
        try
        {
            Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, _tarifasBase, out log);
            Assert.Fail("Se esperaba ZonaNoExisteException");
        }
        catch (ZonaNoExisteException ex)
        {
            // Assert
            Assert.AreEqual("La zona destino xyz no existe", ex.Message);
            Assert.IsNotNull(log);
            StringAssert.Contains(log, "NO se procesó, ya que la zona destino xyz no existe.");
        }
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
    public void CalcularTarifaEnvio_RedondeoCostoFinal_UsaMidpointAwayFromZeroYLogF2()
    {
        // Arrange
        decimal cantidadKg = 15.502m;
        string zonaOrigen = "a";
        string zonaDestino = "b";

        var dict = new Dictionary<(string Origen, string Destino), decimal>
        {
            { ("a", "b"), 2.5m }
        };

        // Act
        string log;
        decimal resultado = Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, dict, out log);

        // 15.502 * 2.5 = 38.755 -> 38.76
        Assert.AreEqual(38.76m, resultado);
        StringAssert.Contains(log, "38.76");
        Assert.IsFalse(log.Contains("38.755"));
    }

    [TestMethod]
    public void CalcularTarifaEnvio_RedondeoRecargoRutaInversa_UsaMidpointAwayFromZeroYLogF2()
    {
        // Arrange
        decimal cantidadKg = 15.498m;
        string zonaOrigen = "a";
        string zonaDestino = "b";

        var dict = new Dictionary<(string Origen, string Destino), decimal>
        {
            { ("b", "a"), 2.5m }
        };

        // Act
        string log;
        decimal resultado = Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, dict, out log);

        // Costo sin recargo: 38.745 -> 38.75
        // Recargo 10%: 3.8745 -> 3.87
        // Total: 42.62
        Assert.AreEqual(42.62m, resultado);
        StringAssert.Contains(log, "38.75");
        StringAssert.Contains(log, "3.87");
        StringAssert.Contains(log, "42.62");
    }

    [TestMethod]
    public void CalcularTarifaEnvio_RedondeoTransbordoMultiplesTramos_UsaMidpointAwayFromZeroYLogF2()
    {
        // Arrange
        decimal cantidadKg = 10.0m;
        string zonaOrigen = "a";
        string zonaDestino = "d";

        var dict = new Dictionary<(string Origen, string Destino), decimal>
        {
            { ("a", "b"), 1.2345m }, // 12.345 -> 12.35
            { ("b", "d"), 2.3455m }  // 23.455 -> 23.46
        };

        // Act
        string log;
        decimal resultado = Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, dict, out log);

        // Total: 12.35 + 23.46 = 35.81
        Assert.AreEqual(35.81m, resultado);
        StringAssert.Contains(log, "12.35");
        StringAssert.Contains(log, "23.46");
        StringAssert.Contains(log, "35.81");
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

    [TestMethod]
    public void CalcularTarifaEnvio_RutaInversaExiste_CalculaConRecargoY_LogCorrecto()
    {
        // Arrange
        decimal cantidadKg = 5.0m;
        string zonaOrigen = "sur";
        string zonaDestino = "norte";
        // Ruta directa sur->norte no existe, pero norte->sur sí (100.0m)
        
        var dict = new Dictionary<(string Origen, string Destino), decimal>
        {
            { ("norte", "sur"), 100.0m },
            { ("norte", "este"), 80.0m },
            { ("sur", "este"), 70.0m }
        };

        // Act
        string log;
        decimal resultado = Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, dict, out log);

        // Assert
        // Costo sin recargo: 100.0 * 5.0 = 500.0
        // Recargo 10%: 500.0 * 0.10 = 50.0
        // Total: 500.0 + 50.0 = 550.0
        Assert.AreEqual(550.0m, resultado);
        Assert.IsNotNull(log);
        StringAssert.Contains(log, "se encontró una ruta inversa");
        StringAssert.Contains(log, "recargo de 10%");
        StringAssert.Contains(log, "550");
    }

    [TestMethod]
    public void CalcularTarifaEnvio_RutaInversaConDecimales_RedondeoCorrecto()
    {
        // Arrange
        decimal cantidadKg = 15.5m;
        string zonaOrigen = "sur";
        string zonaDestino = "este";
        // Ruta directa sur->este no existe, pero este->sur existe (70.0m)
        
        var dict = new Dictionary<(string Origen, string Destino), decimal>
        {
            { ("norte", "sur"), 100.0m },
            { ("norte", "este"), 80.0m },
            { ("este", "norte"), 80.0m },
            { ("este", "sur"), 70.0m }
        };

        // Act
        string log;
        decimal resultado = Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, dict, out log);

        // Assert
        // Costo sin recargo: 70.0 * 15.5 = 1085.0
        // Recargo 10%: 1085.0 * 0.10 = 108.5
        // Total: 1085.0 + 108.5 = 1193.5
        Assert.AreEqual(1193.50m, resultado);
        Assert.IsNotNull(log);
        StringAssert.Contains(log, "se encontró una ruta inversa");
        StringAssert.Contains(log, "1193");
    }

    [TestMethod]
    public void CalcularTarifaEnvio_NiRutaDirectaNiInversaNiTransbordoExisten_LanzaZonaNoExisteException()
    {
        // Arrange
        decimal cantidadKg = 5.0m;
        string zonaOrigen = "a";
        string zonaDestino = "f"; 
        // a, b, c, d, e, f existen pero no hay forma de conectar A con F

        var dict = new Dictionary<(string Origen, string Destino), decimal>
        {
            { ("a", "b"), 1.0m },
            { ("b", "c"), 1.0m },
            { ("d", "e"), 1.0m },
            { ("e", "f"), 1.0m }
            // Dos componentes desconectadas: A-B-C y D-E-F. F no es accesible desde A
        };

        // Act
        string log = null!;
        try
        {
            Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, dict, out log);
            Assert.Fail("Se esperaba ZonaNoExisteException");
        }
        catch (ZonaNoExisteException ex)
        {
            // Assert
            StringAssert.Contains(ex.Message, "No existe");
            Assert.IsNotNull(log);
            StringAssert.Contains(log, "no existe");
        }
    }

    [TestMethod]
    public void CalcularTarifaEnvio_RutaConTransbordoSimple_CalculaCostoTotal()
    {
        // Arrange
        decimal cantidadKg = 10.0m;
        string zonaOrigen = "a";
        string zonaDestino = "d";
        
        var dict = new Dictionary<(string Origen, string Destino), decimal>
        {
            { ("a", "b"), 2.5m },  // Tramo 1: A -> B: 2.5 * 10 = 25.00
            { ("b", "d"), 3.0m },  // Tramo 2: B -> D: 3.0 * 10 = 30.00
            { ("a", "c"), 2.0m }
        };

        // Act
        string log;
        decimal resultado = Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, dict, out log);

        // Assert
        // Total: 25.00 + 30.00 = 55.00
        Assert.AreEqual(55.00m, resultado);
        Assert.IsNotNull(log);
        StringAssert.Contains(log, "transbordo");
        StringAssert.Contains(log, "55");
    }

    [TestMethod]
    public void CalcularTarifaEnvio_RutaTransbordoConRutaInversaEnUnTramo_RecargoIncluido()
    {
        // Arrange
        decimal cantidadKg = 10.0m;
        string zonaOrigen = "a";
        string zonaDestino = "d";
        
        var dict = new Dictionary<(string Origen, string Destino), decimal>
        {
            { ("b", "a"), 2.5m },  // Ruta inversa para A -> B: 2.5 * 10 = 25 + 2.5 (10%) = 27.5
            { ("b", "d"), 3.0m },  // Tramo 2: B -> D: 3.0 * 10 = 30.00
            { ("a", "c"), 2.0m }
        };

        // Act
        string log;
        decimal resultado = Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, dict, out log);

        // Assert
        // Tramo 1 (ruta inversa): 25 + 2.5 = 27.5
        // Tramo 2: 30.00
        // Total: 27.5 + 30 = 57.5
        Assert.AreEqual(57.50m, resultado);
        Assert.IsNotNull(log);
        StringAssert.Contains(log, "transbordo");
        StringAssert.Contains(log, "57"); // Flexible para formato regional
    }

    [TestMethod]
    public void CalcularTarifaEnvio_RutaTransbordoAmbosTramosInversos_RecargoEnAmbos()
    {
        // Arrange
        decimal cantidadKg = 10.0m;
        string zonaOrigen = "a";
        string zonaDestino = "d";
        
        var dict = new Dictionary<(string Origen, string Destino), decimal>
        {
            { ("b", "a"), 2.5m },  // Ruta inversa: 25 + 2.5 = 27.5
            { ("d", "b"), 3.0m },  // Ruta inversa: 30 + 3.0 = 33.0
            { ("a", "c"), 1.0m }
        };

        // Act
        string log;
        decimal resultado = Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, dict, out log);

        // Assert
        // Tramo 1 (ruta inversa): 25 + 2.5 = 27.5
        // Tramo 2 (ruta inversa): 30 + 3.0 = 33.0
        // Total: 27.5 + 33.0 = 60.5
        Assert.AreEqual(60.50m, resultado);
        Assert.IsNotNull(log);
        StringAssert.Contains(log, "transbordo");
        StringAssert.Contains(log, "60"); // Flexible para formato regional
    }

    [TestMethod]
    public void CalcularTarifaEnvio_VariasRutasTransbordo_SelectionaMenorCosto()
    {
        // Arrange
        decimal cantidadKg = 10.0m;
        string zonaOrigen = "a";
        string zonaDestino = "d";
        
        var dict = new Dictionary<(string Origen, string Destino), decimal>
        {
            // Opción 1: A -> B -> D: 10 + 20 = 30.00
            { ("a", "b"), 1.0m },
            { ("b", "d"), 2.0m },
            
            // Opción 2: A -> C -> D: 15 + 15 = 30.00
            { ("a", "c"), 1.5m },
            { ("c", "d"), 1.5m }
        };

        // Act
        string log;
        decimal resultado = Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, dict, out log);

        // Assert
        // Ambas rutas cuestan 30.00, se selecciona la primera (por orden de búsqueda)
        Assert.AreEqual(30.00m, resultado);
        Assert.IsNotNull(log);
        StringAssert.Contains(log, "transbordo");
    }

    [TestMethod]
    public void CalcularTarifaEnvio_NingunaRutaDisponible_LanzaExcepcion()
    {
        // Arrange
        decimal cantidadKg = 10.0m;
        string zonaOrigen = "a";
        string zonaDestino = "d";
        
        var dict = new Dictionary<(string Origen, string Destino), decimal>
        {
            // Zonas A, B, C y D existen pero no hay forma de conectar A -> D
            { ("a", "b"), 1.0m },
            { ("b", "c"), 1.0m },
            { ("c", "b"), 1.0m },
            { ("d", "d"), 0.5m }  // D existe pero está aislado
        };

        // Act
        string log = null!;
        try
        {
            Program.CalcularTarifaEnvio(cantidadKg, zonaOrigen, zonaDestino, dict, out log);
            Assert.Fail("Se esperaba ZonaNoExisteException");
        }
        catch (ZonaNoExisteException ex)
        {
            // Assert
            StringAssert.Contains(ex.Message, "No existe");
            Assert.IsNotNull(log);
            StringAssert.Contains(log, "no existe");
        }
    }
}