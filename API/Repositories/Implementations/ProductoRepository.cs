using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

/// <summary>
/// Implementación del repositorio para la entidad Producto.
/// </summary>
public class ProductoRepository : Repository<Producto>, IProductoRepository
{
    public ProductoRepository(AppDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<Producto?> GetByNumeroParteAsync(string numeroParte)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.NumeroParte == numeroParte);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Producto>> GetAllActivosAsync()
    {
        return await _dbSet
            .Where(p => p.Activo)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> ExisteNumeroParteAsync(string numeroParte)
    {
        return await _dbSet.AnyAsync(p => p.NumeroParte == numeroParte);
    }
}
