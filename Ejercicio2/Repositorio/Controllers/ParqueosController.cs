using Ejercicio2.Repositorio.DTOs.Request;
using Ejercicio2.Repositorio.DTOs.Response;
using Ejercicio2.Repositorio.Repositories;
using Microsoft.AspNetCore.Mvc;
using ParqueoDatabaseExample.Models;

namespace Ejercicio2.Repositorio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParqueosController : ControllerBase
{
    private readonly IParqueoRepository _repository;

    public ParqueosController(IParqueoRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ParqueoDto>>> GetAll()
    {
        var parqueos = await _repository.GetAllAsync();
        return Ok(MapToDto(parqueos));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ParqueoDto>> GetById(int id)
    {
        var parqueo = await _repository.GetByIdAsync(id);
        if (parqueo == null)
            return NotFound(new { message = $"Parqueo con ID {id} no encontrado" });

        return Ok(MapToDto(parqueo));
    }

    [HttpGet("provincia/{provincia}")]
    public async Task<ActionResult<IEnumerable<ParqueoDto>>> GetByProvincia(string provincia)
    {
        var parqueos = await _repository.GetByProvinciaAsync(provincia);
        return Ok(MapToDto(parqueos));
    }

    [HttpGet("nombre/{nombre}")]
    public async Task<ActionResult<IEnumerable<ParqueoDto>>> GetByNombre(string nombre)
    {
        var parqueos = await _repository.GetByNombreAsync(nombre);
        return Ok(MapToDto(parqueos));
    }

    [HttpGet("rango-precio")]
    public async Task<ActionResult<IEnumerable<ParqueoDto>>> GetByPrecioRange([FromQuery] decimal min, [FromQuery] decimal max)
    {
        var parqueos = await _repository.GetByPrecioHoraRangeAsync(min, max);
        return Ok(MapToDto(parqueos));
    }

    [HttpPost]
    public async Task<ActionResult<ParqueoDto>> Create([FromBody] CreateParqueoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre) || 
            string.IsNullOrWhiteSpace(dto.NombreDeProvincia))
        {
            return BadRequest(new { message = "Nombre y NombreDeProvincia son requeridos" });
        }

        if (dto.PrecioPorHora <= 0)
        {
            return BadRequest(new { message = "PrecioPorHora debe ser mayor a 0" });
        }

        var parqueo = new PrqParqueo
        {
            Nombre = dto.Nombre,
            NombreDeProvincia = dto.NombreDeProvincia,
            PrecioPorHora = dto.PrecioPorHora
        };

        await _repository.InsertAsync(parqueo);

        return CreatedAtAction(nameof(GetById), new { id = parqueo.Id }, MapToDto(parqueo));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateParqueoDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = $"Parqueo con ID {id} no encontrado" });

        if (string.IsNullOrWhiteSpace(dto.Nombre) || 
            string.IsNullOrWhiteSpace(dto.NombreDeProvincia))
        {
            return BadRequest(new { message = "Nombre y NombreDeProvincia son requeridos" });
        }

        if (dto.PrecioPorHora <= 0)
        {
            return BadRequest(new { message = "PrecioPorHora debe ser mayor a 0" });
        }

        existing.Nombre = dto.Nombre;
        existing.NombreDeProvincia = dto.NombreDeProvincia;
        existing.PrecioPorHora = dto.PrecioPorHora;

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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = $"Parqueo con ID {id} no encontrado" });

        try
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    private static ParqueoDto MapToDto(PrqParqueo parqueo) => new()
    {
        Id = parqueo.Id,
        Nombre = parqueo.Nombre,
        NombreDeProvincia = parqueo.NombreDeProvincia,
        PrecioPorHora = parqueo.PrecioPorHora
    };

    private static IEnumerable<ParqueoDto> MapToDto(IEnumerable<PrqParqueo> parqueos) 
        => parqueos.Select(MapToDto);
}
