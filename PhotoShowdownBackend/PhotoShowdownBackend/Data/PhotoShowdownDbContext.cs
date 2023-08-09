using Microsoft.EntityFrameworkCore;
using PhotoShowdownBackend.Models;

namespace PhotoShowdownBackend.Data;

public class PhotoShowdownDbContext: DbContext
{
    public PhotoShowdownDbContext(DbContextOptions<PhotoShowdownDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;

}
