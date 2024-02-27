using Microsoft.EntityFrameworkCore;
using PhotoShowdownBackend.Data;

namespace PhotoShowdownBackend.Utils;

/// <summary>
/// A class that initializes the database by ending all matches that are in progress (In case the server was restarted)
/// </summary>
public static class DatabaseInitializer
{
    public static async Task Initialize(PhotoShowdownDbContext dbContext)
    {
        // Set all Match rows to "ended"
        foreach (var match in await dbContext.Matches.Where(m => 
                !m.EndDate.HasValue || 
                DateTime.UtcNow < m.EndDate ||
                m.MatchConnections.Count == 0)
            .Include(m=> m.MatchConnections).ToListAsync())
        {
            match.EndDate = DateTime.UtcNow;
            dbContext.MatchConnections.RemoveRange(match.MatchConnections);
        }

        await dbContext.SaveChangesAsync();
    }
}
