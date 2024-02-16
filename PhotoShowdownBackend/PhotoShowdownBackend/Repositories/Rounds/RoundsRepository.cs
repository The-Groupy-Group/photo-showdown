using PhotoShowdownBackend.Data;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;

namespace PhotoShowdownBackend.Repositories.Rounds;

public class RoundsRepository : Repository<Round>, IRoundsRepository
{
    public RoundsRepository(PhotoShowdownDbContext dbContext) : base(dbContext)
    {
    }
}
