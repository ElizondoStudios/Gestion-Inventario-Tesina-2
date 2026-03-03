using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

/// <summary>
/// Implementación del repositorio para la tabla intermedia UsuarioSucursal.
/// </summary>
public class UsuarioSucursalRepository : Repository<UsuarioSucursal>, IUsuarioSucursalRepository
{
    public UsuarioSucursalRepository(AppDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<UsuarioSucursal?> GetByCompositeKeyAsync(int idUsuario, int idSucursal)
    {
        return await _dbSet
            .FirstOrDefaultAsync(us => us.IdUsuario == idUsuario && us.IdSucursal == idSucursal);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UsuarioSucursal>> GetByUsuarioAsync(int idUsuario)
    {
        return await _dbSet
            .Include(us => us.Sucursal)
            .Where(us => us.IdUsuario == idUsuario)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UsuarioSucursal>> GetBySucursalAsync(int idSucursal)
    {
        return await _dbSet
            .Include(us => us.Usuario)
            .Where(us => us.IdSucursal == idSucursal)
            .ToListAsync();
    }
}
