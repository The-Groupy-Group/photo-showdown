using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.Repository;

namespace PhotoShowdownBackend.Repositories.MatchConnections;

public interface IMatchConnectionsRepository : IRepository<MatchConnection>
{
    Task<bool> IsMatchEmpty(int matchId);
    Task<bool> IsUserInThisMatch(int userId, int matchId);
    Task<bool> UserConnectedToMatch(int userId);
}
