# Instrucciones de Pruebas - API REST con Proveedor JSON

## 1. Configuración Inicial

### 1.1 appsettings.json
```json
{
  "Repository": {
    "Provider": "Json",
    "JsonFilesPath": "..\\..\\Base de datos",
    "JsonAutomovilesFile": "PRQ_Automoviles.json",
    "JsonParqueoFile": "PRQ_Parqueo.json",
    "JsonIngresosFile": "PRQ_IngresoAutomoviles.json"
  }
}
```

### 1.2 IniciarAPI
```bash
cd C:\Github\NuevoProyecto\Ejercicio2\Repositorio
dotnet run
# API disponible en http://localhost:5000
```

---

## 2. Pruebas de Automóviles

### 2.1 GET - Obtener todos
- **Método**: GET
- **URL**: `http://localhost:5000/api/automoviles`
- **Expected**: 200 OK
- **Response**: Array con todos los automóviles

### 2.2 GET - Por ID
- **Método**: GET  
- **URL**: `http://localhost:5000/api/automoviles/1`
- **Expected**: 200 OK o 404

### 2.3 GET - Por Color
- **Método**: GET
- **URL**: `http://localhost:5000/api/automoviles/color/Rojo`
- **Expected**: 200 OK

### 2.4 GET - Por Rango Año
- **Método**: GET
- **URL**: `http://localhost:5000/api/automoviles/rango-anio?anoInicio=2020&anoFin=2023`
- **Expected**: 200 OK

### 2.5 GET - Por Fabricante
- **Método**: GET
- **URL**: `http://localhost:5000/api/automoviles/fabricante/Toyota`
- **Expected**: 200 OK

### 2.6 GET - Por Tipo
- **Método**: GET
- **URL**: `http://localhost:5000/api/automoviles/tipo/Sedan`
- **Expected**: 200 OK

### 2.7 POST - Crear Automóvil
- **Método**: POST
- **URL**: `http://localhost:5000/api/automoviles`
- **Body**:
```json
{
  "color": "Verde",
  "ano": 2024,
  "fabricante": "Tesla",
  "tipo": "Sedan"
}
```
- **Expected**: 201 Created

### 2.8 PUT - Actualizar Automóvil
- **Método**: PUT
- **URL**: `http://localhost:5000/api/automoviles/6`
- **Body**:
```json
{
  "color": "Verde",
  "ano": 2025,
  "fabricante": "Tesla",
  "tipo": "SUV"
}
```
- **Expected**: 200 OK

### 2.9 DELETE - Eliminar Automóvil
- **Método**: DELETE
- **URL**: `http://localhost:5000/api/automoviles/6`
- **Expected**: 204 No Content

---

## 3. Pruebas de Parqueo

### 3.1 GET - Obtener todos
- **Método**: GET
- **URL**: `http://localhost:5000/api/parqueos`
- **Expected**: 200 OK

### 3.2 GET - Por ID
- **Método**: GET
- **URL**: `http://localhost:5000/api/parqueos/1`
- **Expected**: 200 OK

### 3.3 GET - Por Provincia
- **Método**: GET
- **URL**: `http://localhost:5000/api/parqueos/provincia/San Jose`
- **Expected**: 200 OK

### 3.4 GET - Por Nombre
- **Método**: GET
- **URL**: `http://localhost:5000/api/parqueos/nombre/Central`
- **Expected**: 200 OK

### 3.5 GET - Por Rango Precio
- **Método**: GET
- **URL**: `http://localhost:5000/api/parqueos/rango-precio?min=5&max=20`
- **Expected**: 200 OK

### 3.6 POST - Crear Parqueo
- **Método**: POST
- **URL**: `http://localhost:5000/api/parqueos`
- **Body**:
```json
{
  "nombre": "Parqueo Norte",
  "nombreDeProvincia": "Alajuela",
  "precioPorHora": 15
}
```
- **Expected**: 201 Created

### 3.7 PUT - Actualizar Parqueo
- **Método**: PUT
- **URL**: `http://localhost:5000/api/parqueos/3`
- **Body**:
```json
{
  "nombre": "Parqueo Actualizado",
  "nombreDeProvincia": "San Jose",
  "precioPorHora": 20
}
```
- **Expected**: 200 OK

### 3.8 DELETE - Eliminar Parqueo
- **Método**: DELETE
- **URL**: `http://localhost:5000/api/parqueos/3`
- **Expected**: 204 No Content

---

## 4. Pruebas de Ingreso

### 4.1 GET - Obtener todos
- **Método**: GET
- **URL**: `http://localhost:5000/api/ingresos`
- **Expected**: 200 OK

### 4.2 GET - Por Consecutivo
- **Método**: GET
- **URL**: `http://localhost:5000/api/ingresos/1`
- **Expected**: 200 OK

### 4.3 GET - Precio por Parqueo
- **Método**: GET
- **URL**: `http://localhost:5000/api/ingresos/precio-parqueo/1`
- **Expected**: 200 OK

### 4.4 GET - Por Tipo con Fechas
- **Método**: GET
- **URL**: `http://localhost:5000/api/ingresos/por-tipo?tipoAutomovil=Sedan&desde=2023-01-01&hasta=2025-12-31`
- **Expected**: 200 OK

### 4.5 GET - Por Provincia con Fechas
- **Método**: GET
- **URL**: `http://localhost:5000/api/ingresos/por-provincia?provincia=San Jose&desde=2023-01-01&hasta=2025-12-31`
- **Expected**: 200 OK

### 4.6 POST - Crear Ingreso
- **Método**: POST
- **URL**: `http://localhost:5000/api/ingresos`
- **Body**:
```json
{
  "idAutomovil": 1,
  "idParqueo": 1,
  "fechaHoraEntrada": "2024-01-15T10:00:00"
}
```
- **Expected**: 201 Created

### 4.7 PUT - Actualizar Ingreso (registrar salida)
- **Método**: PUT
- **URL**: `http://localhost:5000/api/ingresos/16`
- **Body**:
```json
{
  "fechaHoraSalida": "2024-01-15T12:30:00"
}
```
- **Expected**: 200 OK

### 4.8 DELETE - Eliminar Ingreso
- **Método**: DELETE
- **URL**: `http://localhost:5000/api/ingresos/16`
- **Expected**: 204 No Content

---

## 5. Pruebas de Casos de Error

### 5.1 400 - Bad Request (campos vacíos)
```
POST /api/automoviles
Body: {"color":"","ano":2024,"fabricante":"","tipo":"Sedan"}
Expected: 400 - "Color, Fabricante y Tipo son requeridos"
```

### 5.2 404 - Not Found
```
GET /api/automoviles/999
Expected: 404 - "Automovil con ID 999 no encontrado"
```

### 5.3 409 - Conflicto (eliminar con ingresos)
```
DELETE /api/automoviles/1 (si tiene ingresos asociados)
Expected: 409 - "No se puede eliminar..."
```

---

## 6. Verificación de Persistencia

### 6.1 Verificar que archivos JSON se actualizan
1. Ejecutar POST para crear nuevo registro
2. Verificar en archivo PRQ_Automoviles.json que aparece el nuevo registro
3. Detener API (Ctrl+C)
4. Iniciar API nuevamente
5. Ejecutar GET - el registro debe persistir

### 6.2 Verificar estructura de archivos JSON
```bash
# Después de POST Automóvil:
cat PRQ_Automoviles.json
# Debe mostrar nuevo registro con Id, Color, Ano, Fabricante, Tipo
```

---

## 7. Captura de Evidencia

### Tabla de Resultados
| Endpoint | Metodo | Input | Codigo | Response | Resultado |
|----------|--------|-------|-------|----------|----------|
| /api/automoviles | GET | - | 200 | [...] | PASS |
| /api/automoviles/1 | GET | - | 200 | {...} | PASS |
| /api/automoviles | POST | {...} | 201 | {...} | PASS |
| ... | ... | ... | ... | ... | ... |

---

## 8. Comandos curl para validación rápida

```bash
# GET todos automoviles
curl http://localhost:5000/api/automoviles

# POST nuevo automovil
curl -X POST http://localhost:5000/api/automoviles -H "Content-Type: application/json" -d "{\"color\":\"Rosa\",\"ano\":2024,\"fabricante\":\"Kia\",\"tipo\":\"Sedan\"}"

# PUT actualizar
curl -X PUT http://localhost:5000/api/automoviles/7 -H "Content-Type: application/json" -d "{\"color\":\"Rosa\",\"ano\":2025,\"fabricante\":\"Kia\",\"tipo\":\"SUV\"}"

# DELETE
curl -X DELETE http://localhost:5000/api/automoviles/7
```