namespace API.DTOs;

// ─── Respuesta ───

/// <summary>
/// DTO de respuesta para MovimientoLog (Bitácora de Auditoría).
/// </summary>
public class MovimientoLogDto
{
    public int IdLog { get; set; }
    public int IdMovimiento { get; set; }
    public int IdUsuario { get; set; }
    public string? NombreUsuario { get; set; }
    public string Accion { get; set; } = null!;
    public DateTime Fecha { get; set; }
    public string? ValorAnterior { get; set; }
    public string? ValorNuevo { get; set; }
}
