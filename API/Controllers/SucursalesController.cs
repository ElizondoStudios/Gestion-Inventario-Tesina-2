using API.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controlador para la gestión de Sucursales (Módulo 3).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SucursalesController : ControllerBase
{
    private readonly ISucursalService _sucursalService;

    public SucursalesController(ISucursalService sucursalService)
    {
        _sucursalService = sucursalService;
    }

    /// <summary>
    /// Obtiene todas las sucursales activas.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SucursalDto>>> GetAll()
    {
        var sucursales = await _sucursalService.GetAllAsync();
        return Ok(sucursales);
    }

    /// <summary>
    /// Obtiene una sucursal por su Id.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SucursalDto>> GetById(int id)
    {
        var sucursal = await _sucursalService.GetByIdAsync(id);
        if (sucursal is null) return NotFound(new { message = "Sucursal no encontrada." });
        return Ok(sucursal);
    }

    /// <summary>
    /// Crea una nueva sucursal.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SucursalDto>> Create([FromBody] CreateSucursalDto dto)
    {
        var sucursal = await _sucursalService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = sucursal.IdSucursal }, sucursal);
    }

    /// <summary>
    /// Actualiza una sucursal existente.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSucursalDto dto)
    {
        var result = await _sucursalService.UpdateAsync(id, dto);
        if (!result) return NotFound(new { message = "Sucursal no encontrada." });
        return NoContent();
    }

    /// <summary>
    /// Desactiva una sucursal (borrado lógico).
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Desactivar(int id)
    {
        var result = await _sucursalService.DesactivarAsync(id);
        if (!result) return NotFound(new { message = "Sucursal no encontrada." });
        return NoContent();
    }
}
