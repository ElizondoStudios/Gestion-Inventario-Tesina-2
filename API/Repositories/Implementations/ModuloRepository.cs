using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

/// <summary>
/// Implementación del repositorio para la entidad Modulo.
/// </summary>
public class ModuloRepository : Repository<Modulo>, IModuloRepository
{
    public ModuloRepository(AppDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<IEnumerable<Modulo>> GetAllActivosAsync()
    {
        return await _dbSet
            .Where(m => m.Activo)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Modulo?> GetWithCategoriasAsync(int idModulo)
    {
        return await _dbSet
            .Include(m => m.ModuloCategorias)
            .FirstOrDefaultAsync(m => m.IdModulo == idModulo);
    }
}
