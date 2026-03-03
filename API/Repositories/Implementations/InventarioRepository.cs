using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

/// <summary>
/// Implementación del repositorio para la entidad Inventario.
/// </summary>
public class InventarioRepository : Repository<Inventario>, IInventarioRepository
{
    public InventarioRepository(AppDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<Inventario?> GetByProductoYSucursalAsync(int idProducto, int idSucursal)
    {
        return await _dbSet
            .Include(i => i.Producto)
            .Include(i => i.Sucursal)
            .FirstOrDefaultAsync(i => i.IdProducto == idProducto && i.IdSucursal == idSucursal);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Inventario>> GetBySucursalAsync(int idSucursal)
    {
        return await _dbSet
            .Include(i => i.Producto)
            .Where(i => i.IdSucursal == idSucursal)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Inventario>> GetByProductoAsync(int idProducto)
    {
        return await _dbSet
            .Include(i => i.Sucursal)
            .Where(i => i.IdProducto == idProducto)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Inventario>> GetAlertasStockBajoAsync()
    {
        return await _dbSet
            .Include(i => i.Producto)
            .Include(i => i.Sucursal)
            .Where(i => i.StockActual <= i.StockMinimo)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Inventario>> GetAlertasStockBajoPorSucursalAsync(int idSucursal)
    {
        return await _dbSet
            .Include(i => i.Producto)
            .Include(i => i.Sucursal)
            .Where(i => i.IdSucursal == idSucursal && i.StockActual <= i.StockMinimo)
            .ToListAsync();
    }
}
