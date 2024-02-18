using Microsoft.EntityFrameworkCore;
using PhotoShowdownBackend.Data;

namespace PhotoShowdownBackend.Utils;

/// <summary>
/// A class that initializes the database by ending all matches that are in progress (In case the server was restarted)
/// </summary>
public static class DatabaseInitializer
{
    public static void Initialize(PhotoShowdownDbContext dbContext)
    {
        // Set all Match rows to "ended"
        foreach (var match in dbContext.Matches.Where(m => m.StartDate < DateTime.UtcNow).Include(m=> m.MatchConnections))
        {
            match.EndDate = DateTime.UtcNow;
            dbContext.MatchConnections.RemoveRange(match.MatchConnections);
        }

        dbContext.SaveChanges();
    }
}
