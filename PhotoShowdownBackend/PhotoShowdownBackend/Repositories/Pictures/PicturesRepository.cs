using PhotoShowdownBackend.Data;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;

namespace PhotoShowdownBackend.Repositories.Pictures;

public class PicturesRepository : Repository<Picture>, IPicturesRepository
{
    public PicturesRepository(PhotoShowdownDbContext db) : base(db)
    {
    }
}
