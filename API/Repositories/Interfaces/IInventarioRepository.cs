using API.Models;

namespace API.Repositories.Interfaces;

/// <summary>
/// Repositorio para la entidad Inventario.
/// </summary>
public interface IInventarioRepository : IRepository<Inventario>
{
    /// <summary>
    /// Obtiene el inventario de un producto en una sucursal específica.
    /// </summary>
    Task<Inventario?> GetByProductoYSucursalAsync(int idProducto, int idSucursal);

    /// <summary>
    /// Obtiene todo el inventario de una sucursal con los datos del producto.
    /// </summary>
    Task<IEnumerable<Inventario>> GetBySucursalAsync(int idSucursal);

    /// <summary>
    /// Obtiene todo el inventario de un producto en todas las sucursales.
    /// </summary>
    Task<IEnumerable<Inventario>> GetByProductoAsync(int idProducto);

    /// <summary>
    /// Obtiene los registros de inventario cuyo stock actual es menor o igual al stock mínimo (alertas).
    /// </summary>
    Task<IEnumerable<Inventario>> GetAlertasStockBajoAsync();

    /// <summary>
    /// Obtiene las alertas de stock bajo para una sucursal específica.
    /// </summary>
    Task<IEnumerable<Inventario>> GetAlertasStockBajoPorSucursalAsync(int idSucursal);
}
