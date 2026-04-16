-- ====================================================================
-- Script SQL para verificar campos calculados en PRQ_IngresoAutomoviles
-- Compatible con SQL Server y usuario user01
-- ====================================================================

-- ====================================================================
-- 1. CREAR DATOS DE PRUEBA (5 registros: 3 completados, 2 activos)
-- ====================================================================

-- Limpiar datos existentes
DELETE FROM PRQ_IngresoAutomoviles;
DELETE FROM PRQ_Automoviles;
DELETE FROM PRQ_Parqueo;

-- Insertar Parqueos de prueba
INSERT INTO PRQ_Parqueo (id, nombre, nombre_de_provincia, precio_por_hora)
VALUES 
(1, 'Parqueo Central', 'San Jose', 10.00),
(2, 'Parqueo Norte', 'Alajuela', 15.00);

-- Insertar Automoviles de prueba
INSERT INTO PRQ_Automoviles (id, color, ano, fabricante, tipo)
VALUES
(1, 'Rojo', 2022, 'Toyota', 'Sedan'),
(2, 'Azul', 2023, 'Honda', 'SUV'),
(3, 'Verde', 2021, 'Ford', 'Camioneta');

-- Insertar Ingresos de prueba (3 completados, 2 activos)
SET IDENTITY_INSERT PRQ_IngresoAutomoviles ON;

INSERT INTO PRQ_IngresoAutomoviles (consecutivo, id_automovil, id_parqueo, fecha_hora_entrada, fecha_hora_salida)
VALUES
-- Completados (tienen salida)
(1, 1, 1, '2024-01-15 08:00:00', '2024-01-15 10:30:00'),  -- 150 min = 2.5 horas * 10 = 25
(2, 2, 1, '2024-01-15 12:00:00', '2024-01-15 14:00:00'),  -- 120 min = 2.0 horas * 10 = 20
(3, 3, 2, '2024-01-15 15:00:00', '2024-01-15 18:30:00'),  -- 210 min = 3.5 horas * 15 = 52.5
-- Activos (sin salida)
(4, 1, 2, '2024-01-16 09:00:00', NULL),  -- Still parked
(5, 2, 1, '2024-01-16 14:00:00', NULL);  -- Still parked

SET IDENTITY_INSERT PRQ_IngresoAutomoviles OFF;

PRINT 'Datos de prueba insertados: 3 completados, 2 activos';
GO

-- ====================================================================
-- 2. CREAR VISTA CON CAMPOS CALCULADOS
-- ====================================================================

IF OBJECT_ID('vw_IngresosConCamposCalculados', 'V') IS NOT NULL
    DROP VIEW vw_IngresosConCamposCalculados;
GO

CREATE VIEW vw_IngresosConCamposCalculados AS
SELECT 
    i.consecutivo,
    i.id_automovil,
    i.id_parqueo,
    i.fecha_hora_entrada,
    i.fecha_hora_salida,
    p.nombre AS nombre_parqueo,
    p.precio_por_hora,
    -- Calcular duracion en minutos
    CASE 
        WHEN i.fecha_hora_salida IS NULL THEN NULL
        ELSE DATEDIFF(MINUTE, i.fecha_hora_entrada, i.fecha_hora_salida)
    END AS duracion_minutos,
    -- Calcular duracion en horas (2 decimales)
    CASE 
        WHEN i.fecha_hora_salida IS NULL THEN NULL
        ELSE ROUND(DATEDIFF(SECOND, i.fecha_hora_entrada, i.fecha_hora_salida) / 3600.0, 2)
    END AS duracion_horas,
    -- Calcular monto total a pagar
    CASE 
        WHEN i.fecha_hora_salida IS NULL THEN NULL
        ELSE ROUND(
            (DATEDIFF(SECOND, i.fecha_hora_entrada, i.fecha_hora_salida) / 3600.0) * p.precio_por_hora
        , 2)
    END AS monto_total_pagar,
    -- Estado del ingreso
    CASE 
        WHEN i.fecha_hora_salida IS NULL THEN 'Activo'
        ELSE 'Completado'
    END AS estado
FROM PRQ_IngresoAutomoviles i
LEFT JOIN PRQ_Parqueo p ON i.id_parqueo = p.id;
GO

PRINT 'Vista vw_IngresosConCamposCalculados creada exitosamente';
GO

-- ====================================================================
-- 3. MOSTRAR RESULTADOS CON CAMPOS CALCULADOS
-- ====================================================================

PRINT '';
PRINT '============================================================';
PRINT 'CONSULTA: Todos los ingresos con campos calculados';
PRINT '============================================================';

SELECT 
    consecutivo AS [Consecutivo],
    fecha_hora_entrada AS [Fecha Entrada],
    fecha_hora_salida AS [Fecha Salida],
    duracion_minutos AS [Minutos],
    duracion_horas AS [Horas],
    monto_total_pagar AS [Monto],
    estado AS [Estado]
FROM vw_IngresosConCamposCalculados
ORDER BY consecutive;
GO

-- ====================================================================
-- 4. VALIDAR REGISTROS CON SALIDA NULL (Deben tener campos NULL)
-- ====================================================================

PRINT '';
PRINT '============================================================';
PRINT 'VALIDACION 1: Registros activos (salida NULL)';
PRINT '============================================================';

SELECT 
    consecutivo AS [Consecutivo],
    fecha_hora_entrada AS [Entrada],
    fecha_hora_salida AS [Salida],
    duracion_minutos AS [Minutos],
    duracion_horas AS [Horas],
    monto_total_pagar AS [Monto],
    estado AS [Estado],
    CASE 
        WHEN duracion_minutos IS NULL AND duracion_horas IS NULL AND monto_total_pagar IS NULL 
        THEN 'VALIDO' 
        ELSE 'ERROR - Deben ser NULL'
    END AS [Validacion]
FROM vw_IngresosConCamposCalculados
WHERE estado = 'Activo';
GO

-- ====================================================================
-- 5. VALIDAR REGISTROS CON SALIDA (Deben tener valores calculados)
-- ====================================================================

PRINT '';
PRINT '============================================================';
PRINT 'VALIDACION 2: Registros completados (con salida)';
PRINT '============================================================';

SELECT 
    consecutivo AS [Consecutivo],
    fecha_hora_entrada AS [Entrada],
    fecha_hora_salida AS [Salida],
    duracion_minutos AS [Minutos],
    duracion_horas AS [Horas],
    precio_por_hora AS [Precio/Hora],
    monto_total_pagar AS [Monto],
    estado AS [Estado],
    CASE 
        WHEN duracion_minutos > 0 AND duracion_horas > 0 AND monto_total_pagar > 0 
        THEN 'VALIDO' 
        ELSE 'ERROR - Deben tener valores'
    END AS [Validacion]
FROM vw_IngresosConCamposCalculados
WHERE estado = 'Completado';
GO

-- ====================================================================
-- 6. RESUMEN ESTADISTICO
-- ====================================================================

PRINT '';
PRINT '============================================================';
PRINT 'RESUMEN: Estadisticas por estado';
PRINT '============================================================';

SELECT 
    estado AS [Estado],
    COUNT(*) AS [Total Registros],
    SUM(duracion_minutos) AS [Total Minutos],
    AVG(duracion_horas) AS [Promedio Horas],
    SUM(monto_total_pagar) AS [Monto Total]
FROM vw_IngresosConCamposCalculados
GROUP BY estado;
GO

-- ====================================================================
-- 7. DETALLE DE CADA REGISTRO
-- ====================================================================

PRINT '';
PRINT '============================================================';
PRINT 'DETALLE: Registro por registro';
PRINT '============================================================';

SELECT 
    consecutive AS [Consecutivo],
    nombre_parqueo AS [Parqueo],
    precio_por_hora AS [Precio x Hora],
    fecha_hora_entrada AS [Entrada],
    fecha_hora_salida AS [Salida],
    duracion_minutos AS [Minutos],
    duracion_horas AS [Horas],
    monto_total_pagar AS [Total],
    estado AS [Estado]
FROM vw_IngresosConCamposCalculados
ORDER BY consecutive;
GO

-- ====================================================================
-- 8. LIMPIEZA (Opcional - descomentar si se desea)
-- ====================================================================

-- DELETE FROM PRQ_IngresoAutomoviles;
-- DELETE FROM PRQ_Automoviles;
-- DELETE FROM PRQ_Parqueo;

PRINT '';
PRINT '============================================================';
PRINT 'Script ejecutado exitosamente';
PRINT '============================================================';
GO
