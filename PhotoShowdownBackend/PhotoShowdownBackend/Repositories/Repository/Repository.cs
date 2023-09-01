using System.Linq.Expressions;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PhotoShowdownBackend.Data;

namespace PhotoShowdownBackend.Repositories.Repository;

/// <summary>
/// A generic Data Access Layer (DAL) repository implementation for our database
/// </summary>
/// <typeparam name="T">A model representing a Db table</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    private readonly PhotoShowdownDbContext _db;
    internal DbSet<T> _dbSet;

    private const int MAX_PAGE_SIZE = 100;
    public Repository(PhotoShowdownDbContext db)
    {
        _db = db;
        _dbSet = _db.Set<T>();
    }
    virtual public async Task<T?> GetAsync(Expression<Func<T, bool>> filter, bool tracked = true)
    {
        IQueryable<T> query = _dbSet;
        if (!tracked)
        {
            query = query.AsNoTracking();
        }
 
        return await query.Where(filter).FirstOrDefaultAsync();
    }

    /// <inheritdoc></inheritdoc>
    virtual public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, int? pageNumber = null, int? pageSize = null)
    {
        
        var query = GetAllQuery(filter, tracked, pageNumber, pageSize); 

        return await query.ToListAsync();
    }
    virtual public async Task<T> CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
    virtual public async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Entry(entity).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return entity;
    }
    virtual public async Task<T> DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    virtual public IQueryable<T> GetAllQuery(Expression<Func<T, bool>>? filter = null, bool tracked = true, int? pageNumber = null, int? pageSize = null)
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
        if (pageSize.HasValue && pageNumber.HasValue)
        {
            if (pageSize > MAX_PAGE_SIZE)
            {
                pageSize = MAX_PAGE_SIZE;
            }

            query = query.Skip(pageSize.Value * (pageNumber.Value - 1)).Take(pageSize.Value);
        }



        return query;
    }

}


