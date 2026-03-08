using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

/// <summary>
/// Implementación del repositorio para la entidad MovimientoInventario.
/// </summary>
public class MovimientoInventarioRepository : Repository<MovimientoInventario>, IMovimientoInventarioRepository
{
    public MovimientoInventarioRepository(AppDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<IEnumerable<MovimientoInventario>> GetByProductoAsync(int idProducto)
    {
        return await _dbSet
            .Include(m => m.Usuario)
            .Include(m => m.TipoMovimiento)
            .Include(m => m.SucursalOrigen)
            .Include(m => m.SucursalDestino)
            .Where(m => m.IdProducto == idProducto)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<MovimientoInventario>> GetBySucursalAsync(int idSucursal)
    {
        return await _dbSet
            .Include(m => m.Producto)
            .Include(m => m.Usuario)
            .Include(m => m.TipoMovimiento)
            .Include(m => m.SucursalOrigen)
            .Include(m => m.SucursalDestino)
            .Where(m => m.IdSucursalOrigen == idSucursal || m.IdSucursalDestino == idSucursal)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<MovimientoInventario>> GetByUsuarioAsync(int idUsuario)
    {
        return await _dbSet
            .Include(m => m.Producto)
            .Include(m => m.TipoMovimiento)
            .Include(m => m.SucursalOrigen)
            .Include(m => m.SucursalDestino)
            .Where(m => m.IdUsuario == idUsuario)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<MovimientoInventario>> GetByTipoAsync(int idTipoMovimiento)
    {
        return await _dbSet
            .Include(m => m.Producto)
            .Include(m => m.Usuario)
            .Include(m => m.TipoMovimiento)
            .Include(m => m.SucursalOrigen)
            .Include(m => m.SucursalDestino)
            .Where(m => m.IdTipoMovimiento == idTipoMovimiento)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<MovimientoInventario>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        return await _dbSet
            .Include(m => m.Producto)
            .Include(m => m.Usuario)
            .Include(m => m.TipoMovimiento)
            .Include(m => m.SucursalOrigen)
            .Include(m => m.SucursalDestino)
            .Where(m => m.Fecha >= fechaInicio && m.Fecha <= fechaFin)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<MovimientoInventario?> GetWithDetailsAsync(int idMovimiento)
    {
        return await _dbSet
            .Include(m => m.Producto)
            .Include(m => m.Usuario)
            .Include(m => m.TipoMovimiento)
            .Include(m => m.SucursalOrigen)
            .Include(m => m.SucursalDestino)
            .FirstOrDefaultAsync(m => m.IdMovimiento == idMovimiento);
    }
}
