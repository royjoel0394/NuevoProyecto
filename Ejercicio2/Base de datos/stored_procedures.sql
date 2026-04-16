-- =====================================================
-- STORED PROCEDURES - CRUD Sistema de Parqueo
-- SQL Server - user01
-- =====================================================

USE [hecferme-sql-server];
GO
SET QUOTED_IDENTIFIER ON;
GO

PRINT '=========================================================';
PRINT '  CREANDO STORED PROCEDURES';
PRINT '=========================================================';

-- =====================================================
-- PRQ_AUTOMOVILES
-- =====================================================

IF OBJECT_ID('dbo.sp_Automovil_GetAll', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Automovil_GetAll;
GO

CREATE PROCEDURE sp_Automovil_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id, color, año, fabricante, tipo FROM PRQ_Automoviles ORDER BY id;
END;
GO

IF OBJECT_ID('dbo.sp_Automovil_GetById', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Automovil_GetById;
GO

CREATE PROCEDURE sp_Automovil_GetById
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id, color, año, fabricante, tipo FROM PRQ_Automoviles WHERE id = @id;
END;
GO

IF OBJECT_ID('dbo.sp_Automovil_Insert', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Automovil_Insert;
GO

CREATE PROCEDURE sp_Automovil_Insert
    @color NVARCHAR(50),
    @año INT,
    @fabricante NVARCHAR(50),
    @tipo NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO PRQ_Automoviles (color, año, fabricante, tipo)
    VALUES (@color, @año, @fabricante, @tipo);
    SELECT SCOPE_IDENTITY() AS id;
END;
GO

IF OBJECT_ID('dbo.sp_Automovil_Update', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Automovil_Update;
GO

CREATE PROCEDURE sp_Automovil_Update
    @id INT,
    @color NVARCHAR(50),
    @año INT,
    @fabricante NVARCHAR(50),
    @tipo NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE PRQ_Automoviles 
    SET color = @color, año = @año, fabricante = @fabricante, tipo = @tipo
    WHERE id = @id;
END;
GO

IF OBJECT_ID('dbo.sp_Automovil_Delete', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Automovil_Delete;
GO

CREATE PROCEDURE sp_Automovil_Delete
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM PRQ_IngresoAutomoviles WHERE id_automovil = @id)
    BEGIN
        RAISERROR('El automóvil tiene ingresos asociados', 16, 1);
        RETURN;
    END
    DELETE FROM PRQ_Automoviles WHERE id = @id;
END;
GO

-- =====================================================
-- PRQ_PARQUEO
-- =====================================================

IF OBJECT_ID('dbo.sp_Parqueo_GetAll', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Parqueo_GetAll;
GO

CREATE PROCEDURE sp_Parqueo_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id, nombre_provincia, nombre, precio_por_hora FROM PRQ_Parqueo ORDER BY id;
END;
GO

IF OBJECT_ID('dbo.sp_Parqueo_GetById', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Parqueo_GetById;
GO

CREATE PROCEDURE sp_Parqueo_GetById
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id, nombre_provincia, nombre, precio_por_hora FROM PRQ_Parqueo WHERE id = @id;
END;
GO

IF OBJECT_ID('dbo.sp_Parqueo_Insert', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Parqueo_Insert;
GO

CREATE PROCEDURE sp_Parqueo_Insert
    @nombre_provincia NVARCHAR(50),
    @nombre NVARCHAR(50),
    @precio_por_hora DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO PRQ_Parqueo (nombre_provincia, nombre, precio_por_hora)
    VALUES (@nombre_provincia, @nombre, @precio_por_hora);
    SELECT SCOPE_IDENTITY() AS id;
END;
GO

IF OBJECT_ID('dbo.sp_Parqueo_Update', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Parqueo_Update;
GO

CREATE PROCEDURE sp_Parqueo_Update
    @id INT,
    @nombre_provincia NVARCHAR(50),
    @nombre NVARCHAR(50),
    @precio_por_hora DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE PRQ_Parqueo 
    SET nombre_provincia = @nombre_provincia, nombre = @nombre, precio_por_hora = @precio_por_hora
    WHERE id = @id;
END;
GO

IF OBJECT_ID('dbo.sp_Parqueo_Delete', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Parqueo_Delete;
GO

CREATE PROCEDURE sp_Parqueo_Delete
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM PRQ_IngresoAutomoviles WHERE id_parqueo = @id)
    BEGIN
        RAISERROR('El parqueo tiene ingresos asociados', 16, 1);
        RETURN;
    END
    DELETE FROM PRQ_Parqueo WHERE id = @id;
END;
GO

-- =====================================================
-- PRQ_INGRESOAUTOMOVILES
-- =====================================================

IF OBJECT_ID('dbo.sp_Ingreso_GetAll', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Ingreso_GetAll;
GO

CREATE PROCEDURE sp_Ingreso_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM PRQ_IngresoAutomoviles ORDER BY consecutive;
END;
GO

IF OBJECT_ID('dbo.sp_Ingreso_GetById', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Ingreso_GetById;
GO

CREATE PROCEDURE sp_Ingreso_GetById
    @consecutivo INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM PRQ_IngresoAutomoviles WHERE consecutivo = @consecutivo;
END;
GO

IF OBJECT_ID('dbo.sp_Ingreso_Insert', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Ingreso_Insert;
GO

CREATE PROCEDURE sp_Ingreso_Insert
    @id_parqueo INT,
    @id_automovil INT,
    @fecha_hora_entrada DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET @fecha_hora_entrada = ISNULL(@fecha_hora_entrada, GETDATE());
    INSERT INTO PRQ_IngresoAutomoviles (id_parqueo, id_automovil, fecha_hora_entrada)
    VALUES (@id_parqueo, @id_automovil, @fecha_hora_entrada);
    SELECT SCOPE_IDENTITY() AS consecutivo;
END;
GO

IF OBJECT_ID('dbo.sp_Ingreso_UpdateSalida', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Ingreso_UpdateSalida;
GO

CREATE PROCEDURE sp_Ingreso_UpdateSalida
    @consecutivo INT,
    @fecha_hora_salida DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @fecha_entrada DATETIME;
    SELECT @fecha_entrada = fecha_hora_entrada FROM PRQ_IngresoAutomoviles WHERE consecutivo = @consecutivo;
    
    IF @fecha_entrada IS NULL
    BEGIN
        RAISERROR('Ingreso no encontrado', 16, 1);
        RETURN;
    END
    
    IF @fecha_hora_salida <= @fecha_entrada
    BEGIN
        RAISERROR('La fecha de salida debe ser posterior a la entrada', 16, 1);
        RETURN;
    END
    
    UPDATE PRQ_IngresoAutomoviles 
    SET fecha_hora_salida = @fecha_hora_salida
    WHERE consecutivo = @consecutivo;
END;
GO

IF OBJECT_ID('dbo.sp_Ingreso_Delete', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Ingreso_Delete;
GO

CREATE PROCEDURE sp_Ingreso_Delete
    @consecutivo INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM PRQ_IngresoAutomoviles WHERE consecutivo = @consecutivo;
END;
GO

-- =====================================================
-- EJEMPLOS DE EJECUCION
-- =====================================================
PRINT '';
PRINT '=========================================================';
PRINT '  EJEMPLOS DE EJECUCION';
PRINT '=========================================================';
PRINT '';
PRINT '-- Automoviles';
PRINT 'EXEC sp_Automovil_GetAll;';
PRINT 'EXEC sp_Automovil_GetById @id = 1;';
PRINT "EXEC sp_Automovil_Insert @color = 'Rojo', @año = 2024, @fabricante = 'Toyota', @tipo = 'sedan';";
PRINT 'EXEC sp_Automovil_Update @id = 1, @color = "Azul", @año = 2023, @fabricante = "Honda", @tipo = "suv";';
PRINT 'EXEC sp_Automovil_Delete @id = 1;';
PRINT '';
PRINT '-- Parqueos';
PRINT 'EXEC sp_Parqueo_GetAll;';
PRINT 'EXEC sp_Parqueo_GetById @id = 1;';
PRINT "EXEC sp_Parqueo_Insert @nombre_provincia = 'San Jose', @nombre = 'Parqueo Central', @precio_por_hora = 2.50;";
PRINT 'EXEC sp_Parqueo_Update @id = 1, @nombre_provincia = "San Jose", @nombre = "Parqueo Central", @precio_por_hora = 3.00;';
PRINT 'EXEC sp_Parqueo_Delete @id = 1;';
PRINT '';
PRINT '-- Ingresos';
PRINT 'EXEC sp_Ingreso_GetAll;';
PRINT 'EXEC sp_Ingreso_GetById @consecutivo = 1;';
PRINT 'EXEC sp_Ingreso_Insert @id_parqueo = 1, @id_automovil = 1;';
PRINT "EXEC sp_Ingreso_UpdateSalida @consecutivo = 1, @fecha_hora_salida = '2024-01-01 12:00:00';";
PRINT 'EXEC sp_Ingreso_Delete @consecutivo = 1;';
PRINT '';
PRINT '=========================================================';
PRINT '  STORED PROCEDURES CREADOS';
PRINT '=========================================================';
GO