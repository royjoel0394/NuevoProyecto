-- =====================================================
-- EJEMPLOS DE CONSULTA CON CAMPOS CALCULADOS
-- =====================================================

USE [hecferme-sql-server];
GO

-- 1. Ver todos los ingresos con duración y monto calculado
SELECT
    consecutivo,
    id_parqueo,
    id_automovil,
    fecha_hora_entrada,
    fecha_hora_salida,
    DuracionEstadiaMinutos,
    DuracionEstadiaHoras,
    MontoTotalAPagar
FROM dbo.VW_PRQ_IngresoAutomovilesCalculado
ORDER BY consecutivo;

-- 2. Ver solo ingresos cerrados con pago calculado
SELECT
    consecutivo,
    id_parqueo,
    id_automovil,
    fecha_hora_entrada,
    fecha_hora_salida,
    DuracionEstadiaHoras,
    MontoTotalAPagar
FROM dbo.VW_PRQ_IngresoAutomovilesCalculado
WHERE fecha_hora_salida IS NOT NULL
ORDER BY fecha_hora_salida DESC;

-- 3. Ver ingresos aún activos (sin salida)
SELECT
    consecutivo,
    id_parqueo,
    id_automovil,
    fecha_hora_entrada,
    fecha_hora_salida,
    DuracionEstadiaMinutos,
    DuracionEstadiaHoras,
    MontoTotalAPagar
FROM dbo.VW_PRQ_IngresoAutomovilesCalculado
WHERE fecha_hora_salida IS NULL;

-- 4. Total cobrado por parqueo
SELECT
    id_parqueo,
    SUM(MontoTotalAPagar) AS TotalCobrado
FROM dbo.VW_PRQ_IngresoAutomovilesCalculado
WHERE MontoTotalAPagar IS NOT NULL
GROUP BY id_parqueo;

-- =====================================================
-- FIN DE EJEMPLOS
-- =====================================================