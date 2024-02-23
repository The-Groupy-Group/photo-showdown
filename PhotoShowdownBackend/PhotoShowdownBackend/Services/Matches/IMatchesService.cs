using PhotoShowdownBackend.Consts;
using PhotoShowdownBackend.Dtos.Matches;
using PhotoShowdownBackend.Dtos.Rounds;
using PhotoShowdownBackend.Dtos.Users;
using PhotoShowdownBackend.Exceptions;

namespace PhotoShowdownBackend.Services.Matches;

/// <summary>
/// A business logic service interface for our Users
/// </summary>
public interface IMatchesService
{
    Task<MatchCreationResponseDTO> CreateNewMatch(int ownerId);
    Task<List<MatchDTO>> GetAllMatches(MatchStates? state);
    Task<MatchDTO> GetMatchById(int matchId);
    Task AddUserToMatch(UserPublicDetailsDTO user, int matchId);
    Task RemoveUserFromMatch(UserPublicDetailsDTO user, int matchId);
    Task<MatchDTO?> GetMatchByUserId(int userId);
    Task StartMatch(int userId, StartMatchDTO startMatchDTO);
    Task<RoundDTO> GetCurrentRound(int matchId);
    Task SelectPicture(int pictureId, int matchId, int roundIndex,int userId)
}
