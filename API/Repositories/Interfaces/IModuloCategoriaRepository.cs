using API.Models;

namespace API.Repositories.Interfaces;

/// <summary>
/// Repositorio para la entidad ModuloCategoria.
/// </summary>
public interface IModuloCategoriaRepository : IRepository<ModuloCategoria>
{
    /// <summary>
    /// Obtiene todas las categorías de un módulo específico.
    /// </summary>
    Task<IEnumerable<ModuloCategoria>> GetByModuloAsync(int idModulo);

    /// <summary>
    /// Obtiene una categoría con su módulo padre cargado.
    /// </summary>
    Task<ModuloCategoria?> GetWithModuloAsync(int idModuloCategoria);
}
