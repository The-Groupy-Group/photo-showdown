using PhotoShowdownBackend.Dtos.Matches;

namespace PhotoShowdownBackend.Services.Matches;

/// <summary>
/// A business logic service interface for our Users
/// </summary>
public interface IMatchesService
{
    Task CloseMatch(int matchId);
    Task<MatchCreationResponseDTO> CreateNewMatch(int ownerId);
    Task<List<MatchDTO>> GetAllOpenMatches();
    Task<bool> DoesMatchExists(int matchId);
    Task<MatchDTO> GetMatchById(int matchId);
}
