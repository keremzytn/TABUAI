using System.Linq.Expressions;

namespace TabuAI.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllNoTrackingAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);
    Task<IEnumerable<T>> FindNoTrackingAsync(Expression<Func<T, bool>> expression);
    Task<T?> FindFirstAsync(Expression<Func<T, bool>> expression);
    Task<T?> FindFirstNoTrackingAsync(Expression<Func<T, bool>> expression);
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync<TKey>(
        int page,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, TKey>>? orderBy = null,
        bool descending = false);
    IQueryable<T> AsQueryable();
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task DeleteAsync(Guid id);
    Task<int> CountAsync();
    Task<int> CountAsync(Expression<Func<T, bool>> expression);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> expression);
}