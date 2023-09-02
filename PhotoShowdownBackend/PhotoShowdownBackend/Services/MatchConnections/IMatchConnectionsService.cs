namespace PhotoShowdownBackend.Services.MatchConnections;

public interface IMatchConnectionsService
{
    public Task CreateMatchConnection(int userId, int matchId);
}
