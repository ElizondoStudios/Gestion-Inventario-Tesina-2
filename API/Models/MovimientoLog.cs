namespace API.Models;

public class MovimientoLog
{
    public int IdLog { get; set; }
    public int IdMovimiento { get; set; }
    public int IdUsuario { get; set; }
    public string Accion { get; set; } = null!;
    public DateTime Fecha { get; set; }
    public string? ValorAnterior { get; set; }
    public string? ValorNuevo { get; set; }

    // Navegación
    public MovimientoInventario MovimientoInventario { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
