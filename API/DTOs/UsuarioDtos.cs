using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

// ─── Respuesta ───

/// <summary>
/// DTO de respuesta para Usuario.
/// </summary>
public class UsuarioDto
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public bool Activo { get; set; }
    public int IdRol { get; set; }
    public string? NombreRol { get; set; }
}

/// <summary>
/// DTO de respuesta para Usuario con sus sucursales asignadas.
/// </summary>
public class UsuarioDetalleDto
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public bool Activo { get; set; }
    public int IdRol { get; set; }
    public string? NombreRol { get; set; }
    public List<SucursalDto> Sucursales { get; set; } = [];
}

/// <summary>
/// DTO de respuesta para inicio de sesión.
/// </summary>
public class LoginResponseDto
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public string NombreRol { get; set; } = null!;
    public List<RolModuloPermisoDto> Permisos { get; set; } = [];
}

// ─── Solicitud ───

/// <summary>
/// DTO para crear un nuevo Usuario.
/// </summary>
public class CreateUsuarioDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
    [StringLength(150, ErrorMessage = "El correo no puede exceder 150 caracteres.")]
    public string Correo { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    public string Contrasenia { get; set; } = null!;

    [Required(ErrorMessage = "El rol es obligatorio.")]
    public int IdRol { get; set; }
}

/// <summary>
/// DTO para actualizar un Usuario existente.
/// </summary>
public class UpdateUsuarioDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
    [StringLength(150, ErrorMessage = "El correo no puede exceder 150 caracteres.")]
    public string Correo { get; set; } = null!;

    [Required(ErrorMessage = "El rol es obligatorio.")]
    public int IdRol { get; set; }
}

/// <summary>
/// DTO para iniciar sesión.
/// </summary>
public class LoginDto
{
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
    public string Correo { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string Contrasenia { get; set; } = null!;
}

/// <summary>
/// DTO para asignar un usuario a una sucursal.
/// </summary>
public class AsignarSucursalDto
{
    [Required(ErrorMessage = "El Id de la sucursal es obligatorio.")]
    public int IdSucursal { get; set; }
}
