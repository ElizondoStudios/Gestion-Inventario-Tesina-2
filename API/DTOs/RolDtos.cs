using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

// ─── Respuesta ───

/// <summary>
/// DTO de respuesta para Rol.
/// </summary>
public class RolDto
{
    public int IdRol { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
}

/// <summary>
/// DTO de respuesta para Rol con sus permisos detallados.
/// </summary>
public class RolDetalleDto
{
    public int IdRol { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public List<RolModuloPermisoDto> Permisos { get; set; } = [];
}

// ─── Solicitud ───

/// <summary>
/// DTO para crear un nuevo Rol.
/// </summary>
public class CreateRolDto
{
    [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
    public string Nombre { get; set; } = null!;

    [StringLength(255, ErrorMessage = "La descripción no puede exceder 255 caracteres.")]
    public string? Descripcion { get; set; }
}

/// <summary>
/// DTO para actualizar un Rol existente.
/// </summary>
public class UpdateRolDto
{
    [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
    public string Nombre { get; set; } = null!;

    [StringLength(255, ErrorMessage = "La descripción no puede exceder 255 caracteres.")]
    public string? Descripcion { get; set; }
}
