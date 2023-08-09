using System.Linq.Expressions;

namespace PhotoShowdownBackend.Repositories.Repository;

public interface IRepository<T> where T : class
{
    Task<T?> GetAsync(Expression<Func<T, bool>> filter, bool tracked = true);
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, int pageSize = 0, int pageNumber = 1);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<T> DeleteAsync(T entity);
}
