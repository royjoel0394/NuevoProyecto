-- =====================================================
-- CORREGIR COLUMNAS Y TRIGGER
-- SQL Server - user01
-- =====================================================

USE [hecferme-sql-server];
GO
SET QUOTED_IDENTIFIER ON;
GO

PRINT '=========================================================';
PRINT '  CORRIENDO ACTUALIZACION DE COLUMNAS';
PRINT '=========================================================';

-- =====================================================
-- Actualizar valores existentes
-- =====================================================

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
-- Verificar datos
-- =====================================================
PRINT '';
PRINT 'Datos en la tabla:';

SELECT 
    consecutive,
    fecha_hora_entrada,
    fecha_hora_salida,
    duracion_minutos,
    duracion_horas,
    monto_total_pagar
FROM PRQ_IngresoAutomoviles
WHERE fecha_hora_salida IS NOT NULL
ORDER BY consecutive;

-- =====================================================
-- Eliminar y recrear trigger
-- =====================================================
PRINT '';
PRINT 'Recreando trigger...';

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

PRINT 'Trigger recreado.';

-- =====================================================
-- Prueba del trigger
-- =====================================================
PRINT '';
PRINT 'Probando trigger...';

-- Insertar registro de prueba
INSERT INTO PRQ_IngresoAutomoviles (id_parqueo, id_automovil, fecha_hora_entrada, fecha_hora_salida)
VALUES (1, 1, '2026-04-15 08:00:00', '2026-04-15 12:00:00');

-- Verificar que el trigger actualizó los valores
SELECT 
    consecutive,
    duracion_minutos,
    duracion_horas,
    monto_total_pagar
FROM PRQ_IngresoAutomoviles
WHERE consecutive = SCOPE_IDENTITY();

PRINT '';
PRINT '=========================================================';
PRINT '  CORRECCION COMPLETADA';
PRINT '=========================================================';
GO