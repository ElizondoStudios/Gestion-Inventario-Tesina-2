using API.Models;

namespace API.Repositories.Interfaces;

/// <summary>
/// Repositorio para la entidad MovimientoInventario.
/// </summary>
public interface IMovimientoInventarioRepository : IRepository<MovimientoInventario>
{
    /// <summary>
    /// Obtiene los movimientos de un producto específico.
    /// </summary>
    Task<IEnumerable<MovimientoInventario>> GetByProductoAsync(int idProducto);

    /// <summary>
    /// Obtiene los movimientos de una sucursal (como origen o destino).
    /// </summary>
    Task<IEnumerable<MovimientoInventario>> GetBySucursalAsync(int idSucursal);

    /// <summary>
    /// Obtiene los movimientos realizados por un usuario.
    /// </summary>
    Task<IEnumerable<MovimientoInventario>> GetByUsuarioAsync(int idUsuario);

    /// <summary>
    /// Obtiene los movimientos filtrados por tipo de movimiento.
    /// </summary>
    Task<IEnumerable<MovimientoInventario>> GetByTipoAsync(int idTipoMovimiento);

    /// <summary>
    /// Obtiene los movimientos en un rango de fechas.
    /// </summary>
    Task<IEnumerable<MovimientoInventario>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin);

    /// <summary>
    /// Obtiene un movimiento con todos sus datos de navegación cargados.
    /// </summary>
    Task<MovimientoInventario?> GetWithDetailsAsync(int idMovimiento);
}
