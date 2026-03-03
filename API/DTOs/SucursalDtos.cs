using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

// ─── Respuesta ───

/// <summary>
/// DTO de respuesta para Sucursal.
/// </summary>
public class SucursalDto
{
    public int IdSucursal { get; set; }
    public string Nombre { get; set; } = null!;
    public string Direccion { get; set; } = null!;
    public string Ciudad { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public bool Activa { get; set; }
}

// ─── Solicitud ───

/// <summary>
/// DTO para crear una nueva Sucursal.
/// </summary>
public class CreateSucursalDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "La dirección es obligatoria.")]
    [StringLength(255, ErrorMessage = "La dirección no puede exceder 255 caracteres.")]
    public string Direccion { get; set; } = null!;

    [Required(ErrorMessage = "La ciudad es obligatoria.")]
    [StringLength(100, ErrorMessage = "La ciudad no puede exceder 100 caracteres.")]
    public string Ciudad { get; set; } = null!;

    [Required(ErrorMessage = "El estado es obligatorio.")]
    [StringLength(100, ErrorMessage = "El estado no puede exceder 100 caracteres.")]
    public string Estado { get; set; } = null!;
}

/// <summary>
/// DTO para actualizar una Sucursal existente.
/// </summary>
public class UpdateSucursalDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "La dirección es obligatoria.")]
    [StringLength(255, ErrorMessage = "La dirección no puede exceder 255 caracteres.")]
    public string Direccion { get; set; } = null!;

    [Required(ErrorMessage = "La ciudad es obligatoria.")]
    [StringLength(100, ErrorMessage = "La ciudad no puede exceder 100 caracteres.")]
    public string Ciudad { get; set; } = null!;

    [Required(ErrorMessage = "El estado es obligatorio.")]
    [StringLength(100, ErrorMessage = "El estado no puede exceder 100 caracteres.")]
    public string Estado { get; set; } = null!;
}
