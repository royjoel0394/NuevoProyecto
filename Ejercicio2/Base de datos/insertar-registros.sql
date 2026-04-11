-- =====================================================
-- SCRIPT DE INSERCIÓN DE DATOS: Sistema de Parqueo
-- SQL Server
-- =====================================================
-- Descripción: Inserción de datos de prueba para el sistema de parqueo
-- Incluye automóviles, parqueos e ingresos/salidas

-- =====================================================
-- 1. INSERCIÓN DE AUTOMÓVILES
-- =====================================================

INSERT INTO PRQ_Automoviles (color, año, fabricante, tipo)
VALUES 
    ('Blanco', 2022, 'Toyota', 'sedán'),
    ('Negro', 2020, 'Honda', '4x4'),
    ('Rojo', 2023, 'Yamaha', 'moto'),
    ('Azul', 2021, 'Ford', 'hatchback'),
    ('Gris', 2019, 'Chevrolet', 'camioneta');

-- =====================================================
-- 2. INSERCIÓN DE PARQUEOS
-- =====================================================

INSERT INTO PRQ_Parqueo (nombre_de_provincia, nombre, precio_por_hora)
VALUES 
    ('San José', 'Centro Parqueo San José', 2.50),
    ('Heredia', 'Parqueo Norte Heredia', 2.00);

-- =====================================================
-- 3. INSERCIÓN DE INGRESOS DE AUTOMÓVILES
-- =====================================================

INSERT INTO PRQ_IngresoAutomoviles (id_parqueo, id_automovil, fecha_hora_entrada, fecha_hora_salida)
VALUES 
    -- Registros del 8 de abril (completados)
    (1, 1, '2026-04-08 08:30:00', '2026-04-08 11:45:00'),
    (1, 2, '2026-04-08 09:15:00', '2026-04-08 14:20:00'),
    (2, 3, '2026-04-08 10:00:00', '2026-04-08 10:45:00'),
    
    -- Registros del 9 de abril (completados)
    (1, 4, '2026-04-09 07:30:00', '2026-04-09 12:15:00'),
    (2, 5, '2026-04-09 08:00:00', '2026-04-09 16:30:00'),
    (1, 1, '2026-04-09 13:00:00', '2026-04-09 18:45:00'),
    
    -- Registros del 10 de abril (actuales - algunos sin salida)
    (2, 2, '2026-04-10 06:30:00', '2026-04-10 09:20:00'),
    (1, 3, '2026-04-10 08:15:00', NULL), -- Aún en el parqueo
    (2, 4, '2026-04-10 09:00:00', '2026-04-10 13:30:00'),
    (1, 5, '2026-04-10 10:00:00', NULL), -- Aún en el parqueo
    
    -- Registros adicionales del 7 de abril (pasado)
    (1, 1, '2026-04-07 15:30:00', '2026-04-07 18:00:00'),
    (2, 2, '2026-04-07 16:45:00', '2026-04-07 19:30:00'),
    
    -- Registros adicionales del 6 de abril (pasado)
    (1, 4, '2026-04-06 08:00:00', '2026-04-06 12:30:00'),
    (2, 3, '2026-04-06 10:15:00', '2026-04-06 11:00:00'),
    (1, 5, '2026-04-06 14:00:00', NULL); -- Aún en el parqueo (histórico)

-- =====================================================
-- RESUMEN DE DATOS INSERTADOS
-- =====================================================
-- Total de automóviles: 5
--   - 1 sedán (Toyota)
--   - 1 4x4 (Honda)
--   - 1 moto (Yamaha)
--   - 1 hatchback (Ford)
--   - 1 camioneta (Chevrolet)
--
-- Total de parqueos: 2
--   - Centro Parqueo San José (San José) - $2.50/hora
--   - Parqueo Norte Heredia (Heredia) - $2.00/hora
--
-- Total de registros de ingreso: 15
--   - Registros completados (con salida): 12
--   - Registros activos (sin salida): 3
-- =====================================================

-- Opcional: Ver los datos insertados
-- SELECT * FROM PRQ_Automoviles;
-- SELECT * FROM PRQ_Parqueo;
-- SELECT * FROM PRQ_IngresoAutomoviles ORDER BY fecha_hora_entrada DESC;