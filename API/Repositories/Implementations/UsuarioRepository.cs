using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

/// <summary>
/// Implementación del repositorio para la entidad Usuario.
/// </summary>
public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(AppDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<Usuario?> GetByCorreoAsync(string correo)
    {
        return await _dbSet
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Correo == correo);
    }

    /// <inheritdoc />
    public async Task<Usuario?> GetWithRolAsync(int idUsuario)
    {
        return await _dbSet
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
    }

    /// <inheritdoc />
    public async Task<Usuario?> GetWithSucursalesAsync(int idUsuario)
    {
        return await _dbSet
            .Include(u => u.UsuarioSucursales)
                .ThenInclude(us => us.Sucursal)
            .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Usuario>> GetAllActivosAsync()
    {
        return await _dbSet
            .Include(u => u.Rol)
            .Where(u => u.Activo)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> ExisteCorreoAsync(string correo)
    {
        return await _dbSet.AnyAsync(u => u.Correo == correo);
    }
}
