namespace PhotoShowdownBackend.Services.MatchConnections;

public interface IMatchConnectionsService
{
    public Task CreateMatchConnection(int userId, int matchId);
    Task<bool> UserConnectedToMatch(int userId);
    Task<bool> IsMatchEmpty(int matchId);
    Task<bool> IsUserInThisMatch(int userId, int matchId);
    Task CloseConnection(int userId, int matchId);
}
