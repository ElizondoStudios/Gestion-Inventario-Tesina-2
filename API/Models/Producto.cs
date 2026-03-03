namespace API.Models;

public class Producto
{
    public int IdProducto { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string NumeroParte { get; set; } = null!;
    public decimal Precio { get; set; }
    public bool Activo { get; set; } = true;

    // Navegación
    public ICollection<Inventario> Inventarios { get; set; } = [];
    public ICollection<MovimientoInventario> MovimientosInventario { get; set; } = [];
}
