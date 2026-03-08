using API.Models;

namespace API.Repositories.Interfaces;

/// <summary>
/// Repositorio para la entidad MovimientoLog (Bitácora de Auditoría).
/// </summary>
public interface IMovimientoLogRepository : IRepository<MovimientoLog>
{
    /// <summary>
    /// Obtiene los logs de un movimiento específico.
    /// </summary>
    Task<IEnumerable<MovimientoLog>> GetByMovimientoAsync(int idMovimiento);

    /// <summary>
    /// Obtiene los logs realizados por un usuario específico.
    /// </summary>
    Task<IEnumerable<MovimientoLog>> GetByUsuarioAsync(int idUsuario);

    /// <summary>
    /// Obtiene los logs en un rango de fechas.
    /// </summary>
    Task<IEnumerable<MovimientoLog>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin);

    /// <summary>
    /// Obtiene un log con sus datos de navegación cargados.
    /// </summary>
    Task<MovimientoLog?> GetWithDetailsAsync(int idLog);
}
