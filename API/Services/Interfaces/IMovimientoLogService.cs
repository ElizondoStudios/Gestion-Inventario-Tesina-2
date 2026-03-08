using API.DTOs;

namespace API.Services.Interfaces;

/// <summary>
/// Servicio para la consulta de la Bitácora de Auditoría (MovimientoLog).
/// </summary>
public interface IMovimientoLogService
{
    Task<IEnumerable<MovimientoLogDto>> GetAllAsync();
    Task<MovimientoLogDto?> GetByIdAsync(int idLog);
    Task<IEnumerable<MovimientoLogDto>> GetByMovimientoAsync(int idMovimiento);
    Task<IEnumerable<MovimientoLogDto>> GetByUsuarioAsync(int idUsuario);
    Task<IEnumerable<MovimientoLogDto>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin);
}
