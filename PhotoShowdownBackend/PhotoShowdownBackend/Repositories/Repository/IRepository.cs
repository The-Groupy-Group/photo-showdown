using System.Linq.Expressions;

namespace PhotoShowdownBackend.Repositories.Repository;

/// <summary>
/// A generic Data Access Layer (DAL) repository interface for our database
/// </summary>
/// <typeparam name="T">A model representing a Db table</typeparam>
public interface IRepository<T> where T : class
{
    public Task<T?> GetAsync(Expression<Func<T, bool>> filter, bool tracked = true);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="tracked"></param>
    /// <param name="pageNumber">Starting from 1</param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, int? pageNumber = null, int? pageSize = null);
    public Task<T> CreateAsync(T entity);
    public Task<T> UpdateAsync(T entity);
    public Task<T> DeleteAsync(T entity);
}
