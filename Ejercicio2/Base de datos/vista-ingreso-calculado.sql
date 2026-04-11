-- =====================================================
-- VISTA SQL: VW_PRQ_IngresoAutomovilesCalculado
-- Incluye cálculos de duración y monto total a pagar
-- =====================================================

USE [hecferme-sql-server];
GO

IF OBJECT_ID('dbo.VW_PRQ_IngresoAutomovilesCalculado', 'V') IS NOT NULL
    DROP VIEW dbo.VW_PRQ_IngresoAutomovilesCalculado;
GO

CREATE VIEW dbo.VW_PRQ_IngresoAutomovilesCalculado
AS
SELECT
    i.consecutivo,
    i.id_parqueo,
    i.id_automovil,
    i.fecha_hora_entrada,
    i.fecha_hora_salida,
    p.precio_por_hora,
    CASE
        WHEN i.fecha_hora_salida IS NULL THEN NULL
        ELSE DATEDIFF(MINUTE, i.fecha_hora_entrada, i.fecha_hora_salida)
    END AS DuracionEstadiaMinutos,
    CASE
        WHEN i.fecha_hora_salida IS NULL THEN NULL
        ELSE ROUND(CAST(DATEDIFF(MINUTE, i.fecha_hora_entrada, i.fecha_hora_salida) AS DECIMAL(10,2)) / 60.0, 2)
    END AS DuracionEstadiaHoras,
    CASE
        WHEN i.fecha_hora_salida IS NULL THEN NULL
        ELSE ROUND(
            (CAST(DATEDIFF(MINUTE, i.fecha_hora_entrada, i.fecha_hora_salida) AS DECIMAL(10,2)) / 60.0)
            * p.precio_por_hora,
            2)
    END AS MontoTotalAPagar
FROM dbo.PRQ_IngresoAutomoviles AS i
INNER JOIN dbo.PRQ_Parqueo AS p
    ON i.id_parqueo = p.id;
GO

PRINT '✅ Vista VW_PRQ_IngresoAutomovilesCalculado creada correctamente.';
-- =====================================================
-- FIN DE LA VISTA
-- =====================================================