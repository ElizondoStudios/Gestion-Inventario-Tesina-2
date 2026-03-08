namespace API.Models;

public class MovimientoInventario
{
    public int IdMovimiento { get; set; }
    public int IdProducto { get; set; }
    public int? IdSucursalOrigen { get; set; }
    public int? IdSucursalDestino { get; set; }
    public int IdUsuario { get; set; }
    public int IdTipoMovimiento { get; set; }
    public int Cantidad { get; set; }
    public DateTime Fecha { get; set; }
    public string? Observaciones { get; set; }

    // Navegación
    public Producto Producto { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
    public TipoMovimiento TipoMovimiento { get; set; } = null!;
    public Sucursal? SucursalOrigen { get; set; }
    public Sucursal? SucursalDestino { get; set; }
    public ICollection<MovimientoLog> MovimientoLogs { get; set; } = [];
}
