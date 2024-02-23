﻿using Microsoft.EntityFrameworkCore;
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

    virtual public async Task<Round?> GetLast(int matchId,
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
            .Where(round => round.MatchId == matchId)
            .LastOrDefaultAsync();

    }

}
