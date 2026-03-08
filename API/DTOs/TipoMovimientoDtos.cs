using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

// ─── Respuesta ───

/// <summary>
/// DTO de respuesta para TipoMovimiento.
/// </summary>
public class TipoMovimientoDto
{
    public int IdTipoMovimiento { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool AfectaStock { get; set; }
    public bool EsTransferencia { get; set; }
}

// ─── Solicitud ───

/// <summary>
/// DTO para crear un nuevo TipoMovimiento.
/// </summary>
public class CreateTipoMovimientoDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
    public string Nombre { get; set; } = null!;

    [StringLength(255, ErrorMessage = "La descripción no puede exceder 255 caracteres.")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "Debe indicar si afecta stock.")]
    public bool AfectaStock { get; set; }

    [Required(ErrorMessage = "Debe indicar si es transferencia.")]
    public bool EsTransferencia { get; set; }
}

/// <summary>
/// DTO para actualizar un TipoMovimiento existente.
/// </summary>
public class UpdateTipoMovimientoDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
    public string Nombre { get; set; } = null!;

    [StringLength(255, ErrorMessage = "La descripción no puede exceder 255 caracteres.")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "Debe indicar si afecta stock.")]
    public bool AfectaStock { get; set; }

    [Required(ErrorMessage = "Debe indicar si es transferencia.")]
    public bool EsTransferencia { get; set; }
}
