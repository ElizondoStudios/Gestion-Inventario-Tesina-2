using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

/// <summary>
/// Implementación del repositorio para la entidad Rol.
/// </summary>
public class RolRepository : Repository<Rol>, IRolRepository
{
    public RolRepository(AppDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<Rol?> GetByNombreAsync(string nombre)
    {
        return await _dbSet
            .FirstOrDefaultAsync(r => r.Nombre == nombre);
    }

    /// <inheritdoc />
    public async Task<Rol?> GetWithPermisosAsync(int idRol)
    {
        return await _dbSet
            .Include(r => r.RolModuloPermisos)
                .ThenInclude(rmp => rmp.ModuloCategoria)
                    .ThenInclude(mc => mc.Modulo)
            .FirstOrDefaultAsync(r => r.IdRol == idRol);
    }
}
