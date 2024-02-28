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
        return await GetAsync(e => e, filter, tracked);
    }
    virtual public async Task<S?> GetAsync<S>(Expression<Func<T, S>> map, Expression<Func<T, bool>> filter, bool tracked = true)
    {
        IQueryable<T> query = _dbSet;
        if (!tracked)
        {
            query = query.AsNoTracking();
        }

        return await query.Where(filter).Select(map).FirstOrDefaultAsync();
    }

    /// <inheritdoc></inheritdoc>
    virtual public async Task<List<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        bool tracked = true,
        int? pageIndex = null,
        int? pageSize = null)
    {

        return await GetAllAsync(e => e, filter, tracked, pageIndex, pageSize);
    }
    /// <inheritdoc></inheritdoc>
    virtual public async Task<List<S>> GetAllAsync<S>(
        Expression<Func<T, S>>? map,
        Expression<Func<T, bool>>? filter = null,
        bool tracked = true,
        int? pageIndex = null,
        int? pageSize = null)
    {
        var query = _dbSet;

        return await GetAllFromQueryAsync(query, filter, map, tracked, pageIndex, pageSize);
    }
    virtual public async Task<T> CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
    virtual public async Task CreateRangeAsync(T[] entities)
    {
        await _dbSet.AddRangeAsync(entities);
        await _db.SaveChangesAsync();
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
    virtual public async Task DeleteRangeAsync(T[] entities)
    {
        _dbSet.RemoveRange(entities);
        await _db.SaveChangesAsync();
    }
    virtual public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }
    virtual internal async Task<List<S>> GetAllFromQueryAsync<S>(
        IQueryable<T> query,
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, S>>? map = null,
        bool tracked = true,
        int? pageIndex = null,
        int? pageSize = null)
    {
        if (filter is not null)
        {
            query = query.Where(filter);
        }
        if (!tracked)
        {
            query = query.AsNoTracking();
        }
        if (pageSize.HasValue && pageIndex.HasValue)
        {
            if (pageSize > MAX_PAGE_SIZE)
            {
                pageSize = MAX_PAGE_SIZE;
            }

            query = query.Skip(pageSize.Value * (pageIndex.Value)).Take(pageSize.Value);
        }
        IQueryable<S> mappedQuery;
        if (map is null)
        {
            map = e => (S)(object)e;
        }
        mappedQuery = query.Select(map);

        return await mappedQuery.ToListAsync();
    }
}