-- =====================================================
-- AGREGAR COLUMNAS FISICAS A PRQ_IngresoAutomoviles
-- SQL Server - user01
-- =====================================================

USE [hecferme-sql-server];
GO

PRINT '=========================================================';
PRINT '  AGREGANDO COLUMNAS A PRQ_IngresoAutomoviles';
PRINT '=========================================================';

-- =====================================================
-- Agregar columnas si no existen
-- =====================================================

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
             WHERE TABLE_NAME = 'PRQ_IngresoAutomoviles' AND COLUMN_NAME = 'duracion_minutos')
BEGIN
    ALTER TABLE PRQ_IngresoAutomoviles 
    ADD duracion_minutos INT NULL;
    PRINT 'Columna duracion_minutos agregada.';
END
ELSE
    PRINT 'Columna duracion_minutos ya existe.';
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
             WHERE TABLE_NAME = 'PRQ_IngresoAutomoviles' AND COLUMN_NAME = 'duracion_horas')
BEGIN
    ALTER TABLE PRQ_IngresoAutomoviles 
    ADD duracion_horas DECIMAL(5,2) NULL;
    PRINT 'Columna duracion_horas agregada.';
END
ELSE
    PRINT 'Columna duracion_horas ya existe.';
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
             WHERE TABLE_NAME = 'PRQ_IngresoAutomoviles' AND COLUMN_NAME = 'monto_total_pagar')
BEGIN
    ALTER TABLE PRQ_IngresoAutomoviles 
    ADD monto_total_pagar DECIMAL(10,2) NULL;
    PRINT 'Columna monto_total_pagar agregada.';
END
ELSE
    PRINT 'Columna monto_total_pagar ya existe.';
GO

-- =====================================================
-- Actualizar valores para registros existentes
-- =====================================================
PRINT '';
PRINT 'Actualizando registros existentes...';

UPDATE PRQ_IngresoAutomoviles
SET 
    duracion_minutos = CASE 
        WHEN fecha_hora_salida IS NULL THEN NULL
        ELSE DATEDIFF(MINUTE, fecha_hora_entrada, fecha_hora_salida)
    END,
    duracion_horas = CASE 
        WHEN fecha_hora_salida IS NULL THEN NULL
        ELSE ROUND(CAST(DATEDIFF(MINUTE, fecha_hora_entrada, fecha_hora_salida) AS FLOAT) / 60, 2)
    END,
    monto_total_pagar = CASE 
        WHEN fecha_hora_salida IS NULL THEN NULL
        ELSE ROUND(
            (CAST(DATEDIFF(MINUTE, fecha_hora_entrada, fecha_hora_salida) AS FLOAT) / 60) 
            * (SELECT precio_por_hora FROM PRQ_Parqueo WHERE id = PRQ_IngresoAutomoviles.id_parqueo),
            2)
    END
WHERE fecha_hora_salida IS NOT NULL;

PRINT 'Registros actualizados.';

-- =====================================================
-- Crear trigger para actualizar automaticamente
-- =====================================================
PRINT '';
PRINT 'Creando trigger...';

IF OBJECT_ID('dbo.TRG_ActualizarCamposIngreso', 'TR') IS NOT NULL
    DROP TRIGGER dbo.TRG_ActualizarCamposIngreso;
GO

CREATE TRIGGER TRG_ActualizarCamposIngreso
ON PRQ_IngresoAutomoviles
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE i
    SET 
        i.duracion_minutos = CASE 
            WHEN i.fecha_hora_salida IS NULL THEN NULL
            ELSE DATEDIFF(MINUTE, i.fecha_hora_entrada, i.fecha_hora_salida)
        END,
        i.duracion_horas = CASE 
            WHEN i.fecha_hora_salida IS NULL THEN NULL
            ELSE ROUND(CAST(DATEDIFF(MINUTE, i.fecha_hora_entrada, i.fecha_hora_salida) AS FLOAT) / 60, 2)
        END,
        i.monto_total_pagar = CASE 
            WHEN i.fecha_hora_salida IS NULL THEN NULL
            ELSE ROUND(
                (CAST(DATEDIFF(MINUTE, i.fecha_hora_entrada, i.fecha_hora_salida) AS FLOAT) / 60) 
                * p.precio_por_hora,
                2)
        END
    FROM PRQ_IngresoAutomoviles i
    INNER JOIN PRQ_Parqueo p ON i.id_parqueo = p.id
    WHERE i.fecha_hora_salida IS NOT NULL;
END;
GO

PRINT 'Trigger creado.';

-- =====================================================
-- Verificar estructura
-- =====================================================
PRINT '';
PRINT 'Estructura actual:';

SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'PRQ_IngresoAutomoviles'
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT '=========================================================';
PRINT '  COLUMNAS AGREGADAS CORRECTAMENTE';
PRINT '=========================================================';
GO