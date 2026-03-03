using API.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controlador para la gestión de Roles y Permisos (Módulo 2).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRolService _rolService;

    public RolesController(IRolService rolService)
    {
        _rolService = rolService;
    }

    /// <summary>
    /// Obtiene todos los roles.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RolDto>>> GetAll()
    {
        var roles = await _rolService.GetAllAsync();
        return Ok(roles);
    }

    /// <summary>
    /// Obtiene un rol por su Id, incluyendo sus permisos.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<RolDetalleDto>> GetById(int id)
    {
        var rol = await _rolService.GetByIdAsync(id);
        if (rol is null) return NotFound(new { message = "Rol no encontrado." });
        return Ok(rol);
    }

    /// <summary>
    /// Crea un nuevo rol.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<RolDto>> Create([FromBody] CreateRolDto dto)
    {
        var rol = await _rolService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = rol.IdRol }, rol);
    }

    /// <summary>
    /// Actualiza un rol existente.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRolDto dto)
    {
        var result = await _rolService.UpdateAsync(id, dto);
        if (!result) return NotFound(new { message = "Rol no encontrado." });
        return NoContent();
    }

    /// <summary>
    /// Elimina un rol.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _rolService.DeleteAsync(id);
        if (!result) return NotFound(new { message = "Rol no encontrado." });
        return NoContent();
    }

    /// <summary>
    /// Obtiene los permisos de un rol.
    /// </summary>
    [HttpGet("{id}/permisos")]
    public async Task<ActionResult<IEnumerable<RolModuloPermisoDto>>> GetPermisos(int id)
    {
        var permisos = await _rolService.GetPermisosAsync(id);
        return Ok(permisos);
    }

    /// <summary>
    /// Asigna o actualiza un permiso para un rol.
    /// </summary>
    [HttpPost("{id}/permisos")]
    public async Task<IActionResult> AsignarPermiso(int id, [FromBody] AsignarPermisoDto dto)
    {
        try
        {
            var result = await _rolService.AsignarPermisoAsync(id, dto);
            if (!result) return NotFound(new { message = "Rol o categoría no encontrados." });
            return Ok(new { message = "Permiso asignado correctamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Remueve un permiso de un rol.
    /// </summary>
    [HttpDelete("{idRol}/permisos/{idModuloCategoria}")]
    public async Task<IActionResult> RemoverPermiso(int idRol, int idModuloCategoria)
    {
        var result = await _rolService.RemoverPermisoAsync(idRol, idModuloCategoria);
        if (!result) return NotFound(new { message = "Permiso no encontrado." });
        return NoContent();
    }
}
