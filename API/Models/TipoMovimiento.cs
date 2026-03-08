namespace API.Models;

public class TipoMovimiento
{
    public int IdTipoMovimiento { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool AfectaStock { get; set; }
    public bool EsTransferencia { get; set; }

    // Navegación
    public ICollection<MovimientoInventario> MovimientosInventario { get; set; } = [];
}
