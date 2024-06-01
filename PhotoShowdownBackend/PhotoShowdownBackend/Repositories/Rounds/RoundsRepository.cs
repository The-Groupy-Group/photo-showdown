using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using PhotoShowdownBackend.Data;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;
using System.Linq.Expressions;

namespace PhotoShowdownBackend.Repositories.Rounds;

public class RoundsRepository : Repository<Round>, IRoundsRepository
{
    public RoundsRepository(PhotoShowdownDbContext dbContext) : base(dbContext)
    {
        
    }

    virtual public async Task<Round?> GetLastRoundByMatchIdWithIncludes(
        int matchId,
        bool tracked = true
        )
    {
        IQueryable<Round> query = _dbSet;
        if (!tracked)
        {
            query = query.AsNoTracking();
        }
        return await query
            .Include(round => round.Match)
            .Include(round => round.Winner)
            .Include(round => round.RoundPictures)
            .ThenInclude(rp => rp.RoundVotes)
            .Include(round => round.RoundPictures)
            .ThenInclude(rp => rp.Picture)
            .Include(round => round.RoundPictures)
            .ThenInclude(rp => rp.User)
            .Where(round => round.MatchId == matchId)
            .OrderBy(round => round.RoundIndex)
            .LastOrDefaultAsync();

    }

    virtual public async Task<Round?> GetWithIncludesAsync(
        Expression<Func<Round, bool>> filter,
        bool tracked = true
        )
    {
        IQueryable<Round> query = _dbSet;
        if (!tracked)
        {
            query = query.AsNoTracking();
        }
        return await query
            .Include(round => round.Match)
            .Include(round => round.Winner)
            .Include(round => round.RoundPictures)
            .ThenInclude(rp => rp.RoundVotes)
            .Include(round => round.RoundPictures)
            .ThenInclude(rp => rp.Picture)
            .Include(round => round.RoundPictures)
            .ThenInclude(rp => rp.User)
            .Where(filter)
            .FirstOrDefaultAsync();

    }

}
