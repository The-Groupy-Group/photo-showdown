using PhotoShowdownBackend.Dtos.Matches;

namespace PhotoShowdownBackend.Services.Matches;

/// <summary>
/// A business logic service interface for our Users
/// </summary>
public interface IMatchesService
{
    Task<MatchCreationResponseDTO> CreateNewMatch(int userId);
    Task<List<MatchDTO>> GetAllOpenMatches();

}
