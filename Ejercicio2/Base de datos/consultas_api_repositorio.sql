-- =====================================================
-- SCRIPT: Consultas para API repositorio
-- SQL Server - user01
-- =====================================================

USE [hecferme-sql-server];
GO
SET QUOTED_IDENTIFIER ON;
GO

PRINT '=========================================================';
PRINT '  CONSULTAS PARA API REPOSITORIO';
PRINT '=========================================================';
PRINT '';

-- =====================================================
-- 1. PRQ_AUTOMOVILES
-- =====================================================
PRINT '=========================================================';
PRINT '  1. PRQ_AUTOMOVILES';
PRINT '=========================================================';

-- ----------------------------------------------------
-- 1.1 Busqueda por ID (llave primaria)
-- ----------------------------------------------------
PRINT '';
PRINT '--- 1.1 Busqueda por ID ---';

SELECT * FROM PRQ_Automoviles WHERE id = 1;

-- ----------------------------------------------------
-- 1.2 Busqueda por color (busqueda parcial)
-- ----------------------------------------------------
PRINT '';
PRINT '--- 1.2 Busqueda por color (LIKE) ---';

SELECT * FROM PRQ_Automoviles WHERE color LIKE '%Rojo%';

-- ----------------------------------------------------
-- 1.3 Busqueda por intervalo de ano (desde - hasta)
-- ----------------------------------------------------
PRINT '';
PRINT '--- 1.3 Busqueda por intervalo de ano ---';

SELECT * FROM PRQ_Automoviles WHERE id BETWEEN 1 AND 3;

-- ----------------------------------------------------
-- 1.4 Busqueda por fabricante (busqueda parcial)
-- ----------------------------------------------------
PRINT '';
PRINT '--- 1.4 Busqueda por fabricante (LIKE) ---';

SELECT * FROM PRQ_Automoviles WHERE fabricante LIKE '%Toyota%';

-- ----------------------------------------------------
-- 1.5 Busqueda por tipo (busqueda parcial)
-- ----------------------------------------------------
PRINT '';
PRINT '--- 1.5 Busqueda por tipo (LIKE) ---';

SELECT * FROM PRQ_Automoviles WHERE tipo LIKE '%sedan%';

-- =====================================================
-- 2. PRQ_PARQUEO
-- =====================================================
PRINT '';
PRINT '=========================================================';
PRINT '  2. PRQ_PARQUEO';
PRINT '=========================================================';

-- ----------------------------------------------------
-- 2.1 Busqueda por ID (llave primaria)
-- ----------------------------------------------------
PRINT '';
PRINT '--- 2.1 Busqueda por ID ---';

SELECT * FROM PRQ_Parqueo WHERE id = 1;

-- ----------------------------------------------------
-- 2.2 Busqueda por provincia (busqueda parcial)
-- ----------------------------------------------------
PRINT '';
PRINT '--- 2.2 Busqueda por provincia (LIKE) ---';

SELECT * FROM PRQ_Parqueo WHERE nombre_provincia LIKE '%San%';

-- ----------------------------------------------------
-- 2.3 Busqueda por nombre del parqueo (busqueda parcial)
-- ----------------------------------------------------
PRINT '';
PRINT '--- 2.3 Busqueda por nombre (LIKE) ---';

SELECT * FROM PRQ_Parqueo WHERE nombre LIKE '%Parqueo%';

-- ----------------------------------------------------
-- 2.4 Busqueda por rango de precios (minimo - maximo)
-- ----------------------------------------------------
PRINT '';
PRINT '--- 2.4 Busqueda por rango de precios ---';

SELECT * FROM PRQ_Parqueo WHERE precio_por_hora BETWEEN 1.00 AND 3.00;

-- =====================================================
-- 3. PRQ_INGRESOAUTOMOVILES
-- =====================================================
PRINT '';
PRINT '=========================================================';
PRINT '  3. PRQ_INGRESOAUTOMOVILES';
PRINT '=========================================================';

-- ----------------------------------------------------
-- 3.1 Busqueda por ID (llave primaria)
-- ----------------------------------------------------
PRINT '';
PRINT '--- 3.1 Busqueda por ID ---';

SELECT * FROM PRQ_IngresoAutomoviles WHERE consecutivo = 1;

-- ----------------------------------------------------
-- CONSULTA A: Por tipo de automata en intervalo de fechas
-- ----------------------------------------------------
PRINT '';
PRINT '--- CONSULTA A: Por tipo de automata en intervalo de fechas ---';

SELECT 
    a.tipo AS tipo_automovil,
    i.fecha_hora_entrada AS hora_entrada,
    i.fecha_hora_salida AS hora_salida,
    CASE 
        WHEN i.fecha_hora_salida IS NULL THEN 'Pendiente'
        ELSE CAST(ROUND(
            (CAST(DATEDIFF(MINUTE, i.fecha_hora_entrada, i.fecha_hora_salida) AS FLOAT) / 60) * p.precio_por_hora,
            2) AS VARCHAR(20))
    END AS monto_a_cancelar
FROM PRQ_IngresoAutomoviles i
INNER JOIN PRQ_Automoviles a ON i.id_automovil = a.id
INNER JOIN PRQ_Parqueo p ON i.id_parqueo = p.id
WHERE a.tipo LIKE '%sedan%'
  AND i.fecha_hora_entrada >= '2023-10-15'
  AND i.fecha_hora_entrada < '2023-10-18'
ORDER BY i.fecha_hora_entrada;

-- ----------------------------------------------------
-- CONSULTA B: Por provincia del parqueo en intervalo de fechas
-- ----------------------------------------------------
PRINT '';
PRINT '--- CONSULTA B: Por provincia del parqueo en intervalo de fechas ---';

SELECT 
    p.nombre AS nombre_parqueo,
    p.nombre_provincia AS provincia,
    i.fecha_hora_entrada AS hora_entrada,
    i.fecha_hora_salida AS hora_salida,
    CASE 
        WHEN i.fecha_hora_salida IS NULL THEN NULL
        ELSE ROUND(
            (CAST(DATEDIFF(MINUTE, i.fecha_hora_entrada, i.fecha_hora_salida) AS FLOAT) / 60) * p.precio_por_hora,
        2)
    END AS monto_a_cancelar
FROM PRQ_IngresoAutomoviles i
INNER JOIN PRQ_Parqueo p ON i.id_parqueo = p.id
WHERE p.nombre_provincia LIKE '%San%'
  AND i.fecha_hora_entrada >= '2023-10-15'
  AND i.fecha_hora_entrada < '2023-10-18'
ORDER BY i.fecha_hora_entrada;

-- =====================================================
-- RESUMEN DE CONSULTAS CREADAS
-- =====================================================
PRINT '';
PRINT '=========================================================';
PRINT '  RESUMEN DE CONSULTAS';
PRINT '=========================================================';
PRINT '';
PRINT 'PRQ_Automoviles: 5 consultas (PK + 4 filtros)';
PRINT 'PRQ_Parqueo: 4 consultas (PK + 3 filtros)';
PRINT 'PRQ_IngresoAutomoviles: 3 consultas (PK + 2 avanzadas)';
PRINT '';
PRINT '=========================================================';
PRINT '  CONSULTAS EJECUTADAS CORRECTAMENTE';
PRINT '=========================================================';
GO