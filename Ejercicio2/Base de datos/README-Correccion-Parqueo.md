# 🔧 Corrección de Tabla PRQ_Parqueo

## ❌ Problema identificado

La tabla `PRQ_Parqueo` en la base de datos del profesor **no tiene la columna `nombre_de_provincia`**, lo que causa errores en la aplicación Entity Framework Core.

## ✅ Solución

### Paso 1: Ejecutar el script de corrección

Ejecuta el archivo **[corregir-tabla-parqueo.sql](corregir-tabla-parqueo.sql)** en SQL Server Management Studio (SSMS) o Azure Data Studio conectado a tu instancia en la nube.

**Conexión requerida:**
- Server: `hecferme-sqlserver.database.windows.net,1433`
- Database: `hecferme-sql-server`
- User: `user01`
- Password: `MyVeryStr0ngPassw0rd*`

### Paso 2: Verificar la corrección

Después de ejecutar el script, la tabla `PRQ_Parqueo` tendrá la estructura correcta:

```sql
-- Estructura esperada después de la corrección
CREATE TABLE PRQ_Parqueo (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre_de_provincia NVARCHAR(100) NOT NULL,  -- ← Columna agregada
    nombre NVARCHAR(100) NOT NULL,
    precio_por_hora DECIMAL(10,2) NOT NULL
);
```

### Paso 3: Probar la aplicación

Una vez corregida la tabla, ejecuta:

```bash
cd "c:\Github\NuevoProyecto\Ejercicio2\Base de datos"
$env:SQL_SERVER_PASSWORD = "MyVeryStr0ngPassw0rd*"
dotnet run
```

## 📊 Resultado esperado

Después de la corrección, la aplicación debería mostrar:

```
🚗 AUTOMÓVILES REGISTRADOS:
ID: 1, Toyota sedán Rojo (2020)
...

🏢 PARQUEOS DISPONIBLES:
ID: 1, Centro Parqueo San José (San José) - ₡2.50/hora
ID: 2, Parqueo Norte Heredia (Heredia) - ₡2.00/hora

🔄 INGRESOS ACTIVOS (AUTOMÓVILES EN PARQUEO):
...

📈 ESTADÍSTICAS DEL PARQUEO:
...
```

## 🔍 Verificación adicional

Si aún hay problemas, ejecuta esta consulta para verificar la estructura:

```sql
SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME LIKE 'PRQ%'
ORDER BY TABLE_NAME, ORDINAL_POSITION;
```

## ⚠️ Notas importantes

- El script es **seguro** y solo agrega la columna si no existe
- **No elimina** datos existentes
- Asigna valores por defecto a registros existentes
- Es **reversible** si es necesario

¡Una vez ejecutado el script, la aplicación funcionará completamente! 🚀