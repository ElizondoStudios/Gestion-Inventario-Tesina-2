using API.Models;

namespace API.Repositories.Interfaces;

/// <summary>
/// Repositorio para la entidad Sucursal.
/// </summary>
public interface ISucursalRepository : IRepository<Sucursal>
{
    /// <summary>
    /// Obtiene todas las sucursales activas.
    /// </summary>
    Task<IEnumerable<Sucursal>> GetAllActivasAsync();

    /// <summary>
    /// Obtiene una sucursal con sus usuarios asignados.
    /// </summary>
    Task<Sucursal?> GetWithUsuariosAsync(int idSucursal);

    /// <summary>
    /// Obtiene una sucursal con su inventario cargado.
    /// </summary>
    Task<Sucursal?> GetWithInventarioAsync(int idSucursal);
}
