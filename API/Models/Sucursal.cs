namespace API.Models;

public class Sucursal
{
    public int IdSucursal { get; set; }
    public string Nombre { get; set; } = null!;
    public string Direccion { get; set; } = null!;
    public string Ciudad { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public bool Activa { get; set; } = true;

    // Navegación
    public ICollection<UsuarioSucursal> UsuarioSucursales { get; set; } = [];
    public ICollection<Inventario> Inventarios { get; set; } = [];
    public ICollection<MovimientoInventario> MovimientosOrigen { get; set; } = [];
    public ICollection<MovimientoInventario> MovimientosDestino { get; set; } = [];
}
