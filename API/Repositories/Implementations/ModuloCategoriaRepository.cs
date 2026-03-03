using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

/// <summary>
/// Implementación del repositorio para la entidad ModuloCategoria.
/// </summary>
public class ModuloCategoriaRepository : Repository<ModuloCategoria>, IModuloCategoriaRepository
{
    public ModuloCategoriaRepository(AppDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<IEnumerable<ModuloCategoria>> GetByModuloAsync(int idModulo)
    {
        return await _dbSet
            .Where(mc => mc.IdModulo == idModulo)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ModuloCategoria?> GetWithModuloAsync(int idModuloCategoria)
    {
        return await _dbSet
            .Include(mc => mc.Modulo)
            .FirstOrDefaultAsync(mc => mc.IdModuloCategoria == idModuloCategoria);
    }
}
