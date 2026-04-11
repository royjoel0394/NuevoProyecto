-- =====================================================
-- SCRIPT DE BASE DE DATOS: Sistema de Parqueo
-- SQL Server
-- =====================================================
-- Descripción: Modelo de datos para gestionar un parqueo de automóviles
-- Incluye tablas de automóviles, ubicaciones de parqueo y registro de ingresos/salidas
-- =====================================================

-- Tabla de Automóviles
CREATE TABLE PRQ_Automoviles (
    id INT IDENTITY(1,1) PRIMARY KEY,
    color NVARCHAR(50) NOT NULL,
    año INT NOT NULL,
    fabricante NVARCHAR(100) NOT NULL,
    tipo NVARCHAR(50) NOT NULL -- Ejemplos: sedán, 4x4, moto
);

-- Tabla de Parqueos (Ubicaciones)
CREATE TABLE PRQ_Parqueo (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre_de_provincia NVARCHAR(100) NOT NULL,
    nombre NVARCHAR(100) NOT NULL,
    precio_por_hora DECIMAL(10,2) NOT NULL
);

-- Tabla de Ingreso de Automóviles (Registro de entrada y salida)
CREATE TABLE PRQ_IngresoAutomoviles (
    consecutivo INT IDENTITY(1,1) PRIMARY KEY,
    id_parqueo INT NOT NULL,
    id_automovil INT NOT NULL,
    fecha_hora_entrada DATETIME NOT NULL,
    fecha_hora_salida DATETIME NULL,
    -- Definición de llaves foráneas
    CONSTRAINT FK_IngresoAutomoviles_Parqueo FOREIGN KEY (id_parqueo) 
        REFERENCES PRQ_Parqueo(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_IngresoAutomoviles_Automoviles FOREIGN KEY (id_automovil) 
        REFERENCES PRQ_Automoviles(id) ON DELETE RESTRICT ON UPDATE CASCADE
);

-- =====================================================
-- Índices para mejorar el rendimiento
-- =====================================================

-- Índice para búsquedas rápidas por tipo de automóvil
CREATE INDEX IDX_Automoviles_Tipo ON PRQ_Automoviles(tipo);

-- Índice para búsquedas por provincia
CREATE INDEX IDX_Parqueo_Provincia ON PRQ_Parqueo(nombre_de_provincia);

-- Índice para búsquedas de ingresos activos (sin salida)
CREATE INDEX IDX_IngresoAutomoviles_SinSalida 
    ON PRQ_IngresoAutomoviles(id_automovil, fecha_hora_salida) 
    WHERE fecha_hora_salida IS NULL;

-- =====================================================
-- Comentarios de las tablas (SQL Server Extended Properties)
-- =====================================================

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Registro de automóviles disponibles en el parqueo',
    @level0type = N'SCHEMA', @level0name = 'dbo',
    @level1type = N'TABLE', @level1name = 'PRQ_Automoviles';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Ubicaciones de parqueo con información de provincia y tarifa',
    @level0type = N'SCHEMA', @level0name = 'dbo',
    @level1type = N'TABLE', @level1name = 'PRQ_Parqueo';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Registro de entrada y salida de automóviles en las diferentes ubicaciones de parqueo',
    @level0type = N'SCHEMA', @level0name = 'dbo',
    @level1type = N'TABLE', @level1name = 'PRQ_IngresoAutomoviles';

-- =====================================================
-- FIN DEL SCRIPT
-- =====================================================