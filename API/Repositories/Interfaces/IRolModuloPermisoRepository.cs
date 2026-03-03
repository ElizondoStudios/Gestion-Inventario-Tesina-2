using API.Models;

namespace API.Repositories.Interfaces;

/// <summary>
/// Repositorio para la entidad RolModuloPermiso.
/// </summary>
public interface IRolModuloPermisoRepository : IRepository<RolModuloPermiso>
{
    /// <summary>
    /// Obtiene todos los permisos asignados a un rol.
    /// </summary>
    Task<IEnumerable<RolModuloPermiso>> GetByRolAsync(int idRol);

    /// <summary>
    /// Obtiene un permiso por su clave compuesta (IdRol, IdModuloCategoria).
    /// </summary>
    Task<RolModuloPermiso?> GetByCompositeKeyAsync(int idRol, int idModuloCategoria);

    /// <summary>
    /// Obtiene todos los permisos de un rol con las categorías y módulos cargados.
    /// </summary>
    Task<IEnumerable<RolModuloPermiso>> GetByRolWithDetailsAsync(int idRol);
}
