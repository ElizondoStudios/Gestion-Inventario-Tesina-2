using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

/// <summary>
/// Implementación del repositorio para la entidad MovimientoLog (Bitácora de Auditoría).
/// </summary>
public class MovimientoLogRepository : Repository<MovimientoLog>, IMovimientoLogRepository
{
    public MovimientoLogRepository(AppDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<IEnumerable<MovimientoLog>> GetByMovimientoAsync(int idMovimiento)
    {
        return await _dbSet
            .Include(l => l.Usuario)
            .Where(l => l.IdMovimiento == idMovimiento)
            .OrderByDescending(l => l.Fecha)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<MovimientoLog>> GetByUsuarioAsync(int idUsuario)
    {
        return await _dbSet
            .Include(l => l.MovimientoInventario)
            .Where(l => l.IdUsuario == idUsuario)
            .OrderByDescending(l => l.Fecha)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<MovimientoLog>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        return await _dbSet
            .Include(l => l.Usuario)
            .Include(l => l.MovimientoInventario)
            .Where(l => l.Fecha >= fechaInicio && l.Fecha <= fechaFin)
            .OrderByDescending(l => l.Fecha)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<MovimientoLog?> GetWithDetailsAsync(int idLog)
    {
        return await _dbSet
            .Include(l => l.Usuario)
            .Include(l => l.MovimientoInventario)
            .FirstOrDefaultAsync(l => l.IdLog == idLog);
    }
}
