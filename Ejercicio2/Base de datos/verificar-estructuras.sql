-- =====================================================
-- SCRIPT DE VERIFICACIÓN: Estructura de tablas PRQ
-- SQL Server en la nube
-- =====================================================

USE [hecferme-sql-server];
GO

PRINT '🔍 VERIFICANDO ESTRUCTURA DE TABLAS PRQ';
PRINT '========================================';

-- Verificar estructura de todas las tablas PRQ
SELECT
    'PRQ_Automoviles' as Tabla,
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH as Max_Length,
    IS_NULLABLE,
    COLUMN_DEFAULT as Default_Value
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'PRQ_Automoviles'
ORDER BY ORDINAL_POSITION

UNION ALL

SELECT
    'PRQ_Parqueo' as Tabla,
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'PRQ_Parqueo'
ORDER BY ORDINAL_POSITION

UNION ALL

SELECT
    'PRQ_IngresoAutomoviles' as Tabla,
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'PRQ_IngresoAutomoviles'
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT '📊 DATOS ACTUALES EN LAS TABLAS';
PRINT '===============================';

-- Mostrar datos de PRQ_Automoviles
PRINT 'PRQ_Automoviles:';
SELECT id, fabricante, tipo, color, año
FROM PRQ_Automoviles
ORDER BY id;

PRINT '';

-- Mostrar datos de PRQ_Parqueo
PRINT 'PRQ_Parqueo:';
SELECT id, nombre, nombre_de_provincia, precio_por_hora
FROM PRQ_Parqueo
ORDER BY id;

PRINT '';

-- Mostrar datos de PRQ_IngresoAutomoviles
PRINT 'PRQ_IngresoAutomoviles (primeros 10 registros):';
SELECT TOP 10 consecutivo, id_parqueo, id_automovil,
       fecha_hora_entrada, fecha_hora_salida
FROM PRQ_IngresoAutomoviles
ORDER BY consecutivo;

PRINT '';
PRINT '✅ Verificación completada.';

-- =====================================================
-- FIN DEL SCRIPT DE VERIFICACIÓN
-- =====================================================