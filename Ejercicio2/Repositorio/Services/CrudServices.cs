// =====================================================
// SERVICIOS API - Sistema de Parqueo
// C# con Entity Framework Core
// =====================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Ejercicio2.Repositorio.Services
{
    // =====================================================
    // MODELOS DE ENTIDAD
    // =====================================================

    public class PrqAutomovile
    {
        public int Id { get; set; }
        public string Color { get; set; } = string.Empty;
        public int Ano { get; set; }
        public string Fabricante { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }

    public class PrqParqueo
    {
        public int Id { get; set; }
        public string NombreProvincia { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioPorHora { get; set; }
    }

    public class PrqIngresoAutomovil
    {
        public int Consecutivo { get; set; }
        public int IdParqueo { get; set; }
        public int IdAutomovil { get; set; }
        public DateTime FechaHoraEntrada { get; set; }
        public DateTime? FechaHoraSalida { get; set; }
    }

    // =====================================================
    // MODELOS DE REQUEST DTOs
    // =====================================================

    public class CreateAutomovilRequest
    {
        public string Color { get; set; } = string.Empty;
        public int Ano { get; set; }
        public string Fabricante { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }

    public class UpdateAutomovilRequest
    {
        public int Id { get; set; }
        public string Color { get; set; } = string.Empty;
        public int Ano { get; set; }
        public string Fabricante { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }

    public class CreateParqueoRequest
    {
        public string NombreProvincia { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioPorHora { get; set; }
    }

    public class UpdateParqueoRequest
    {
        public int Id { get; set; }
        public string NombreProvincia { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioPorHora { get; set; }
    }

    public class CreateIngresoRequest
    {
        public int IdParqueo { get; set; }
        public int IdAutomovil { get; set; }
        public DateTime? FechaHoraEntrada { get; set; }  // Opcional: usa fecha actual si es null
    }

    public class UpdateIngresoRequest
    {
        public int Consecutivo { get; set; }
        public DateTime FechaHoraSalida { get; set; }
    }

    // =====================================================
    // RESPUESTA DE LA API
    // =====================================================

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ApiResponse<T> Ok(T data, string message = "Operacion exitosa")
            => new() { Success = true, Message = message, Data = data };

        public static ApiResponse<T> Error(string message, List<string>? errors = null)
            => new() { Success = false, Message = message, Errors = errors ?? new() };
    }

    // =====================================================
    // SERVICIO: Automóviles CRUD
    // =====================================================

    public interface IAutomovilCrudService
    {
        Task<ApiResponse<PrqAutomovile>> InsertAsync(CreateAutomovilRequest request, CancellationToken cancellationToken = default);
        Task<ApiResponse<PrqAutomovile>> UpdateAsync(UpdateAutomovilRequest request, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<ApiResponse<PrqAutomovile>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }

    public class AutomovilCrudService : IAutomovilCrudService
    {
        private readonly DbContext _context;

        public AutomovilCrudService(DbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Inserta un nuevo automóvil
        /// </summary>
        public async Task<ApiResponse<PrqAutomovile>> InsertAsync(CreateAutomovilRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                // ========== VALIDACIONES ==========
                var errors = new List<string>();

                if (string.IsNullOrWhiteSpace(request.Color))
                    errors.Add("El color es obligatorio.");
                
                if (request.Ano < 1900 || request.Ano > DateTime.Now.Year + 1)
                    errors.Add($"El año debe estar entre 1900 y {DateTime.Now.Year + 1}.");
                
                if (string.IsNullOrWhiteSpace(request.Fabricante))
                    errors.Add("El fabricante es obligatorio.");
                
                if (string.IsNullOrWhiteSpace(request.Tipo))
                    errors.Add("El tipo es obligatorio.");

                if (errors.Count > 0)
                    return ApiResponse<PrqAutomovile>.Error("Errores de validación.", errors);

                // ========== INSERCION ==========
                var entity = new PrqAutomovile
                {
                    Color = request.Color.Trim(),
                    Ano = request.Ano,
                    Fabricante = request.Fabricante.Trim(),
                    Tipo = request.Tipo.Trim()
                };

                _context.Set<PrqAutomovile>().Add(entity);
                await _context.SaveChangesAsync(cancellationToken);

                return ApiResponse<PrqAutomovile>.Ok(entity, "Automóvil insertado correctamente.");
            }
            catch (DbUpdateException ex)
            {
                return ApiResponse<PrqAutomovile>.Error("Error al insertar el automóvil.", new List<string> { ex.Message });
            }
            catch (Exception ex)
            {
                return ApiResponse<PrqAutomovile>.Error("Error inesperado.", new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un automóvil existente
        /// </summary>
        public async Task<ApiResponse<PrqAutomovile>> UpdateAsync(UpdateAutomovilRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                // ========== VALIDACIONES ==========
                var errors = new List<string>();

                if (request.Id <= 0)
                    errors.Add("El ID es obligatorio.");
                
                if (string.IsNullOrWhiteSpace(request.Color))
                    errors.Add("El color es obligatorio.");
                
                if (request.Ano < 1900 || request.Ano > DateTime.Now.Year + 1)
                    errors.Add($"El año debe estar entre 1900 y {DateTime.Now.Year + 1}.");
                
                if (string.IsNullOrWhiteSpace(request.Fabricante))
                    errors.Add("El fabricante es obligatorio.");
                
                if (string.IsNullOrWhiteSpace(request.Tipo))
                    errors.Add("El tipo es obligatorio.");

                if (errors.Count > 0)
                    return ApiResponse<PrqAutomovile>.Error("Errores de validación.", errors);

                // ========== VERIFICAR EXISTENCIA ==========
                var entity = await _context.Set<PrqAutomovile>().FindAsync(new object[] { request.Id }, cancellationToken);
                if (entity == null)
                    return ApiResponse<PrqAutomovile>.Error($"El automóvil con ID {request.Id} no existe.");

                // ========== ACTUALIZACION ==========
                entity.Color = request.Color.Trim();
                entity.Ano = request.Ano;
                entity.Fabricante = request.Fabricante.Trim();
                entity.Tipo = request.Tipo.Trim();

                _context.Set<PrqAutomovile>().Update(entity);
                await _context.SaveChangesAsync(cancellationToken);

                return ApiResponse<PrqAutomovile>.Ok(entity, "Automóvil actualizado correctamente.");
            }
            catch (DbUpdateException ex)
            {
                return ApiResponse<PrqAutomovile>.Error("Error al actualizar el automóvil.", new List<string> { ex.Message });
            }
            catch (Exception ex)
            {
                return ApiResponse<PrqAutomovile>.Error("Error inesperado.", new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// Elimina un automóvil (borrado físico)
        /// Verifica que no tenga ingresos asociados
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                // ========== VALIDACIONES ==========
                if (id <= 0)
                    return ApiResponse<bool>.Error("El ID debe ser mayor a 0.");

                // ========== VERIFICAR EXISTENCIA ==========
                var entity = await _context.Set<PrqAutomovile>().FindAsync(new object[] { id }, cancellationToken);
                if (entity == null)
                    return ApiResponse<bool>.Error($"El automóvil con ID {id} no existe.");

                // ========== VERIFICAR INGRESOS ASOCIADOS ==========
                var hasIngresos = await _context.Set<PrqIngresoAutomovil>()
                    .AnyAsync(i => i.IdAutomovil == id, cancellationToken);
                
                if (hasIngresos)
                    return ApiResponse<bool>.Error("No se puede eliminar el automóvil porque tiene ingresos asociados.");

                // ========== ELIMINACION ==========
                _context.Set<PrqAutomovile>().Remove(entity);
                await _context.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Ok(true, "Automóvil eliminado correctamente.");
            }
            catch (DbUpdateException ex)
            {
                return ApiResponse<bool>.Error("Error al eliminar el automóvil.", new List<string> { ex.Message });
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Error("Error inesperado.", new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un automóvil por ID
        /// </summary>
        public async Task<ApiResponse<PrqAutomovile>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return ApiResponse<PrqAutomovile>.Error("El ID debe ser mayor a 0.");

                var entity = await _context.Set<PrqAutomovile>().FindAsync(new object[] { id }, cancellationToken);
                if (entity == null)
                    return ApiResponse<PrqAutomovile>.Error($"El automóvil con ID {id} no existe.");

                return ApiResponse<PrqAutomovile>.Ok(entity);
            }
            catch (Exception ex)
            {
                return ApiResponse<PrqAutomovile>.Error("Error inesperado.", new List<string> { ex.Message });
            }
        }
    }

    // =====================================================
    // SERVICIO: Parqueo CRUD
    // =====================================================

    public interface IParqueoCrudService
    {
        Task<ApiResponse<PrqParqueo>> InsertAsync(CreateParqueoRequest request, CancellationToken cancellationToken = default);
        Task<ApiResponse<PrqParqueo>> UpdateAsync(UpdateParqueoRequest request, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<ApiResponse<PrqParqueo>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }

    public class ParqueoCrudService : IParqueoCrudService
    {
        private readonly DbContext _context;

        public ParqueoCrudService(DbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Inserta un nuevo parqueo
        /// </summary>
        public async Task<ApiResponse<PrqParqueo>> InsertAsync(CreateParqueoRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                // ========== VALIDACIONES ==========
                var errors = new List<string>();

                if (string.IsNullOrWhiteSpace(request.NombreProvincia))
                    errors.Add("La provincia es obligatoria.");
                
                if (string.IsNullOrWhiteSpace(request.Nombre))
                    errors.Add("El nombre es obligatorio.");
                
                if (request.PrecioPorHora <= 0)
                    errors.Add("El precio por hora debe ser mayor a 0.");

                if (errors.Count > 0)
                    return ApiResponse<PrqParqueo>.Error("Errores de validación.", errors);

                // ========== INSERCION ==========
                var entity = new PrqParqueo
                {
                    NombreProvincia = request.NombreProvincia.Trim(),
                    Nombre = request.Nombre.Trim(),
                    PrecioPorHora = request.PrecioPorHora
                };

                _context.Set<PrqParqueo>().Add(entity);
                await _context.SaveChangesAsync(cancellationToken);

                return ApiResponse<PrqParqueo>.Ok(entity, "Parqueo insertado correctamente.");
            }
            catch (DbUpdateException ex)
            {
                return ApiResponse<PrqParqueo>.Error("Error al insertar el parqueo.", new List<string> { ex.Message });
            }
            catch (Exception ex)
            {
                return ApiResponse<PrqParqueo>.Error("Error inesperado.", new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un parqueo existente
        /// </summary>
        public async Task<ApiResponse<PrqParqueo>> UpdateAsync(UpdateParqueoRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                // ========== VALIDACIONES ==========
                var errors = new List<string>();

                if (request.Id <= 0)
                    errors.Add("El ID es obligatorio.");
                
                if (string.IsNullOrWhiteSpace(request.NombreProvincia))
                    errors.Add("La provincia es obligatoria.");
                
                if (string.IsNullOrWhiteSpace(request.Nombre))
                    errors.Add("El nombre es obligatorio.");
                
                if (request.PrecioPorHora <= 0)
                    errors.Add("El precio por hora debe ser mayor a 0.");

                if (errors.Count > 0)
                    return ApiResponse<PrqParqueo>.Error("Errores de validación.", errors);

                // ========== VERIFICAR EXISTENCIA ==========
                var entity = await _context.Set<PrqParqueo>().FindAsync(new object[] { request.Id }, cancellationToken);
                if (entity == null)
                    return ApiResponse<PrqParqueo>.Error($"El parqueo con ID {request.Id} no existe.");

                // ========== ACTUALIZACION ==========
                entity.NombreProvincia = request.NombreProvincia.Trim();
                entity.Nombre = request.Nombre.Trim();
                entity.PrecioPorHora = request.PrecioPorHora;

                _context.Set<PrqParqueo>().Update(entity);
                await _context.SaveChangesAsync(cancellationToken);

                return ApiResponse<PrqParqueo>.Ok(entity, "Parqueo actualizado correctamente.");
            }
            catch (DbUpdateException ex)
            {
                return ApiResponse<PrqParqueo>.Error("Error al actualizar el parqueo.", new List<string> { ex.Message });
            }
            catch (Exception ex)
            {
                return ApiResponse<PrqParqueo>.Error("Error inesperado.", new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// Elimina un parqueo (borrado físico)
        /// Verifica que no tenga ingresos asociados
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                // ========== VALIDACIONES ==========
                if (id <= 0)
                    return ApiResponse<bool>.Error("El ID debe ser mayor a 0.");

                // ========== VERIFICAR EXISTENCIA ==========
                var entity = await _context.Set<PrqParqueo>().FindAsync(new object[] { id }, cancellationToken);
                if (entity == null)
                    return ApiResponse<bool>.Error($"El parqueo con ID {id} no existe.");

                // ========== VERIFICAR INGRESOS ASOCIADOS ==========
                var hasIngresos = await _context.Set<PrqIngresoAutomovil>()
                    .AnyAsync(i => i.IdParqueo == id, cancellationToken);
                
                if (hasIngresos)
                    return ApiResponse<bool>.Error("No se puede eliminar el parqueo porque tiene ingresos asociados.");

                // ========== ELIMINACION ==========
                _context.Set<PrqParqueo>().Remove(entity);
                await _context.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Ok(true, "Parqueo eliminado correctamente.");
            }
            catch (DbUpdateException ex)
            {
                return ApiResponse<bool>.Error("Error al eliminar el parqueo.", new List<string> { ex.Message });
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Error("Error inesperado.", new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un parqueo por ID
        /// </summary>
        public async Task<ApiResponse<PrqParqueo>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return ApiResponse<PrqParqueo>.Error("El ID debe ser mayor a 0.");

                var entity = await _context.Set<PrqParqueo>().FindAsync(new object[] { id }, cancellationToken);
                if (entity == null)
                    return ApiResponse<PrqParqueo>.Error($"El parqueo con ID {id} no existe.");

                return ApiResponse<PrqParqueo>.Ok(entity);
            }
            catch (Exception ex)
            {
                return ApiResponse<PrqParqueo>.Error("Error inesperado.", new List<string> { ex.Message });
            }
        }
    }

    // =====================================================
    // SERVICIO: Ingreso Automóviles CRUD
    // =====================================================

    public interface IIngresoAutomovilCrudService
    {
        Task<ApiResponse<PrqIngresoAutomovil>> InsertAsync(CreateIngresoRequest request, CancellationToken cancellationToken = default);
        Task<ApiResponse<PrqIngresoAutomovil>> UpdateSalidaAsync(UpdateIngresoRequest request, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeleteAsync(int consecutivo, CancellationToken cancellationToken = default);
        Task<ApiResponse<PrqIngresoAutomovil>> GetByIdAsync(int consecutive, CancellationToken cancellationToken = default);
    }

    public class IngresoAutomovilCrudService : IIngresoAutomovilCrudService
    {
        private readonly DbContext _context;

        public IngresoAutomovilCrudService(DbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Inserta un nuevo ingreso (entrada de automóvil)
        /// Si no se especifica fecha_hora_entrada, usa la fecha actual
        /// </summary>
        public async Task<ApiResponse<PrqIngresoAutomovil>> InsertAsync(CreateIngresoRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                // ========== VALIDACIONES ==========
                var errors = new List<string>();

                if (request.IdParqueo <= 0)
                    errors.Add("El ID del parqueo es obligatorio.");
                
                if (request.IdAutomovil <= 0)
                    errors.Add("El ID del automóvil es obligatorio.");

                if (errors.Count > 0)
                    return ApiResponse<PrqIngresoAutomovil>.Error("Errores de validación.", errors);

                // ========== VERIFICAR EXISTENCIA DEL PARQUEO ==========
                var parqueoExists = await _context.Set<PrqParqueo>()
                    .AnyAsync(p => p.Id == request.IdParqueo, cancellationToken);
                
                if (!parqueoExists)
                    return ApiResponse<PrqIngresoAutomovil>.Error($"El parqueo con ID {request.IdParqueo} no existe.");

                // ========== VERIFICAR EXISTENCIA DEL AUTOMOVIL ==========
                var automovileExists = await _context.Set<PrqAutomovile>()
                    .AnyAsync(a => a.Id == request.IdAutomovil, cancellationToken);
                
                if (!automovileExists)
                    return ApiResponse<PrqIngresoAutomovil>.Error($"El automóvil con ID {request.IdAutomovil} no existe.");

                // ========== DETERMINAR FECHA DE ENTRADA ==========
                var fechaEntrada = request.FechaHoraEntrada ?? DateTime.Now;

                if (fechaEntrada > DateTime.Now)
                    return ApiResponse<PrqIngresoAutomovil>.Error("La fecha de entrada no puede ser futura.");

                // ========== INSERCION ==========
                var entity = new PrqIngresoAutomovil
                {
                    IdParqueo = request.IdParqueo,
                    IdAutomovil = request.IdAutomovil,
                    FechaHoraEntrada = fechaEntrada,
                    FechaHoraSalida = null  // NULL inicialmente (auto dentro del parqueo)
                };

                _context.Set<PrqIngresoAutomovil>().Add(entity);
                await _context.SaveChangesAsync(cancellationToken);

                return ApiResponse<PrqIngresoAutomovil>.Ok(entity, "Ingreso registrado correctamente.");
            }
            catch (DbUpdateException ex)
            {
                return ApiResponse<PrqIngresoAutomovil>.Error("Error al registrar el ingreso.", new List<string> { ex.Message });
            }
            catch (Exception ex)
            {
                return ApiResponse<PrqIngresoAutomovil>.Error("Error inesperado.", new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// Actualiza la fecha de salida (registrar salida del automóvil)
        /// Valida que la fecha de salida sea posterior a la fecha de entrada
        /// </summary>
        public async Task<ApiResponse<PrqIngresoAutomovil>> UpdateSalidaAsync(UpdateIngresoRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                // ========== VALIDACIONES ==========
                if (request.Consecutivo <= 0)
                    return ApiResponse<PrqIngresoAutomovil>.Error("El consecutivo es obligatorio.");
                
                if (request.FechaHoraSalida == default)
                    return ApiResponse<PrqIngresoAutomovil>.Error("La fecha de salida es obligatoria.");

                // ========== VERIFICAR EXISTENCIA ==========
                var entity = await _context.Set<PrqIngresoAutomovil>()
                    .FindAsync(new object[] { request.Consecutivo }, cancellationToken);
                
                if (entity == null)
                    return ApiResponse<PrqIngresoAutomovil>.Error($"El ingreso con consecutivo {request.Consecutivo} no existe.");

                // ========== VALIDAR FECHA DE SALIDA > ENTRADA ==========
                if (request.FechaHoraSalida <= entity.FechaHoraEntrada)
                    return ApiResponse<PrqIngresoAutomovil>.Error("La fecha de salida debe ser posterior a la fecha de entrada.");

                // ========== VALIDAR QUE NO TENGA SALIDA YA REGISTRADA ==========
                if (entity.FechaHoraSalida.HasValue)
                    return ApiResponse<PrqIngresoAutomovil>.Error("Este ingreso ya tiene registrada una salida.");

                // ========== ACTUALIZACION ==========
                entity.FechaHoraSalida = request.FechaHoraSalida;

                _context.Set<PrqIngresoAutomovil>().Update(entity);
                await _context.SaveChangesAsync(cancellationToken);

                return ApiResponse<PrqIngresoAutomovil>.Ok(entity, "Salida registrada correctamente.");
            }
            catch (DbUpdateException ex)
            {
                return ApiResponse<PrqIngresoAutomovil>.Error("Error al registrar la salida.", new List<string> { ex.Message });
            }
            catch (Exception ex)
            {
                return ApiResponse<PrqIngresoAutomovil>.Error("Error inesperado.", new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// Elimina un ingreso (borrado físico)
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteAsync(int consecutive, CancellationToken cancellationToken = default)
        {
            try
            {
                // ========== VALIDACIONES ==========
                if (consecutivo <= 0)
                    return ApiResponse<bool>.Error("El consecutivo debe ser mayor a 0.");

                // ========== VERIFICAR EXISTENCIA ==========
                var entity = await _context.Set<PrqIngresoAutomovil>()
                    .FindAsync(new object[] { consecutive }, cancellationToken);
                
                if (entity == null)
                    return ApiResponse<bool>.Error($"El ingreso con consecutivo {consecutivo} no existe.");

                // ========== ELIMINACION ==========
                _context.Set<PrqIngresoAutomovil>().Remove(entity);
                await _context.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Ok(true, "Ingreso eliminado correctamente.");
            }
            catch (DbUpdateException ex)
            {
                return ApiResponse<bool>.Error("Error al eliminar el ingreso.", new List<bool> { ex.Message });
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Error("Error inesperado.", new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un ingreso por consecutivo
        /// </summary>
        public async Task<ApiResponse<PrqIngresoAutomovil>> GetByIdAsync(int consecutive, CancellationToken cancellationToken = default)
        {
            try
            {
                if (consecutive <= 0)
                    return ApiResponse<PrqIngresoAutomovil>.Error("El consecutivo debe ser mayor a 0.");

                var entity = await _context.Set<PrqIngresoAutomovil>()
                    .FindAsync(new object[] { consecutive }, cancellationToken);
                
                if (entity == null)
                    return ApiResponse<PrqIngresoAutomovil>.Error($"El ingreso con consecutivo {consecutive} no existe.");

                return ApiResponse<PrqIngresoAutomovil>.Ok(entity);
            }
            catch (Exception ex)
            {
                return ApiResponse<PrqIngresoAutomovil>.Error("Error inesperado.", new List<string> { ex.Message });
            }
        }
    }

    // =====================================================
    // REGISTRO DE SERVICIOS EN DI
    // =====================================================

    public static class CrudServiceCollectionExtensions
    {
        public static IServiceCollection AddCrudServices(this IServiceCollection services)
        {
            services.AddScoped<IAutomovilCrudService, AutomovilCrudService>();
            services.AddScoped<IParqueoCrudService, ParqueoCrudService>();
            services.AddScoped<IIngresoAutomovilCrudService, IngresoAutomovilCrudService>();
            return services;
        }
    }
}