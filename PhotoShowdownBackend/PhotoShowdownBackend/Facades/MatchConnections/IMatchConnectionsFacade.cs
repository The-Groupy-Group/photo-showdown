namespace PhotoShowdownBackend.Facades.MatchConnections
{
    public interface IMatchConnectionsFacade
    {

        Task CreateMatchConnection(int userId, int matchId);
        Task<bool> UserConnectedToMatch(int userId);
    }
}
