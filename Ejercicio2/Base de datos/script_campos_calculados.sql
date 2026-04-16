-- =====================================================
-- SCRIPT: Verificar campos personalizados en PRQ_IngresoAutomoviles
-- SQL Server - user01
-- =====================================================

USE [hecferme-sql-server];
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =====================================================
-- PARTE 0: ASEGURAR DATOS DE PRUEBA (mínimo 5 registros: 3 completados, 2 activos)
-- =====================================================
PRINT '📌 PARTE 0: Verificando datos de prueba';
PRINT '---------------------------------------------------------';

DECLARE @total INT;
SELECT @total = COUNT(*) FROM PRQ_IngresoAutomoviles;
PRINT 'Total de registros: ' + CAST(@total AS VARCHAR(10));

IF @total < 5
BEGIN
    INSERT INTO PRQ_IngresoAutomoviles (id_parqueo, id_automovil, fecha_hora_entrada, fecha_hora_salida)
    VALUES (1, 1, '2026-04-15 09:00:00', NULL);
END

IF NOT EXISTS (SELECT 1 FROM PRQ_IngresoAutomoviles WHERE fecha_hora_salida IS NULL)
BEGIN
    INSERT INTO PRQ_IngresoAutomoviles (id_parqueo, id_automovil, fecha_hora_entrada, fecha_hora_salida)
    VALUES (1, 4, '2026-04-15 10:30:00', NULL);
END
GO

PRINT '=========================================================';
PRINT '  VERIFICACIÓN DE CAMPOS CALCULADOS';
PRINT '  PRQ_IngresoAutomoviles';
PRINT '=========================================================';
PRINT '';

-- =====================================================
-- PARTE 1: CREAR VISTA CON LOS 3 CAMPOS CALCULADOS
-- =====================================================
PRINT '📌 PARTE 1: Creando vista PRQ_Vista_Ingresos_Calculados';
PRINT '---------------------------------------------------------';
GO

IF OBJECT_ID('dbo.PRQ_Vista_Ingresos_Calculados', 'V') IS NOT NULL
    DROP VIEW dbo.PRQ_Vista_Ingresos_Calculados;
GO

CREATE VIEW PRQ_Vista_Ingresos_Calculados
AS
SELECT 
    i.consecutivo,
    i.id_parqueo,
    p.nombre AS nombre_parqueo,
    i.id_automovil,
    a.fabricante,
    a.tipo,
    a.color,
    i.fecha_hora_entrada,
    i.fecha_hora_salida,
    
    -- Campo 1: duracion_minutos
    CASE 
        WHEN i.fecha_hora_salida IS NULL THEN NULL
        ELSE DATEDIFF(MINUTE, i.fecha_hora_entrada, i.fecha_hora_salida)
    END AS duracion_minutos,
    
    -- Campo 2: duracion_horas (2 decimales)
    CASE 
        WHEN i.fecha_hora_salida IS NULL THEN NULL
        ELSE ROUND(CAST(DATEDIFF(MINUTE, i.fecha_hora_entrada, i.fecha_hora_salida) AS FLOAT) / 60, 2)
    END AS duracion_horas,
    
    -- Campo 3: monto_total_pagar
    CASE 
        WHEN i.fecha_hora_salida IS NULL THEN NULL
        ELSE ROUND(
            (CAST(DATEDIFF(MINUTE, i.fecha_hora_entrada, i.fecha_hora_salida) AS FLOAT) / 60) 
            * p.precio_por_hora, 
        2)
    END AS monto_total_pagar,
    
    -- Estado del ingreso
    CASE 
        WHEN i.fecha_hora_salida IS NULL THEN 'Activo'
        ELSE 'Completado'
    END AS estado
    
FROM PRQ_IngresoAutomoviles i
INNER JOIN PRQ_Parqueo p ON i.id_parqueo = p.id
INNER JOIN PRQ_Automoviles a ON i.id_automovil = a.id;
GO

PRINT '✅ Vista creada exitosamente.';
PRINT '';
GO

-- =====================================================
-- PARTE 2: MOSTRAR TODOS LOS REGISTROS CON CAMPOS CALCULADOS
-- =====================================================
PRINT '📌 PARTE 2: Todos los registros con campos calculados';
PRINT '---------------------------------------------------------';

SELECT 
    consecutivo AS [Consecutivo],
    fecha_hora_entrada AS [Fecha Entrada],
    fecha_hora_salida AS [Fecha Salida],
    duracion_minutos AS [Minutos],
    duracion_horas AS [Horas],
    monto_total_pagar AS [Monto $],
    estado AS [Estado]
FROM PRQ_Vista_Ingresos_Calculados
ORDER BY consecutivo;
GO

-- =====================================================
-- PARTE 3: VALIDAR REGISTROS CON salida NULL (Deben tener campos NULL)
-- =====================================================
PRINT '';
PRINT '📌 PARTE 3: Validar registros con fecha_hora_salida = NULL';
PRINT '---------------------------------------------------------';

SELECT 
    consecutivo AS [Consecutivo],
    fecha_hora_entrada AS [Fecha Entrada],
    fecha_hora_salida AS [Fecha Salida],
    duracion_minutos AS [Duracion Minutos],
    duracion_horas AS [Duracion Horas],
    monto_total_pagar AS [Monto Pagar],
    estado AS [Estado]
FROM PRQ_Vista_Ingresos_Calculados
WHERE fecha_hora_salida IS NULL
ORDER BY consecutivo;

SELECT 
    'Validación' AS Resultado,
    CASE 
        WHEN COUNT(*) = 0 THEN '✅ PASADA: Todos los activos tienen campos NULL'
        ELSE '❌ FALLIDA: Hay activos con valores'
    END AS Mensaje
FROM PRQ_Vista_Ingresos_Calculados
WHERE fecha_hora_salida IS NULL 
  AND (duracion_minutos IS NOT NULL OR duracion_horas IS NOT NULL OR monto_total_pagar IS NOT NULL);
GO

-- =====================================================
-- PARTE 4: VALIDAR REGISTROS CON salida NO NULA (Deben tener valores calculados)
-- =====================================================
PRINT '';
PRINT '📌 PARTE 4: Validar registros con fecha_hora_salida NOT NULL';
PRINT '---------------------------------------------------------';

SELECT 
    consecutivo AS [Consecutivo],
    fecha_hora_entrada AS [Fecha Entrada],
    fecha_hora_salida AS [Fecha Salida],
    duracion_minutos AS [Duracion Minutos],
    duracion_horas AS [Duracion Horas],
    monto_total_pagar AS [Monto Pagar],
    estado AS [Estado]
FROM PRQ_Vista_Ingresos_Calculados
WHERE fecha_hora_salida IS NOT NULL
ORDER BY consecutivo;

SELECT 
    'Validación' AS Resultado,
    CASE 
        WHEN COUNT(*) = 0 THEN '✅ PASADA: Todos los completados tienen valores calculados'
        ELSE '❌ FALLIDA: Hay completados con valores nulos'
    END AS Mensaje
FROM PRQ_Vista_Ingresos_Calculados
WHERE fecha_hora_salida IS NOT NULL 
  AND (duracion_minutos IS NULL OR duracion_horas IS NULL OR monto_total_pagar IS NULL);
GO

-- =====================================================
-- PARTE 5: RESUMEN ESTADÍSTICO
-- =====================================================
PRINT '';
PRINT '📌 PARTE 5: Resumen Estadístico';
PRINT '---------------------------------------------------------';

SELECT 
    COUNT(*) AS [Total Registros],
    SUM(CASE WHEN estado = 'Activo' THEN 1 ELSE 0 END) AS [Registros Activos],
    SUM(CASE WHEN estado = 'Completado' THEN 1 ELSE 0 END) AS [Registros Completados],
    SUM(CASE WHEN estado = 'Completado' THEN duracion_minutos ELSE 0 END) AS [Total Minutos],
    SUM(CASE WHEN estado = 'Completado' THEN duracion_horas ELSE 0 END) AS [Total Horas],
    SUM(CASE WHEN estado = 'Completado' THEN monto_total_pagar ELSE 0 END) AS [Monto Total $]
FROM PRQ_Vista_Ingresos_Calculados;
GO

-- =====================================================
-- PARTE 6: DETALLE DE LOS ÚLTIMOS 5 REGISTROS
-- =====================================================
PRINT '';
PRINT '📌 PARTE 6: Detalle de los últimos 5 registros';
PRINT '---------------------------------------------------------';

SELECT TOP 5
    consecutivo AS [Cons.],
    CONVERT(VARCHAR, fecha_hora_entrada, 120) AS [Entrada],
    CONVERT(VARCHAR, fecha_hora_salida, 120) AS [Salida],
    duracion_minutos AS [Min],
    duracion_horas AS [Horas],
    '$' + CAST(monto_total_pagar AS VARCHAR(10)) AS [Monto],
    estado AS [Estado]
FROM PRQ_Vista_Ingresos_Calculados
ORDER BY consecutivo DESC;
GO

PRINT '';
PRINT '=========================================================';
PRINT '  ✅ SCRIPT EJECUTADO CORRECTAMENTE';
PRINT '=========================================================';
GO