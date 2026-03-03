using API.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controlador para la gestión de Usuarios y Autenticación (Módulo 1).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    /// <summary>
    /// Obtiene todos los usuarios activos.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetAll()
    {
        var usuarios = await _usuarioService.GetAllAsync();
        return Ok(usuarios);
    }

    /// <summary>
    /// Obtiene un usuario por su Id, incluyendo sus sucursales asignadas.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UsuarioDetalleDto>> GetById(int id)
    {
        var usuario = await _usuarioService.GetByIdAsync(id);
        if (usuario is null) return NotFound(new { message = "Usuario no encontrado." });
        return Ok(usuario);
    }

    /// <summary>
    /// Crea un nuevo usuario. La contraseña se almacena hasheada (RNF1.1).
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> Create([FromBody] CreateUsuarioDto dto)
    {
        try
        {
            var usuario = await _usuarioService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = usuario.IdUsuario }, usuario);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza un usuario existente.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUsuarioDto dto)
    {
        try
        {
            var result = await _usuarioService.UpdateAsync(id, dto);
            if (!result) return NotFound(new { message = "Usuario no encontrado." });
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Desactiva un usuario (borrado lógico).
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Desactivar(int id)
    {
        var result = await _usuarioService.DesactivarAsync(id);
        if (!result) return NotFound(new { message = "Usuario no encontrado." });
        return NoContent();
    }

    /// <summary>
    /// Inicia sesión con correo y contraseña.
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto)
    {
        var result = await _usuarioService.LoginAsync(dto);
        if (result is null) return Unauthorized(new { message = "Credenciales inválidas o usuario inactivo." });
        return Ok(result);
    }

    /// <summary>
    /// Asigna una sucursal a un usuario.
    /// </summary>
    [HttpPost("{id}/sucursales")]
    public async Task<IActionResult> AsignarSucursal(int id, [FromBody] AsignarSucursalDto dto)
    {
        try
        {
            var result = await _usuarioService.AsignarSucursalAsync(id, dto);
            if (!result) return NotFound(new { message = "Usuario o sucursal no encontrados." });
            return Ok(new { message = "Sucursal asignada correctamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Remueve la asignación de una sucursal a un usuario.
    /// </summary>
    [HttpDelete("{idUsuario}/sucursales/{idSucursal}")]
    public async Task<IActionResult> RemoverSucursal(int idUsuario, int idSucursal)
    {
        var result = await _usuarioService.RemoverSucursalAsync(idUsuario, idSucursal);
        if (!result) return NotFound(new { message = "Asignación no encontrada." });
        return NoContent();
    }
}
