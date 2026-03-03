using System.Linq.Expressions;

namespace API.Repositories.Interfaces;

/// <summary>
/// Interfaz genérica base para operaciones CRUD comunes sobre cualquier entidad.
/// </summary>
/// <typeparam name="T">Tipo de la entidad.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Obtiene todas las entidades.
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Obtiene una entidad por su clave primaria.
    /// </summary>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Busca entidades que cumplan con un predicado.
    /// </summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Agrega una nueva entidad.
    /// </summary>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Actualiza una entidad existente.
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Elimina una entidad.
    /// </summary>
    void Remove(T entity);

    /// <summary>
    /// Persiste todos los cambios pendientes en la base de datos.
    /// </summary>
    Task<int> SaveChangesAsync();
}
