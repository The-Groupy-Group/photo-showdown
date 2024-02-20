using Microsoft.EntityFrameworkCore;
using PhotoShowdownBackend.Models;

namespace PhotoShowdownBackend.Data;

/// <summary>
/// The DbContext for our application
/// </summary>
public class PhotoShowdownDbContext : DbContext
{
    public PhotoShowdownDbContext(DbContextOptions<PhotoShowdownDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Picture> Pictures { get; set; } = null!;
    public DbSet<Match> Matches { get; set; } = null!;
    public DbSet<MatchConnection> MatchConnections { get; set; } = null!;
    public DbSet<Round> Rounds { get; set; } = null!;
    public DbSet<CustomSentence> CustomSentences { get; set; } = null!;
    public DbSet<RoundPicture> RoundPictures { get; set; } = null!;
    public DbSet<RoundVote> RoundVotes { get; set; } = null!;
}
