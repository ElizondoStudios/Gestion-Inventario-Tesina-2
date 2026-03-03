using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

/// <summary>
/// Implementación del repositorio para la entidad Sucursal.
/// </summary>
public class SucursalRepository : Repository<Sucursal>, ISucursalRepository
{
    public SucursalRepository(AppDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<IEnumerable<Sucursal>> GetAllActivasAsync()
    {
        return await _dbSet
            .Where(s => s.Activa)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Sucursal?> GetWithUsuariosAsync(int idSucursal)
    {
        return await _dbSet
            .Include(s => s.UsuarioSucursales)
                .ThenInclude(us => us.Usuario)
            .FirstOrDefaultAsync(s => s.IdSucursal == idSucursal);
    }

    /// <inheritdoc />
    public async Task<Sucursal?> GetWithInventarioAsync(int idSucursal)
    {
        return await _dbSet
            .Include(s => s.Inventarios)
                .ThenInclude(i => i.Producto)
            .FirstOrDefaultAsync(s => s.IdSucursal == idSucursal);
    }
}
