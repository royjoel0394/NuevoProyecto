# Instrucciones de Pruebas de Integración

## Requisitos Previos

1. **Credenciales de SQL Server en la nube** (variables de entorno):
   ```powershell
   $env:SQL_SERVER_HOST = "hecferme-sqlserver.database.windows.net"
   $env:SQL_SERVER_DATABASE = "hecferme-sql-server"
   $env:SQL_SERVER_USER = "user01"
   $env:SQL_SERVER_PASSWORD = "MyVeryStr0ngPassw0rd*"
   ```

2. **Archivos JSON** en la ruta configurada (`appsettings.json`):
   - `automoviles.json`
   - `parqueos.json`
   - `ingresos.json`

---

## 1. Pruebas de Repositorios SQL (Instancia Real)

### 1.1 Pruebas de Consulta

| # | Método | Descripción | Resultado Esperado |
|---|--------|-------------|-------------------|
| 1 | GetAllAsync | Obtiene todos los automóviles | Lista completa de registros |
| 2 | GetByIdAsync(id) | Consulta por ID válido | Registro encontrado |
| 3 | GetByIdAsync(999) | Consulta por ID inexistente | Null |
| 4 | GetByColorAsync("Rojo") | Búsqueda por color | Automóviles rojos |
| 5 | GetByYearRangeAsync(2020,2023) | Búsqueda por rango de año | Automóviles en rango |
| 6 | GetByManufacturerAsync("Toyota") | Búsqueda por fabricante | Automóviles Toyota |
| 7 | GetByTypeAsync("Sedán") | Búsqueda por tipo | Automóviles tipo Sedán |

### 1.2 Pruebas de Escritura

| # | Método | Descripción | Resultado Esperado |
|---|--------|-------------|-------------------|
| 1 | InsertAsync | Insertar nuevo automóvil | ID automático generado |
| 2 | UpdateAsync | Modificar existentes | Cambio persistido |
| 3 | DeleteAsync | Eliminar registro | Registro eliminado |
| 4 | DeleteAsync (con ingresos) | Eliminar con FK | InvalidOperationException |

### 1.3 Prueba de Integridad Referencial

```csharp
// Test: No se puede eliminar automóvil con ingresos
[Fact]
public async Task DeleteAsync_ThrowsException_WhenAutomovilHasIngresos()
{
    // Arrange: Crear automóvil con ingreso
    var auto = new PrqAutomovile { Id = 1, Color = "Rojo", ... };
    var ingreso = new PrqIngresoAutomovile { Consecutivo = 1, IdAutomovil = 1, ... };
    
    // Act & Assert
    await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        await _repository.DeleteAsync(1));
}
```

---

## 2. Pruebas de UnitOfWork

### 2.1 Transacción Exitosa

```csharp
// Test: Múltiples operaciones en una sola transacción
[Fact]
public async Task SaveChangesAsync_CommitsMultipleOperations_WhenAllSuccessful()
{
    // Arrange
    var auto = TestDataBuilder.CreateAutomovil(1);
    var parqueo = TestDataBuilder.CreateParqueo(1);
    
    using var context = CreateRealContext();
    var unitOfWork = new SqlUnitOfWork(context, 
        new SqlAutomovilRepository(context),
        new SqlParqueoRepository(context),
        new SqlIngresoAutomovilRepository(context));

    // Act
    await unitOfWork.AutomovilRepository.InsertAsync(auto);
    await unitOfWork.ParqueoRepository.InsertAsync(parqueo);
    await unitOfWork.SaveChangesAsync();

    // Assert
    var autoResult = await context.PrqAutomoviles.FindAsync(1);
    var parqueoResult = await context.PrqParqueos.FindAsync(1);
    
    Assert.NotNull(autoResult);
    Assert.NotNull(parqueoResult);
}
```

### 2.2 Transacción Fallida (Rollback)

```csharp
// Test: Si una operación falla, todas hacen rollback
[Fact]
public async Task SaveChangesAsync_RollbackOnFailure()
{
    using var context = CreateRealContext();
    var unitOfWork = new SqlUnitOfWork(context, 
        new SqlAutomovilRepository(context),
        new SqlParqueoRepository(context),
        new SqlIngresoAutomovilRepository(context));

    // Insertar auto válido
    await unitOfWork.AutomovilRepository.InsertAsync(
        new PrqAutomovile { Id = 1, Color = "Rojo", ... });
    
    // Intentar insertar ingreso con FK inválida
    var ingresoInvalido = new PrqIngresoAutomovile 
    { 
        Consecutivo = 1, 
        IdAutomovil = 999, // No existe
        IdParqueo = 1 
    };
    
    // Act
    await unitOfWork.IngresoAutomovilRepository.InsertAsync(ingresoInvalido);
    
    // Assert: Verificar que el auto NO fue persistido (rollback)
    var autoResult = await context.PrqAutomoviles.FirstOrDefaultAsync();
    Assert.Null(autoResult);
}
```

### 2.3 Verificar Persistencia con Commit

```csharp
// Test: Los cambios solo persisten después de SaveChanges
[Fact]
public async Task SaveChangesAsync_PersistsOnlyAfterCommit()
{
    using var context = CreateRealContext();
    var unitOfWork = new SqlUnitOfWork(context, 
        new SqlAutomovilRepository(context),
        new SqlParqueoRepository(context),
        new SqlIngresoAutomovilRepository(context));

    var auto = new PrqAutomovile { Id = 100, Color = "Verde", ... };
    
    // Sin commit - no debe persistir
    await unitOfWork.AutomovilRepository.InsertAsync(auto);
    
    var countBefore = await context.PrqAutomoviles.CountAsync();
    
    // Con commit - debe persistir
    await unitOfWork.SaveChangesAsync();
    
    var countAfter = await context.PrqAutomoviles.CountAsync();
    
    Assert.Equal(countBefore, countAfter - 1);
}
```

---

## 3. Pruebas de Cambio JSON vs SQL

### 3.1 Configuración para Comparación

```csharp
// Test: Mismos datos, ambos proveedores
[Fact]
public async Task BothProviders_ReturnSameResults()
{
    // Arrange: Mismos datos en SQL
    var sqlRepo = CreateSqlAutomovilRepository();
    
    var jsonRepo = CreateJsonAutomovilRepository();
    
    // Act
    var sqlResult = await sqlRepo.GetAllAsync();
    var sqlById = await sqlRepo.GetByIdAsync(1);
    var sqlByColor = await sqlRepo.GetByColorAsync("Rojo");
    var sqlByYear = await sqlRepo.GetByYearRangeAsync(2020, 2023);
    
    var jsonResult = await jsonRepo.GetAllAsync();
    var jsonById = await jsonRepo.GetByIdAsync(1);
    var jsonByColor = await jsonRepo.GetByColorAsync("Rojo");
    var jsonByYear = await jsonRepo.GetByYearRangeAsync(2020, 2023);
    
    // Assert: Comparar resultados
    Assert.Equal(sqlResult.Count, jsonResult.Count);
    Assert.Equal(sqlById?.Id, jsonById?.Id);
    Assert.Equal(sqlByColor.Count, jsonByColor.Count);
    Assert.Equal(sqlByYear.Count, jsonByYear.Count);
}
```

### 3.2 Tabla Comparativa JSON vs SQL

| Método | SQL Resultado | JSON Resultado | Coincide |
|--------|---------------|----------------|----------|
| GetAllAsync | 5 autos | 5 autos | ✅ |
| GetByIdAsync(1) | Toyota Corolla | Toyota Corolla | ✅ |
| GetByColorAsync("Rojo") | 2 autos | 2 autos | ✅ |
| GetByYearRangeAsync(2020,2023) | 4 autos | 4 autos | ✅ |
| GetByManufacturerAsync("Honda") | 1 auto | 1 auto | ✅ |

---

## 4. Pruebas de Campos Personalizados

### 4.1 Pruebas de Cálculos con Nullable

```csharp
// Test: DuracionEstadiaMinutos cuando hay salida
[Fact]
public void DuracionEstadiaMinutos_ReturnsValue_WhenSalidaExists()
{
    var ingreso = new PrqIngresoAutomovile
    {
        Consecutivo = 1,
        IdAutomovil = 1,
        IdParqueo = 1,
        FechaHoraEntrada = DateTime.Now.AddHours(-2),
        FechaHoraSalida = DateTime.Now
    };
    
    // 2 horas = 120 minutos
    Assert.Equal(120, ingreso.DuracionEstadiaMinutos);
}

// Test: DuracionEstadiaMinutos cuando NO hay salida
[Fact]
public void DuracionEstadiaMinutos_ReturnsNull_WhenNoSalida()
{
    var ingreso = new PrqIngresoAutomovile
    {
        Consecutivo = 1,
        IdAutomovil = 1,
        IdParqueo = 1,
        FechaHoraEntrada = DateTime.Now
        // Sin FechaHoraSalida
    };
    
    Assert.Null(ingreso.DuracionEstadiaMinutos);
}

// Test: MontoTotalAPagar con datos completos
[Fact]
public void MontoTotalAPagar_ReturnsValue_WhenComplete()
{
    var ingreso = new PrqIngresoAutomovile
    {
        Consecutivo = 1,
        IdAutomovil = 1,
        IdParqueo = 1,
        FechaHoraEntrada = DateTime.Now.AddHours(-2),
        FechaHoraSalida = DateTime.Now,
        IdParqueoNavigation = new PrqParqueo { PrecioPorHora = 10.00m }
    };
    
    // 2 horas * 10 = 20
    Assert.Equal(20.00m, ingreso.MontoTotalAPagar);
}

// Test: MontoTotalAPagar cuando faltan datos
[Fact]
public void MontoTotalAPagar_ReturnsNull_WhenIncomplete()
{
    var ingreso = new PrqIngresoAutomovile
    {
        Consecutivo = 1,
        IdAutomovil = 1,
        IdParqueo = 1,
        FechaHoraEntrada = DateTime.Now
        // Sin salida ni navegación
    };
    
    Assert.Null(ingreso.MontoTotalAPagar);
}
```

---

## 5. Scripts de Ejecución

### 5.1 Ejecutar Todas las Pruebas SQL

```powershell
cd C:\Github\NuevoProyecto\Ejercicio2\Repositorio.Tests
dotnet test --filter "FullyQualifiedName~Sql" --verbosity normal
```

### 5.2 Ejecutar Solo UnitOfWork

```powershell
dotnet test --filter "FullyQualifiedName~UnitOfWork" --verbosity normal
```

### 5.3 Ejecutar Comparación JSON vs SQL

```powershell
dotnet test --filter "FullyQualifiedName~ProviderSwitching" --verbosity normal
```

### 5.4 Ejecutar Pruebas de Campos Calculados

```powershell
dotnet test --filter "FullyQualifiedName~CustomFields" --verbosity normal
```

---

## 6. Plantilla de Resultados

| Prueba | Categoría | Resultado | Evidencia |
|--------|-----------|-----------|-----------|
| GetAllAsync | Repositorio SQL | PASS/FAIL | Log: [...] |
| GetByIdAsync | Repositorio SQL | PASS/FAIL | Log: [...] |
| InsertAsync | Repositorio SQL | PASS/FAIL | ID generado: X |
| UpdateAsync | Repositorio SQL | PASS/FAIL | Log: [...] |
| DeleteAsync | Repositorio SQL | PASS/FAIL | Log: [...] |
| Transacción Exitosa | UnitOfWork | PASS/FAIL | Log: [...] |
| Transacción Fallida | UnitOfWork | PASS/FAIL | Log: [...] |
| GetAll JSON | Comparación | PASS/FAIL | SQL: X vs JSON: Y |
| GetByColor JSON | Comparación | PASS/FAIL | SQL: X vs JSON: Y |
| Duracion Null | Campos | PASS/FAIL | Valor: null |
| Duracion Valor | Campos | PASS/FAIL | Valor: 120 |
| Monto Null | Campos | PASS/FAIL | Valor: null |
| Monto Valor | Campos | PASS/FAIL | Valor: 20.00 |

---

## 7. Notas Importantes

1. **Las pruebas con InMemory** no pueden probar:
   - `DateDiffMinute` (solo SQL Server)
   - Transacciones reales
   - Integridad referencial real

2. **Para pruebas completas** es necesario:
   - Conexión a SQL Server real (azure/variable entorno)
   - Archivos JSON con datos de prueba válidos

3. **Recomendación**: Crear suite de integración separada con:
   - Base de datos de prueba en Azure
   - Datos de prueba reproducibles
   - Cleanup entre ejecuciones
