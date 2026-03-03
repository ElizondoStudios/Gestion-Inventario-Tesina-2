namespace API.Models;

public class ModuloCategoria
{
    public int IdModuloCategoria { get; set; }
    public int IdModulo { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }

    // Navegación
    public Modulo Modulo { get; set; } = null!;
    public ICollection<RolModuloPermiso> RolModuloPermisos { get; set; } = [];
}
