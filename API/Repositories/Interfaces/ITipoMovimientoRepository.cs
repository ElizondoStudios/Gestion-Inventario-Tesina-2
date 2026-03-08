using API.Models;

namespace API.Repositories.Interfaces;

/// <summary>
/// Repositorio para la entidad TipoMovimiento.
/// </summary>
public interface ITipoMovimientoRepository : IRepository<TipoMovimiento>
{
    /// <summary>
    /// Obtiene un tipo de movimiento por su nombre.
    /// </summary>
    Task<TipoMovimiento?> GetByNombreAsync(string nombre);

    /// <summary>
    /// Verifica si ya existe un tipo de movimiento con el nombre indicado.
    /// </summary>
    Task<bool> ExisteNombreAsync(string nombre);
}
