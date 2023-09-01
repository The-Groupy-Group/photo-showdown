using Microsoft.EntityFrameworkCore;
using PhotoShowdownBackend.Data;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;

namespace PhotoShowdownBackend.Repositories.Users;

/// <summary>
/// Data Access Layer (DAL) implementation for our Matches
/// </summary>
public class MatchesReporitory: Repository<Match>, IMatchesReporitory
{
    public MatchesReporitory(PhotoShowdownDbContext _db) : base(_db)
    {
    }

}

