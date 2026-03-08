using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

/// <summary>
/// Implementación del repositorio para la entidad TipoMovimiento.
/// </summary>
public class TipoMovimientoRepository : Repository<TipoMovimiento>, ITipoMovimientoRepository
{
    public TipoMovimientoRepository(AppDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<TipoMovimiento?> GetByNombreAsync(string nombre)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.Nombre == nombre);
    }

    /// <inheritdoc />
    public async Task<bool> ExisteNombreAsync(string nombre)
    {
        return await _dbSet.AnyAsync(t => t.Nombre == nombre);
    }
}
