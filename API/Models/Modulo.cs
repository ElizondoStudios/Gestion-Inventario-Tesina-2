namespace API.Models;

public class Modulo
{
    public int IdModulo { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;

    // Navegación
    public ICollection<ModuloCategoria> ModuloCategorias { get; set; } = [];
}
