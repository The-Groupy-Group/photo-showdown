using System.Linq.Expressions;

namespace PhotoShowdownBackend.Repositories.Repository;

/// <summary>
/// A generic Data Access Layer (DAL) repository interface for our database
/// </summary>
/// <typeparam name="T">A model representing a Db table</typeparam>
public interface IRepository<T> where T : class
{
    Task<T?> GetAsync(Expression<Func<T, bool>> filter, bool tracked = true);
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, int pageSize = 0, int pageNumber = 1);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<T> DeleteAsync(T entity);
}
