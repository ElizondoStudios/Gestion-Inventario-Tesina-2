namespace API.Models;

public class Inventario
{
    public int IdInventario { get; set; }
    public int IdProducto { get; set; }
    public int IdSucursal { get; set; }
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }

    // Navegación
    public Producto Producto { get; set; } = null!;
    public Sucursal Sucursal { get; set; } = null!;
}
