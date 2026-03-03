using API.DTOs;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services.Implementations;

/// <summary>
/// Implementación del servicio para el control de Inventario y alertas de stock bajo (RF4.3).
/// </summary>
public class InventarioService : IInventarioService
{
    private readonly IInventarioRepository _inventarioRepo;

    public InventarioService(IInventarioRepository inventarioRepo)
    {
        _inventarioRepo = inventarioRepo;
    }

    public async Task<IEnumerable<InventarioDto>> GetBySucursalAsync(int idSucursal)
    {
        var inventarios = await _inventarioRepo.GetBySucursalAsync(idSucursal);
        return inventarios.Select(MapToDto);
    }

    public async Task<IEnumerable<InventarioDto>> GetByProductoAsync(int idProducto)
    {
        var inventarios = await _inventarioRepo.GetByProductoAsync(idProducto);
        return inventarios.Select(MapToDto);
    }

    public async Task<IEnumerable<InventarioDto>> GetAlertasStockBajoAsync()
    {
        var alertas = await _inventarioRepo.GetAlertasStockBajoAsync();
        return alertas.Select(MapToDto);
    }

    public async Task<IEnumerable<InventarioDto>> GetAlertasStockBajoPorSucursalAsync(int idSucursal)
    {
        var alertas = await _inventarioRepo.GetAlertasStockBajoPorSucursalAsync(idSucursal);
        return alertas.Select(MapToDto);
    }

    public async Task<bool> UpdateStockMinimoAsync(int idInventario, UpdateStockMinimoDto dto)
    {
        var inventario = await _inventarioRepo.GetByIdAsync(idInventario);
        if (inventario is null) return false;

        inventario.StockMinimo = dto.StockMinimo;
        _inventarioRepo.Update(inventario);
        await _inventarioRepo.SaveChangesAsync();
        return true;
    }

    // ─── Mapeo privado ───

    private static InventarioDto MapToDto(Models.Inventario i) => new()
    {
        IdInventario = i.IdInventario,
        IdProducto = i.IdProducto,
        NombreProducto = i.Producto?.Nombre,
        NumeroParte = i.Producto?.NumeroParte,
        IdSucursal = i.IdSucursal,
        NombreSucursal = i.Sucursal?.Nombre,
        StockActual = i.StockActual,
        StockMinimo = i.StockMinimo,
        StockBajo = i.StockActual <= i.StockMinimo
    };
}
