using System.Linq.Expressions;

namespace PhotoShowdownBackend.Repositories.Repository;

/// <summary>
/// A generic Data Access Layer (DAL) repository interface for our database
/// </summary>
/// <typeparam name="T">A model representing a Db table</typeparam>
public interface IRepository<T> where T : class
{
    public Task<T?> GetAsync(Expression<Func<T, bool>> filter, bool tracked = true);
    public Task<S?> GetAsync<S>(Expression<Func<T, S>> map, Expression<Func<T, bool>> filter, bool tracked = true);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="tracked"></param>
    /// <param name="pageIndex">Starting from 0</param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public Task<List<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        bool tracked = true,
        int? pageIndex = null,
        int? pageSize = null);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="map"></param>
    /// <param name="tracked"></param>
    /// <param name="pageIndex">Starting from 0</param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public Task<List<S>> GetAllAsync<S>(
        Expression<Func<T, S>> map,
        Expression<Func<T, bool>>? filter = null,
        bool tracked = true,
        int? pageIndex = null,
        int? pageSize = null);
    public Task<T> CreateAsync(T entity);
    public Task<T> UpdateAsync(T entity);
    public Task<T> DeleteAsync(T entity);
    public Task DeleteRangeAsync(T[] entities);
    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
}
