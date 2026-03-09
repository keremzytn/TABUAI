using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TabuAI.Domain.Interfaces;
using TabuAI.Infrastructure.Data;

namespace TabuAI.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly TabuAIDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(TabuAIDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> GetAllNoTrackingAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
    {
        return await _dbSet.Where(expression).ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindNoTrackingAsync(Expression<Func<T, bool>> expression)
    {
        return await _dbSet.AsNoTracking().Where(expression).ToListAsync();
    }

    public virtual async Task<T?> FindFirstAsync(Expression<Func<T, bool>> expression)
    {
        return await _dbSet.FirstOrDefaultAsync(expression);
    }

    public virtual async Task<T?> FindFirstNoTrackingAsync(Expression<Func<T, bool>> expression)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(expression);
    }

    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync<TKey>(
        int page,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, TKey>>? orderBy = null,
        bool descending = false)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        var totalCount = await query.CountAsync();

        if (orderBy != null)
            query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, totalCount);
    }

    public virtual IQueryable<T> AsQueryable()
    {
        return _dbSet.AsNoTracking().AsQueryable();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return await Task.FromResult(entity);
    }

    public virtual async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public virtual async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> expression)
    {
        return await _dbSet.CountAsync(expression);
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> expression)
    {
        return await _dbSet.AnyAsync(expression);
    }
}