using Ejercicio2.Repositorio.DTOs.Request;
using Ejercicio2.Repositorio.DTOs.Response;
using Ejercicio2.Repositorio.Dtos;
using Ejercicio2.Repositorio.Repositories;
using Microsoft.AspNetCore.Mvc;
using ParqueoDatabaseExample.Models;

namespace Ejercicio2.Repositorio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngresosController : ControllerBase
{
    private readonly IIngresoAutomovilRepository _repository;

    public IngresosController(IIngresoAutomovilRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IngresoAutomovilDto>>> GetAll()
    {
        var ingresos = await _repository.GetAllAsync();
        return Ok(MapToDto(ingresos));
    }

    [HttpGet("{consecutivo}")]
    public async Task<ActionResult<IngresoAutomovilDto>> GetById(int consecutivo)
    {
        var ingreso = await _repository.GetByIdAsync(consecutivo);
        if (ingreso == null)
            return NotFound(new { message = $"Ingreso con consecutivo {consecutivo} no encontrado" });

        return Ok(MapToDto(ingreso));
    }

    [HttpGet("precio-parqueo/{idParqueo}")]
    public async Task<ActionResult> GetPrecioPorHora(int idParqueo)
    {
        var precio = await _repository.GetPrecioPorHoraAsync(idParqueo);
        if (precio == null)
            return NotFound(new { message = $"Parqueo con ID {idParqueo} no encontrado" });

        return Ok(new { idParqueo, precioPorHora = precio });
    }

    [HttpGet("por-tipo")]
    public async Task<ActionResult<IEnumerable<IngresoAutomovilDetalleDto>>> GetByTipo(
        [FromQuery] string tipoAutomovil,
        [FromQuery] DateTime desde,
        [FromQuery] DateTime hasta)
    {
        if (string.IsNullOrWhiteSpace(tipoAutomovil))
        {
            return BadRequest(new { message = "tipoAutomovil es requerido" });
        }

        var ingresos = await _repository.GetIngresosPorTipoAutomovilAsync(tipoAutomovil, desde, hasta);
        return Ok(ingresos);
    }

    [HttpGet("por-provincia")]
    public async Task<ActionResult<IEnumerable<IngresoPorProvinciaDto>>> GetByProvincia(
        [FromQuery] string provincia,
        [FromQuery] DateTime desde,
        [FromQuery] DateTime hasta)
    {
        if (string.IsNullOrWhiteSpace(provincia))
        {
            return BadRequest(new { message = "provincia es requerido" });
        }

        var ingresos = await _repository.GetIngresosPorProvinciaAsync(provincia, desde, hasta);
        return Ok(ingresos);
    }

    [HttpPost]
    public async Task<ActionResult<IngresoAutomovilDto>> Create([FromBody] CreateIngresoAutomovilDto dto)
    {
        if (dto.IdAutomovil <= 0 || dto.IdParqueo <= 0)
        {
            return BadRequest(new { message = "IdAutomovil e IdParqueo son requeridos" });
        }

        var ingreso = new PrqIngresoAutomovile
        {
            IdAutomovil = dto.IdAutomovil,
            IdParqueo = dto.IdParqueo,
            FechaHoraEntrada = dto.FechaHoraEntrada,
            FechaHoraSalida = dto.FechaHoraSalida
        };

        await _repository.InsertAsync(ingreso);

        return CreatedAtAction(nameof(GetById), new { consecutive = ingreso.Consecutivo }, MapToDto(ingreso));
    }

    [HttpPut("{consecutivo}")]
    public async Task<IActionResult> Update(int consecutivo, [FromBody] UpdateIngresoAutomovilDto dto)
    {
        var existing = await _repository.GetByIdAsync(consecutivo);
        if (existing == null)
            return NotFound(new { message = $"Ingreso con consecutivo {consecutivo} no encontrado" });

        existing.FechaHoraSalida = dto.FechaHoraSalida;

        try
        {
            await _repository.UpdateAsync(existing);
            return Ok(MapToDto(existing));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{consecutivo}")]
    public async Task<IActionResult> Delete(int consecutivo)
    {
        var existing = await _repository.GetByIdAsync(consecutivo);
        if (existing == null)
            return NotFound(new { message = $"Ingreso con consecutivo {consecutivo} no encontrado" });

        try
        {
            await _repository.DeleteAsync(consecutivo);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    private static IngresoAutomovilDto MapToDto(PrqIngresoAutomovile ingreso) => new()
    {
        Consecutivo = ingreso.Consecutivo,
        IdAutomovil = ingreso.IdAutomovil,
        IdParqueo = ingreso.IdParqueo,
        FechaHoraEntrada = ingreso.FechaHoraEntrada,
        FechaHoraSalida = ingreso.FechaHoraSalida,
        DuracionEstadiaMinutos = ingreso.DuracionEstadiaMinutos,
        DuracionEstadiaHoras = ingreso.DuracionEstadiaHoras,
        MontoTotalAPagar = ingreso.MontoTotalAPagar
    };

    private static IEnumerable<IngresoAutomovilDto> MapToDto(IEnumerable<PrqIngresoAutomovile> ingresos) 
        => ingresos.Select(MapToDto);
}
