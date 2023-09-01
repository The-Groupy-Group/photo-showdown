using Microsoft.AspNetCore.Mvc;
using PhotoShowdownBackend.Dtos.Matches;
using PhotoShowdownBackend.Dtos.Users;

namespace PhotoShowdownBackend.Services.Matches;

/// <summary>
/// A business logic service interface for our Users
/// </summary>
public interface IMatchesService
{
    Task<MatchCreationResponseDTO> CreateNewMatch(int userId);
    Task<AllMatchesResponseDTO> GetAllOpenMatches();


}
