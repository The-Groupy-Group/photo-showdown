﻿using Microsoft.EntityFrameworkCore;
using PhotoShowdownBackend.Data;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;
using System.Linq.Expressions;

namespace PhotoShowdownBackend.Repositories.Users;

/// <summary>
/// Data Access Layer (DAL) implementation for our Matches
/// </summary>
public class MatchesRepository : Repository<Match>, IMatchesRepository
{
    public MatchesRepository(PhotoShowdownDbContext _db) : base(_db) { }

    virtual public async Task<List<Match>> GetAllWithUsersAsync(
        Expression<Func<Match, bool>>? filter = null, 
        bool tracked = true, 
        int? pageNumber = null, 
        int? pageSize = null)
    {
        IQueryable<Match> query = _dbSet;
        query = query
            .Include(match => match.Owner)
            .Include(match => match.MatchConnections)
            .ThenInclude(mc => mc.User);

        return await GetAllFromQueryAsync<Match>(query, filter,null, tracked, pageNumber, pageSize);
    }

    virtual public async Task<Match?> GetWithUsersAsync(
        Expression<Func<Match, bool>> filter, 
        bool tracked = true)
    {
        IQueryable<Match> query = _dbSet;
        if (!tracked)
        {
            query = query.AsNoTracking();
        }
        return await query
            .Include(match => match.Owner)
            .Include(match => match.MatchConnections)
            .ThenInclude(mc => mc.User)
            .Where(filter)
            .FirstOrDefaultAsync();
    }
}