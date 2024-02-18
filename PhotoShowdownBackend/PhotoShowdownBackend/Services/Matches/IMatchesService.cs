using PhotoShowdownBackend.Consts;
using PhotoShowdownBackend.Dtos.Matches;
using PhotoShowdownBackend.Dtos.Users;
using PhotoShowdownBackend.Exceptions;

namespace PhotoShowdownBackend.Services.Matches;

/// <summary>
/// A business logic service interface for our Users
/// </summary>
public interface IMatchesService
{
    Task DeleteMatch(int matchId);
    Task<MatchCreationResponseDTO> CreateNewMatch(int ownerId);
    Task<List<MatchDTO>> GetAllMatches(MatchStates? state);
    Task<bool> DoesMatchExists(int matchId);
    Task<MatchDTO> GetMatchById(int matchId);
    Task AddUserToMatch(UserPublicDetailsDTO user, int matchId);
    Task RemoveUserFromMatch(UserPublicDetailsDTO user, int matchId);
    Task<bool> IsUserConnectedToMatch(int userId);
    Task ConnectUserToMatch(int userId, int matchId);
    Task<MatchDTO> GetMatchByUserId(int userId);
    Task StartMatch(int userId, StartMatchDTO startMatchDTO);
}
