namespace API.Models;

public class Usuario
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public byte[] ContraseniaHash { get; set; } = null!;
    public byte[] ContraseniaSalt { get; set; } = null!;
    public bool Activo { get; set; } = true;
    public int IdRol { get; set; }

    // Navegación
    public Rol Rol { get; set; } = null!;
    public ICollection<UsuarioSucursal> UsuarioSucursales { get; set; } = [];
    public ICollection<MovimientoInventario> MovimientosInventario { get; set; } = [];
    public ICollection<MovimientoLog> MovimientoLogs { get; set; } = [];
}
