namespace API.Models;

public class RolModuloPermiso
{
    public int IdRol { get; set; }
    public int IdModuloCategoria { get; set; }
    public bool PuedeLeer { get; set; }
    public bool PuedeEscribir { get; set; }
    public bool PuedeEliminar { get; set; }

    // Navegación
    public Rol Rol { get; set; } = null!;
    public ModuloCategoria ModuloCategoria { get; set; } = null!;
}
