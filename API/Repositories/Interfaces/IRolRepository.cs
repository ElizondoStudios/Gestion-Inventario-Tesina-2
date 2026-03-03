using API.Models;

namespace API.Repositories.Interfaces;

/// <summary>
/// Repositorio para la entidad Rol.
/// </summary>
public interface IRolRepository : IRepository<Rol>
{
    /// <summary>
    /// Obtiene un rol por su nombre.
    /// </summary>
    Task<Rol?> GetByNombreAsync(string nombre);

    /// <summary>
    /// Obtiene un rol incluyendo sus permisos asociados.
    /// </summary>
    Task<Rol?> GetWithPermisosAsync(int idRol);
}
