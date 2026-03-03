using System.Linq.Expressions;
using API.Data;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

/// <summary>
/// Implementación genérica base del repositorio con operaciones CRUD comunes.
/// </summary>
/// <typeparam name="T">Tipo de la entidad.</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    /// <inheritdoc />
    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    /// <inheritdoc />
    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
