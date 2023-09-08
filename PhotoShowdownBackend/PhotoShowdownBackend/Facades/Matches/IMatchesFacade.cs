using PhotoShowdownBackend.Dtos.Matches;

namespace PhotoShowdownBackend.Facades.Matches;

public interface IMatchesFacade
{
    Task<MatchCreationResponseDTO> CreateNewMatch(int userId);
    Task<List<MatchDTO>> GetAllOpenMatches();
    Task<MatchDTO> GetMatchById(int matchId);
    Task<bool> MatchExists(int matchId);
}
