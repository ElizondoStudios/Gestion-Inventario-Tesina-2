using API.DTOs;

namespace API.Services.Interfaces;

/// <summary>
/// Servicio para el control de Inventario.
/// </summary>
public interface IInventarioService
{
    Task<IEnumerable<InventarioDto>> GetAllAsync();
    Task<IEnumerable<InventarioDto>> GetBySucursalAsync(int idSucursal);
    Task<IEnumerable<InventarioDto>> GetByProductoAsync(int idProducto);
    Task<IEnumerable<InventarioDto>> GetAlertasStockBajoAsync();
    Task<IEnumerable<InventarioDto>> GetAlertasStockBajoPorSucursalAsync(int idSucursal);
    Task<bool> UpdateStockMinimoAsync(int idInventario, UpdateStockMinimoDto dto);
}
