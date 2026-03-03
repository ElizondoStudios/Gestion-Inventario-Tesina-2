using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

// ─── Respuesta ───

/// <summary>
/// DTO de respuesta para Módulo.
/// </summary>
public class ModuloDto
{
    public int IdModulo { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
}

/// <summary>
/// DTO de respuesta para Módulo con sus categorías.
/// </summary>
public class ModuloDetalleDto
{
    public int IdModulo { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public List<ModuloCategoriaDto> Categorias { get; set; } = [];
}

/// <summary>
/// DTO de respuesta para Categoría de Módulo.
/// </summary>
public class ModuloCategoriaDto
{
    public int IdModuloCategoria { get; set; }
    public int IdModulo { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? NombreModulo { get; set; }
}

/// <summary>
/// DTO de respuesta para los permisos de un rol sobre una categoría.
/// </summary>
public class RolModuloPermisoDto
{
    public int IdRol { get; set; }
    public int IdModuloCategoria { get; set; }
    public string? NombreModulo { get; set; }
    public string? NombreCategoria { get; set; }
    public bool PuedeLeer { get; set; }
    public bool PuedeEscribir { get; set; }
    public bool PuedeEliminar { get; set; }
}

// ─── Solicitud ───

/// <summary>
/// DTO para crear una categoría de módulo.
/// </summary>
public class CreateModuloCategoriaDto
{
    [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
    public string Nombre { get; set; } = null!;

    [StringLength(255, ErrorMessage = "La descripción no puede exceder 255 caracteres.")]
    public string? Descripcion { get; set; }
}

/// <summary>
/// DTO para actualizar una categoría de módulo.
/// </summary>
public class UpdateModuloCategoriaDto
{
    [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
    public string Nombre { get; set; } = null!;

    [StringLength(255, ErrorMessage = "La descripción no puede exceder 255 caracteres.")]
    public string? Descripcion { get; set; }
}

/// <summary>
/// DTO para asignar o actualizar permisos de un rol sobre una categoría.
/// </summary>
public class AsignarPermisoDto
{
    [Required(ErrorMessage = "El Id de la categoría es obligatorio.")]
    public int IdModuloCategoria { get; set; }

    public bool PuedeLeer { get; set; }
    public bool PuedeEscribir { get; set; }
    public bool PuedeEliminar { get; set; }
}
