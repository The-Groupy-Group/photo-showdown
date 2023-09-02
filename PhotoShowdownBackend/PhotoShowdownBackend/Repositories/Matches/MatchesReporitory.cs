using Microsoft.EntityFrameworkCore;
using PhotoShowdownBackend.Data;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;
using System.Linq.Expressions;

namespace PhotoShowdownBackend.Repositories.Users;

/// <summary>
/// Data Access Layer (DAL) implementation for our Matches
/// </summary>
public class MatchesReporitory: Repository<Match>, IMatchesReporitory
{
    public MatchesReporitory(PhotoShowdownDbContext _db) : base(_db)
    {
    }

    virtual public async Task<List<Match>> GetAllWithMatchConnectionsAsync(Expression<Func<Match, bool>>? filter = null, bool tracked = true, int? pageNumber = null, int? pageSize = null)
    {
        var query = _dbSet;
        query.Include(match => match.MatchConnections).ThenInclude(mc => mc.User);

        return await GetAllFromQueryAsync(query, filter, tracked, pageNumber, pageSize);

    }

    public async Task<bool> DoesMatchExists(int matchId)
    {
        return !(await GetAsync(match => match.Id == matchId) == null);
        
    }
}

