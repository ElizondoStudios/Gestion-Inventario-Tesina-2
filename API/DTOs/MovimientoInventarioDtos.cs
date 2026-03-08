using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

// ─── Respuesta ───

/// <summary>
/// DTO de respuesta para Movimiento de Inventario.
/// </summary>
public class MovimientoInventarioDto
{
    public int IdMovimiento { get; set; }
    public int IdProducto { get; set; }
    public string? NombreProducto { get; set; }
    public int? IdSucursalOrigen { get; set; }
    public string? NombreSucursalOrigen { get; set; }
    public int? IdSucursalDestino { get; set; }
    public string? NombreSucursalDestino { get; set; }
    public int IdUsuario { get; set; }
    public string? NombreUsuario { get; set; }
    public int IdTipoMovimiento { get; set; }
    public string? NombreTipoMovimiento { get; set; }
    public int Cantidad { get; set; }
    public DateTime Fecha { get; set; }
    public string? Observaciones { get; set; }
}

// ─── Solicitud ───

/// <summary>
/// DTO para registrar un movimiento de inventario (entrada, salida o transferencia).
/// </summary>
public class CreateMovimientoDto
{
    [Required(ErrorMessage = "El producto es obligatorio.")]
    public int IdProducto { get; set; }

    /// <summary>
    /// Requerido para Salida y Transferencia. NULL para Entrada.
    /// </summary>
    public int? IdSucursalOrigen { get; set; }

    /// <summary>
    /// Requerido para Entrada y Transferencia. NULL para Salida.
    /// </summary>
    public int? IdSucursalDestino { get; set; }

    [Required(ErrorMessage = "El usuario es obligatorio.")]
    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "El tipo de movimiento es obligatorio.")]
    public int IdTipoMovimiento { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
    public int Cantidad { get; set; }

    [StringLength(255, ErrorMessage = "Las observaciones no pueden exceder 255 caracteres.")]
    public string? Observaciones { get; set; }
}
