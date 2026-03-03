namespace API.Models;

public class UsuarioSucursal
{
    public int IdUsuario { get; set; }
    public int IdSucursal { get; set; }

    // Navegación
    public Usuario Usuario { get; set; } = null!;
    public Sucursal Sucursal { get; set; } = null!;
}
