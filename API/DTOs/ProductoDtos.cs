using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

// ─── Respuesta ───

/// <summary>
/// DTO de respuesta para Producto.
/// </summary>
public class ProductoDto
{
    public int IdProducto { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string NumeroParte { get; set; } = null!;
    public decimal Precio { get; set; }
    public bool Activo { get; set; }
}

// ─── Solicitud ───

/// <summary>
/// DTO para crear un nuevo Producto.
/// </summary>
public class CreateProductoDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres.")]
    public string Nombre { get; set; } = null!;

    [StringLength(255, ErrorMessage = "La descripción no puede exceder 255 caracteres.")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "El número de parte es obligatorio.")]
    [StringLength(100, ErrorMessage = "El número de parte no puede exceder 100 caracteres.")]
    public string NumeroParte { get; set; } = null!;

    [Required(ErrorMessage = "El precio es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
    public decimal Precio { get; set; }
}

/// <summary>
/// DTO para actualizar un Producto existente.
/// </summary>
public class UpdateProductoDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres.")]
    public string Nombre { get; set; } = null!;

    [StringLength(255, ErrorMessage = "La descripción no puede exceder 255 caracteres.")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "El número de parte es obligatorio.")]
    [StringLength(100, ErrorMessage = "El número de parte no puede exceder 100 caracteres.")]
    public string NumeroParte { get; set; } = null!;

    [Required(ErrorMessage = "El precio es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
    public decimal Precio { get; set; }
}
