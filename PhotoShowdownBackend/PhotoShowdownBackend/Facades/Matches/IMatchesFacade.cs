using PhotoShowdownBackend.Dtos.Matches;

namespace PhotoShowdownBackend.Facades.Matches
{
    public interface IMatchesFacade
    {
        Task<MatchCreationResponseDTO> CreateNewMatch(int userId);
        Task<List<MatchDTO>> GetAllOpenMatches();
        Task<bool> MatchExists(int matchId);
    }
}
