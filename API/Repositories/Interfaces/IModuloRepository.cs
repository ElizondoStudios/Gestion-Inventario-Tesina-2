using API.Models;

namespace API.Repositories.Interfaces;

/// <summary>
/// Repositorio para la entidad Modulo.
/// </summary>
public interface IModuloRepository : IRepository<Modulo>
{
    /// <summary>
    /// Obtiene todos los módulos activos.
    /// </summary>
    Task<IEnumerable<Modulo>> GetAllActivosAsync();

    /// <summary>
    /// Obtiene un módulo con sus categorías cargadas.
    /// </summary>
    Task<Modulo?> GetWithCategoriasAsync(int idModulo);
}
