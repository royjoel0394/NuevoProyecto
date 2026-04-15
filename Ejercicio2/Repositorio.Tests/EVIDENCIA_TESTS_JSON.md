# Evidencia de Pruebas API REST - Proveedor JSON

## Configuración
- **Provider**: Json
- **Base URL**: http://localhost:5000
- **Fecha**: 2024-01-15

---

## 1. Automóviles

### 1.1 GET /api/automoviles (Todos)
- **Código**: 200 OK
- **Response**: `[{"id":1,"color":"Blanco","ano":2022,"fabricante":"Toyota","tipo":"sedan"},...]`

### 1.2 GET /api/automoviles/{id} (Por ID)
- **Código**: 200 OK
- **URL**: `/api/automoviles/1`
- **Response**: `{"id":1,"color":"Blanco","ano":2022,"fabricante":"Toyota","tipo":"sedan"}`

### 1.3 GET /api/automoviles/color/{color} (Por Color)
- **Código**: 200 OK
- **URL**: `/api/automoviles/color/Rojo`
- **Response**: `[{"id":3,"color":"Rojo","ano":2023,"fabricante":"Yamaha","tipo":"moto"},{"id":6,"color":"Rojo","ano":2024,"fabricante":"Toyota","tipo":"Sedan"}]`

### 1.4 GET /api/automoviles/fabricante/{fabricante} (Por Fabricante)
- **Código**: 200 OK
- **URL**: `/api/automoviles/fabricante/Toyota`
- **Response**: `[{"id":1,"color":"Blanco","ano":2022,"fabricante":"Toyota","tipo":"sedan"},{"id":6,"color":"Rojo","ano":2024,"fabricante":"Toyota","tipo":"Sedan"}]`

### 1.5 GET /api/automoviles/tipo/{tipo} (Por Tipo)
- **Código**: 200 OK
- **URL**: `/api/automoviles/tipo/Sedan`
- **Response**: `[{"id":6,"color":"Rojo","ano":2024,"fabricante":"Toyota","tipo":"Sedan"}]`

### 1.6 POST /api/automoviles (Crear)
- **Código**: 201 Created
- **Body**: `{"color":"Verde","ano":2025,"fabricante":"BYD","tipo":"SUV"}`
- **Response**: `{"id":7,"color":"Verde","ano":2025,"fabricante":"BYD","tipo":"SUV"}`

### 1.7 PUT /api/automoviles/{id} (Actualizar)
- **Código**: 200 OK
- **URL**: `/api/automoviles/7`
- **Body**: `{"color":"Verde","ano":2025,"fabricante":"BYD","tipo":"Sedan"}`
- **Response**: `{"id":7,"color":"Verde","ano":2025,"fabricante":"BYD","tipo":"Sedan"}`

### 1.8 DELETE /api/automoviles/{id} (Eliminar)
- **Código**: 204 No Content
- **URL**: `/api/automoviles/7`
- **Response**: (vacío)

---

## 2. Parqueos

### 2.1 GET /api/parqueos (Todos)
- **Código**: 200 OK
- **Response**: `[{"id":1,"nombre":"Centro Parqueo San José",...},{"id":2,"nombre":"Parqueo Norte Heredia",...}]`

### 2.2 POST /api/parqueos (Crear)
- **Código**: 201 Created
- **Body**: `{"nombre":"Parqueo Sur","nombreDeProvincia":"Cartago","precioPorHora":12}`
- **Response**: `{"id":3,"nombre":"Parqueo Sur","nombreDeProvincia":"Cartago","precioPorHora":12}`

### 2.3 DELETE /api/parqueos/{id} (Eliminar)
- **Código**: 204 No Content
- **URL**: `/api/parqueos/3`

---

## 3. Ingresos

### 3.1 GET /api/ingresos (Todos)
- **Código**: 200 OK
- **Response**: `[{"consecutivo":1,"idAutomovil":0,"idParqueo":0,...},...]`

### 3.2 POST /api/ingresos (Crear)
- **Código**: 201 Created
- **Body**: `{"idAutomovil":1,"idParqueo":1,"fechaHoraEntrada":"2024-01-15T10:00:00"}`
- **Response**: `{"consecutivo":16,"idAutomovil":1,"idParqueo":1,"fechaHoraEntrada":"2024-01-15T10:00:00",...}`

### 3.3 PUT /api/ingresos/{consecutivo} (Registrar Salida)
- **Código**: 200 OK
- **URL**: `/api/ingresos/16`
- **Body**: `{"fechaHoraSalida":"2024-01-15T12:30:00"}`
- **Response**: `{"consecutivo":16,"idAutomovil":1,"idParqueo":1,"fechaHoraEntrada":"2024-01-15T10:00:00","fechaHoraSalida":"2024-01-15T12:30:00","duracionEstadiaMinutos":150,"duracionEstadiaHoras":2.5,...}`

---

## 4. Casos de Error

### 4.1 GET /api/automoviles/999 (404 Not Found)
- **Código**: 404
- **Response**: `{"message":"Automovil con ID 999 no encontrado"}`

---

## 5. Verificación de Persistencia

### 5.1 Archivo PRQ_Automoviles.json
```json
[
  {"Id":1,"Color":"Blanco","Ano":2022,"Fabricante":"Toyota","Tipo":"sedan"},
  {"Id":2,"Color":"Negro","Ano":2020,"Fabricante":"Honda","Tipo":"4x4"},
  {"Id":3,"Color":"Rojo","Ano":2023,"Fabricante":"Yamaha","Tipo":"moto"},
  {"Id":4,"Color":"Azul","Ano":2021,"Fabricante":"Ford","Tipo":"hatchback"},
  {"Id":5,"Color":"Gris","Ano":2019,"Fabricante":"Chevrolet","Tipo":"camioneta"},
  {"Id":6,"Color":"Rojo","Ano":2024,"Fabricante":"Toyota","Tipo":"Sedan"}
]
```
- **Estado**: ✅ Los datos persisten en archivo JSON

---

## Resumen de Resultados

| Categoría | Pruebas | Resultado |
|-----------|---------|-----------|
| Automóviles GET | 5/5 | ✅ PASS |
| Automóviles POST | 1/1 | ✅ PASS |
| Automóviles PUT | 1/1 | ✅ PASS |
| Automóviles DELETE | 1/1 | ✅ PASS |
| Parqueos GET | 1/1 | ✅ PASS |
| Parqueos POST | 1/1 | ✅ PASS |
| Parqueos DELETE | 1/1 | ✅ PASS |
| Ingresos GET | 1/1 | ✅ PASS |
| Ingresos POST | 1/1 | ✅ PASS |
| Ingresos PUT | 1/1 | ✅ PASS |
| Casos Error | 1/1 | ✅ PASS |
| Persistencia | - | ✅ PASS |

**Total: 15/15 pruebas exitosas**
