using PhotoShowdownBackend.Dtos.Matches;

namespace PhotoShowdownBackend.Services.Matches;

/// <summary>
/// A business logic service interface for our Users
/// </summary>
public interface IMatchesService
{
    Task DeleteMatch(int matchId);
    Task<MatchCreationResponseDTO> CreateNewMatch(int ownerId);
    Task<List<MatchDTO>> GetAllOpenMatches();
    Task<bool> DoesMatchExists(int matchId);
    Task<MatchDTO> GetMatchById(int matchId);
    Task JoinMatch(int userId, int matchId);
    Task LeaveMatch(int userId, int matchId);
    Task<bool> IsUserConnectedToMatch(int userId);
    Task CreateMatchConnection(int userId, int matchId);
}
