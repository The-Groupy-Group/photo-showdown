using Microsoft.EntityFrameworkCore;
using PhotoShowdownBackend.Data;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;

namespace PhotoShowdownBackend.Repositories.MatchConnections;

public class MatchConnectionsRepository : Repository<MatchConnection>, IMatchConnectionsRepository
{
    public MatchConnectionsRepository(PhotoShowdownDbContext _db) : base(_db)
    {
    }

    public async Task<bool> UserConnectedToMatch(int userId)
    {
        return await _dbSet.AnyAsync(mc => mc.UserId == userId);
    }

    public async Task<bool> IsMatchEmpty(int matchId)
    {
        return !await _dbSet.AnyAsync(mc => mc.MatchId == matchId);
    }

    public async Task<bool> IsUserInThisMatch(int userId, int matchId)
    {
        return await _dbSet.AnyAsync(mc =>mc.UserId == userId && mc.MatchId == matchId);
    }
}
