using Microsoft.EntityFrameworkCore;
using PhotoShowdownBackend.Models;

namespace PhotoShowdownBackend.Data;

/// <summary>
/// The DbContext for our application
/// </summary>
public class PhotoShowdownDbContext: DbContext
{
    public PhotoShowdownDbContext(DbContextOptions<PhotoShowdownDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Picture> Pictures { get; set; } = null!;

}
