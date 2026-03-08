using API.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controlador para la gestión del catálogo de Tipos de Movimiento.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TiposMovimientoController : ControllerBase
{
    private readonly ITipoMovimientoService _tipoMovimientoService;

    public TiposMovimientoController(ITipoMovimientoService tipoMovimientoService)
    {
        _tipoMovimientoService = tipoMovimientoService;
    }

    /// <summary>
    /// Obtiene todos los tipos de movimiento.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TipoMovimientoDto>>> GetAll()
    {
        var tipos = await _tipoMovimientoService.GetAllAsync();
        return Ok(tipos);
    }

    /// <summary>
    /// Obtiene un tipo de movimiento por su Id.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TipoMovimientoDto>> GetById(int id)
    {
        var tipo = await _tipoMovimientoService.GetByIdAsync(id);
        if (tipo is null) return NotFound(new { message = "Tipo de movimiento no encontrado." });
        return Ok(tipo);
    }

    /// <summary>
    /// Crea un nuevo tipo de movimiento.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TipoMovimientoDto>> Create([FromBody] CreateTipoMovimientoDto dto)
    {
        try
        {
            var tipo = await _tipoMovimientoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = tipo.IdTipoMovimiento }, tipo);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza un tipo de movimiento existente.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTipoMovimientoDto dto)
    {
        try
        {
            var result = await _tipoMovimientoService.UpdateAsync(id, dto);
            if (!result) return NotFound(new { message = "Tipo de movimiento no encontrado." });
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un tipo de movimiento.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _tipoMovimientoService.DeleteAsync(id);
        if (!result) return NotFound(new { message = "Tipo de movimiento no encontrado." });
        return NoContent();
    }
}
