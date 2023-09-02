using PhotoShowdownBackend.Data;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;

namespace PhotoShowdownBackend.Repositories.MatchConnections
{
    public class MatchConnectionsRepository : Repository<MatchConnection>, IMatchConnectionsRepository
    {
        public MatchConnectionsRepository(PhotoShowdownDbContext _db) : base(_db)
        {
        }
    }
}
