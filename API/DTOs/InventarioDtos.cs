using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

// ─── Respuesta ───

/// <summary>
/// DTO de respuesta para Inventario.
/// </summary>
public class InventarioDto
{
    public int IdInventario { get; set; }
    public int IdProducto { get; set; }
    public string? NombreProducto { get; set; }
    public string? NumeroParte { get; set; }
    public int IdSucursal { get; set; }
    public string? NombreSucursal { get; set; }
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }
    public bool StockBajo { get; set; }
}

// ─── Solicitud ───

/// <summary>
/// DTO para establecer o actualizar el stock mínimo de un producto en una sucursal.
/// </summary>
public class UpdateStockMinimoDto
{
    [Required(ErrorMessage = "El stock mínimo es obligatorio.")]
    [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
    public int StockMinimo { get; set; }
}
