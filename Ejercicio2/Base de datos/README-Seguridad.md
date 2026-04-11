# Configuración Segura de Base de Datos SQL Server en la Nube

## 🔐 Consideraciones de Seguridad

**IMPORTANTE:** Nunca almacenes credenciales de base de datos en el código fuente. Utiliza variables de entorno o servicios de gestión de secretos como Azure Key Vault.

## 📋 Configuración de Variables de Entorno

### Opción 1: Variables de Entorno (Recomendado para desarrollo local)

Crea un archivo `.env` en la raíz del proyecto (agregarlo a `.gitignore`):

```env
SQL_SERVER_HOST=hecferme-sqlserver.database.windows.net
SQL_SERVER_DATABASE=hecferme-sql-server
SQL_SERVER_USER=user01
SQL_SERVER_PASSWORD=MyVeryStr0ngPassw0rd*
```

### Opción 2: Azure Key Vault (Recomendado para producción)

1. **Crear Azure Key Vault:**
   ```bash
   az keyvault create --name tu-key-vault --resource-group tu-resource-group --location eastus
   ```

2. **Crear secreto con la cadena de conexión:**
   ```bash
   az keyvault secret set --vault-name tu-key-vault --name SqlServerConnectionString --value "Server=hecferme-sqlserver.database.windows.net,1433;Database=hecferme-sql-server;User Id=user01;Password=MyVeryStr0ngPassw0rd*;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
   ```

3. **Configurar acceso para tu aplicación:**
   - Asigna permisos de lectura al Key Vault para tu aplicación
   - Usa Managed Identity o Service Principal

## 🛠️ Configuración en .NET

### 1. Instalar paquetes NuGet:

```bash
dotnet add package Azure.Security.KeyVault.Secrets
dotnet add package Azure.Identity
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Extensions.Configuration.EnvironmentVariables
```

### 2. Configurar appsettings.json:

```json
{
  "ConnectionStrings": {
    "ParqueoDatabase": "Server=hecferme-sqlserver.database.windows.net,1433;Database=hecferme-sql-server;User Id=user01;Password=MyVeryStr0ngPassw0rd*;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  },
  "AzureKeyVault": {
    "VaultUri": "https://tu-key-vault.vault.azure.net/",
    "SecretName": "SqlServerConnectionString"
  }
}
```

### 3. Configurar Program.cs:

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        // Cargar configuración desde appsettings.json
        config.AddJsonFile("appsettings.json", optional: true);

        // Cargar variables de entorno
        config.AddEnvironmentVariables();

        // Cargar desde Azure Key Vault (si está configurado)
        var builtConfig = config.Build();
        var keyVaultUri = builtConfig["AzureKeyVault:VaultUri"];
        if (!string.IsNullOrEmpty(keyVaultUri))
        {
            config.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
        }
    })
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<DatabaseConnection>();
    })
    .Build();

var dbConnection = host.Services.GetRequiredService<DatabaseConnection>();

// Usar variables de entorno
string connectionString = dbConnection.GetConnectionStringFromEnvironment();
await dbConnection.TestConnectionAsync(connectionString);
await dbConnection.ConsultarDatosParqueoAsync(connectionString);

// O usar Azure Key Vault
// string connectionString = await dbConnection.GetConnectionStringFromKeyVaultAsync();
```

## 🔍 Consultas de Ejemplo

### Ver todos los automóviles:
```sql
SELECT * FROM PRQ_Automoviles ORDER BY fabricante;
```

### Ver parqueos por provincia:
```sql
SELECT * FROM PRQ_Parqueo WHERE nombre_de_provincia = 'San José';
```

### Ver ingresos activos:
```sql
SELECT i.*, a.fabricante, a.tipo, p.nombre as parqueo
FROM PRQ_IngresoAutomoviles i
INNER JOIN PRQ_Automoviles a ON i.id_automovil = a.id
INNER JOIN PRQ_Parqueo p ON i.id_parqueo = p.id
WHERE i.fecha_hora_salida IS NULL;
```

### Calcular tiempo de parqueo y costo:
```sql
SELECT
    i.consecutivo,
    a.fabricante,
    a.tipo,
    p.nombre as parqueo,
    i.fecha_hora_entrada,
    i.fecha_hora_salida,
    DATEDIFF(MINUTE, i.fecha_hora_entrada, i.fecha_hora_salida) as minutos_parqueo,
    DATEDIFF(MINUTE, i.fecha_hora_entrada, i.fecha_hora_salida) / 60.0 * p.precio_por_hora as costo_total
FROM PRQ_IngresoAutomoviles i
INNER JOIN PRQ_Automoviles a ON i.id_automovil = a.id
INNER JOIN PRQ_Parqueo p ON i.id_parqueo = p.id
WHERE i.fecha_hora_salida IS NOT NULL;
```

## ⚠️ Notas de Seguridad

1. **Nunca commits credenciales** al repositorio Git
2. **Usa HTTPS** para todas las conexiones
3. **Configura timeouts** apropiados
4. **Implementa reintentos** para conexiones fallidas
5. **Monitorea** el uso de la base de datos
6. **Usa RBAC** (Role-Based Access Control) en Azure
7. **Rota** las contraseñas regularmente

## � Corrección de Estructura de Base de Datos

### Problema identificado
La tabla `PRQ_Parqueo` en la base de datos del profesor no incluye la columna `nombre_de_provincia`, causando errores en Entity Framework Core.

### Solución
1. **Ejecuta** [corregir-tabla-parqueo.sql](corregir-tabla-parqueo.sql) para agregar la columna faltante
2. **Verifica** con [verificar-estructuras.sql](verificar-estructuras.sql) que la estructura sea correcta
3. **Prueba** la aplicación con `dotnet run`

### Archivos relacionados
- [README-Correccion-Parqueo.md](README-Correccion-Parqueo.md) - Instrucciones detalladas
- [corregir-tabla-parqueo.sql](corregir-tabla-parqueo.sql) - Script de corrección
- [verificar-estructuras.sql](verificar-estructuras.sql) - Verificación de estructuras