using PhotoShowdownBackend.Data;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;

namespace PhotoShowdownBackend.Repositories.RoundPictures;

public class RoundPicturesRepository: Repository<RoundPicture>, IRoundPicturesRepository
{
    public RoundPicturesRepository(PhotoShowdownDbContext dbContext) : base(dbContext)
    {
    }
}
