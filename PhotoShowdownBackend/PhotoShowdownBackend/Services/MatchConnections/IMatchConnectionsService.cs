namespace PhotoShowdownBackend.Services.MatchConnections;

public interface IMatchConnectionsService
{
    public Task CreateMatchConnection(int userId, int matchId);
    Task<bool> IsUserConnectedToMatch(int userId);
    Task<bool> IsMatchEmpty(int matchId);
    Task<bool> IsUserInThisMatch(int userId, int matchId);
    Task DeleteMatchConnection(int userId, int matchId);
    Task<int> GetMatchIdByUserId(int userId);
}
