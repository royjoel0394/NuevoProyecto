using Ejercicio2.Repositorio.DTOs.Request;
using Ejercicio2.Repositorio.DTOs.Response;
using Ejercicio2.Repositorio.Repositories;
using Microsoft.AspNetCore.Mvc;
using ParqueoDatabaseExample.Models;

namespace Ejercicio2.Repositorio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutomovilesController : ControllerBase
{
    private readonly IAutomovilRepository _repository;

    public AutomovilesController(IAutomovilRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AutomovilDto>>> GetAll()
    {
        var automoviles = await _repository.GetAllAsync();
        return Ok(MapToDto(automoviles));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AutomovilDto>> GetById(int id)
    {
        var auto = await _repository.GetByIdAsync(id);
        if (auto == null)
            return NotFound(new { message = $"Automovil con ID {id} no encontrado" });

        return Ok(MapToDto(auto));
    }

    [HttpGet("color/{color}")]
    public async Task<ActionResult<IEnumerable<AutomovilDto>>> GetByColor(string color)
    {
        var automoviles = await _repository.GetByColorAsync(color);
        return Ok(MapToDto(automoviles));
    }

    [HttpGet("fabricante/{fabricante}")]
    public async Task<ActionResult<IEnumerable<AutomovilDto>>> GetByFabricante(string fabricante)
    {
        var automoviles = await _repository.GetByManufacturerAsync(fabricante);
        return Ok(MapToDto(automoviles));
    }

    [HttpGet("tipo/{tipo}")]
    public async Task<ActionResult<IEnumerable<AutomovilDto>>> GetByTipo(string tipo)
    {
        var automoviles = await _repository.GetByTypeAsync(tipo);
        return Ok(MapToDto(automoviles));
    }

    [HttpGet("rango-anio")]
    public async Task<ActionResult<IEnumerable<AutomovilDto>>> GetByYearRange([FromQuery] int anoInicio, [FromQuery] int anoFin)
    {
        var automoviles = await _repository.GetByYearRangeAsync(anoInicio, anoFin);
        return Ok(MapToDto(automoviles));
    }

    [HttpPost]
    public async Task<ActionResult<AutomovilDto>> Create([FromBody] CreateAutomovilDto dto)
    {
        if (dto == null) 
            return BadRequest(new { message = "Datos invalidos" });

        if (string.IsNullOrWhiteSpace(dto.Color) || 
            string.IsNullOrWhiteSpace(dto.Fabricante) || 
            string.IsNullOrWhiteSpace(dto.Tipo))
        {
            return BadRequest(new { message = "Color, Fabricante y Tipo son requeridos" });
        }

        var auto = new PrqAutomovile
        {
            Color = dto.Color,
            Ano = dto.Ano,
            Fabricante = dto.Fabricante,
            Tipo = dto.Tipo
        };

        await _repository.InsertAsync(auto);

        return CreatedAtAction(nameof(GetById), new { id = auto.Id }, MapToDto(auto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAutomovilDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = $"Automovil con ID {id} no encontrado" });

        if (dto == null) 
            return BadRequest(new { message = "Datos invalidos" });

        if (string.IsNullOrWhiteSpace(dto.Color) || 
            string.IsNullOrWhiteSpace(dto.Fabricante) || 
            string.IsNullOrWhiteSpace(dto.Tipo))
        {
            return BadRequest(new { message = "Color, Fabricante y Tipo son requeridos" });
        }

        existing.Color = dto.Color;
        existing.Ano = dto.Ano;
        existing.Fabricante = dto.Fabricante;
        existing.Tipo = dto.Tipo;

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
            return NotFound(new { message = $"Automovil con ID {id} no encontrado" });

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

    private static AutomovilDto MapToDto(PrqAutomovile auto) => new()
    {
        Id = auto.Id,
        Color = auto.Color,
        Ano = auto.Ano,
        Fabricante = auto.Fabricante,
        Tipo = auto.Tipo
    };

    private static IEnumerable<AutomovilDto> MapToDto(IEnumerable<PrqAutomovile> automoviles) 
        => automoviles.Select(MapToDto);
}