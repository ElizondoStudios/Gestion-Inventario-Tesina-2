namespace API.Models;

public class Rol
{
    public int IdRol { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }

    // Navegación
    public ICollection<Usuario> Usuarios { get; set; } = [];
    public ICollection<RolModuloPermiso> RolModuloPermisos { get; set; } = [];
}
