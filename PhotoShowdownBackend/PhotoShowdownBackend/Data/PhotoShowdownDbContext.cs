using Microsoft.EntityFrameworkCore;
using PhotoShowdownBackend.Models;
using System.Text.Json;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // WinnerIds JSON mapping
        modelBuilder.Entity<Round>(entity =>
        {
            entity.Property(e => e.WinnerIds)
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                    v => JsonSerializer.Deserialize<int[]>(v, (JsonSerializerOptions)null!));
        });
    }
}
