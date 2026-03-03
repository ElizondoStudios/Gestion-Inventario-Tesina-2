using API.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controlador para la gestión de Módulos y Categorías del sistema (Módulo 2).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ModulosController : ControllerBase
{
    private readonly IModuloService _moduloService;

    public ModulosController(IModuloService moduloService)
    {
        _moduloService = moduloService;
    }

    /// <summary>
    /// Obtiene todos los módulos activos.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ModuloDto>>> GetAll()
    {
        var modulos = await _moduloService.GetAllAsync();
        return Ok(modulos);
    }

    /// <summary>
    /// Obtiene un módulo por su Id, incluyendo sus categorías.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ModuloDetalleDto>> GetById(int id)
    {
        var modulo = await _moduloService.GetByIdAsync(id);
        if (modulo is null) return NotFound(new { message = "Módulo no encontrado." });
        return Ok(modulo);
    }

    /// <summary>
    /// Crea una nueva categoría dentro de un módulo.
    /// </summary>
    [HttpPost("{idModulo}/categorias")]
    public async Task<ActionResult<ModuloCategoriaDto>> CreateCategoria(int idModulo, [FromBody] CreateModuloCategoriaDto dto)
    {
        try
        {
            var categoria = await _moduloService.CreateCategoriaAsync(idModulo, dto);
            return CreatedAtAction(nameof(GetById), new { id = idModulo }, categoria);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza una categoría de módulo.
    /// </summary>
    [HttpPut("categorias/{idModuloCategoria}")]
    public async Task<IActionResult> UpdateCategoria(int idModuloCategoria, [FromBody] UpdateModuloCategoriaDto dto)
    {
        var result = await _moduloService.UpdateCategoriaAsync(idModuloCategoria, dto);
        if (!result) return NotFound(new { message = "Categoría no encontrada." });
        return NoContent();
    }

    /// <summary>
    /// Elimina una categoría de módulo.
    /// </summary>
    [HttpDelete("categorias/{idModuloCategoria}")]
    public async Task<IActionResult> DeleteCategoria(int idModuloCategoria)
    {
        var result = await _moduloService.DeleteCategoriaAsync(idModuloCategoria);
        if (!result) return NotFound(new { message = "Categoría no encontrada." });
        return NoContent();
    }
}
