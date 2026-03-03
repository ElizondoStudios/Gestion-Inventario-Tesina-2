using API.Models;

namespace API.Repositories.Interfaces;

/// <summary>
/// Repositorio para la entidad Producto.
/// </summary>
public interface IProductoRepository : IRepository<Producto>
{
    /// <summary>
    /// Obtiene un producto por su número de parte.
    /// </summary>
    Task<Producto?> GetByNumeroParteAsync(string numeroParte);

    /// <summary>
    /// Obtiene todos los productos activos.
    /// </summary>
    Task<IEnumerable<Producto>> GetAllActivosAsync();

    /// <summary>
    /// Verifica si un número de parte ya está registrado.
    /// </summary>
    Task<bool> ExisteNumeroParteAsync(string numeroParte);
}
