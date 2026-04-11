-- =====================================================
-- SCRIPT PARA CORREGIR TABLA PRQ_Parqueo
-- SQL Server en la nube
-- =====================================================
-- Descripción: Agregar columna nombre_de_provincia a la tabla PRQ_Parqueo
-- para que coincida con la estructura diseñada
-- =====================================================

USE [hecferme-sql-server];
GO

-- Verificar si la columna nombre_de_provincia ya existe
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID('PRQ_Parqueo')
    AND name = 'nombre_de_provincia'
)
BEGIN
    PRINT 'Agregando columna nombre_de_provincia a PRQ_Parqueo...';

    -- Agregar la columna nombre_de_provincia
    ALTER TABLE PRQ_Parqueo
    ADD nombre_de_provincia NVARCHAR(100) NOT NULL DEFAULT 'Sin especificar';

    PRINT '✅ Columna nombre_de_provincia agregada exitosamente.';
END
ELSE
BEGIN
    PRINT 'ℹ️ La columna nombre_de_provincia ya existe en PRQ_Parqueo.';
END

-- Actualizar datos de ejemplo (si la tabla tiene datos)
-- Asumiendo que hay parqueos existentes, asignarles provincias
IF EXISTS (SELECT 1 FROM PRQ_Parqueo WHERE nombre_de_provincia = 'Sin especificar')
BEGIN
    PRINT 'Actualizando datos de provincias...';

    -- Actualizar algunos registros con provincias de ejemplo
    UPDATE PRQ_Parqueo
    SET nombre_de_provincia = CASE
        WHEN id = 1 THEN 'San José'
        WHEN id = 2 THEN 'Heredia'
        ELSE 'Alajuela'  -- Provincia por defecto
    END
    WHERE nombre_de_provincia = 'Sin especificar';

    PRINT '✅ Datos de provincias actualizados.';
END

-- Verificar la estructura final de la tabla
PRINT 'Estructura final de PRQ_Parqueo:';
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'PRQ_Parqueo'
ORDER BY ORDINAL_POSITION;

-- Mostrar datos actuales
PRINT 'Datos actuales en PRQ_Parqueo:';
SELECT id, nombre, nombre_de_provincia, precio_por_hora
FROM PRQ_Parqueo
ORDER BY id;

PRINT '✅ Script completado exitosamente.';

-- =====================================================
-- FIN DEL SCRIPT
-- =====================================================