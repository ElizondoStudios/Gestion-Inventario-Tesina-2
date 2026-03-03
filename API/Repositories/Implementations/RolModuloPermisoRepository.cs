using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

/// <summary>
/// Implementación del repositorio para la entidad RolModuloPermiso.
/// </summary>
public class RolModuloPermisoRepository : Repository<RolModuloPermiso>, IRolModuloPermisoRepository
{
    public RolModuloPermisoRepository(AppDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<IEnumerable<RolModuloPermiso>> GetByRolAsync(int idRol)
    {
        return await _dbSet
            .Where(rmp => rmp.IdRol == idRol)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<RolModuloPermiso?> GetByCompositeKeyAsync(int idRol, int idModuloCategoria)
    {
        return await _dbSet
            .FirstOrDefaultAsync(rmp => rmp.IdRol == idRol && rmp.IdModuloCategoria == idModuloCategoria);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RolModuloPermiso>> GetByRolWithDetailsAsync(int idRol)
    {
        return await _dbSet
            .Include(rmp => rmp.ModuloCategoria)
                .ThenInclude(mc => mc.Modulo)
            .Where(rmp => rmp.IdRol == idRol)
            .ToListAsync();
    }
}
