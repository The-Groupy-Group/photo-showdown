using System.Linq.Expressions;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PhotoShowdownBackend.Repositories.Repository;

public abstract class Repository<T> : IRepository<T> where T : class
{
    private readonly PhotoShowdownDbContext _db;
    internal DbSet<T> _dbSet;

    private const int MAX_PAGE_SIZE = 100;
    public Repository(PhotoShowdownDbContext db)
    {
        _db = db;
        _dbSet = _db.Set<T>();
    }
    public async Task<T?> GetAsync(Expression<Func<T, bool>> filter, bool tracked = true)
    {
        IQueryable<T> query = _dbSet;
        if (!tracked)
        {
            query = query.AsNoTracking();
        }
 
        return await query.Where(filter).FirstOrDefaultAsync();
    }

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, int pageSize = 0, int pageNumber = 1)
    {
        IQueryable<T> query = _dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }
        if (!tracked)
        {
            query = query.AsNoTracking();
        }
        if (pageSize > 0)
        {
            if (pageSize > MAX_PAGE_SIZE)
            {
                pageSize = MAX_PAGE_SIZE;
            }

            query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
        }

        return await query.ToListAsync();
    }
    public async Task CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _db.SaveChangesAsync();
    }
    public async Task UpdateAsync(T entity)
    {
        _dbSet.Entry(entity).State = EntityState.Modified;
        await _db.SaveChangesAsync();
    }
    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _db.SaveChangesAsync();
    }
}
