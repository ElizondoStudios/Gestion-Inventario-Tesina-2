using API.Models;

namespace API.Repositories.Interfaces;

/// <summary>
/// Repositorio para la tabla intermedia UsuarioSucursal.
/// </summary>
public interface IUsuarioSucursalRepository : IRepository<UsuarioSucursal>
{
    /// <summary>
    /// Obtiene la asignación por clave compuesta (IdUsuario, IdSucursal).
    /// </summary>
    Task<UsuarioSucursal?> GetByCompositeKeyAsync(int idUsuario, int idSucursal);

    /// <summary>
    /// Obtiene todas las sucursales asignadas a un usuario.
    /// </summary>
    Task<IEnumerable<UsuarioSucursal>> GetByUsuarioAsync(int idUsuario);

    /// <summary>
    /// Obtiene todos los usuarios asignados a una sucursal.
    /// </summary>
    Task<IEnumerable<UsuarioSucursal>> GetBySucursalAsync(int idSucursal);
}
