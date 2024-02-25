using PhotoShowdownBackend.Data;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;

namespace PhotoShowdownBackend.Repositories.RoundVotes;

public class RoundVotesRepository: Repository<RoundVote>, IRoundVotesRepository
{
    public RoundVotesRepository(PhotoShowdownDbContext dbContext) : base(dbContext)
    {
    }
}
