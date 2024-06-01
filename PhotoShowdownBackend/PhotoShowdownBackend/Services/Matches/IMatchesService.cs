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
    Task<MatchCreationResponseDTO> CreateNewMatch(UserInMatchDTO ownerId);
    Task<List<MatchDTO>> GetAllMatches(MatchStates? state);
    Task<MatchDTO> GetMatchById(int matchId);
    Task AddUserToMatch(UserInMatchDTO user, int matchId);
    Task RemoveUserFromMatch(UserInMatchDTO user, int matchId);
    Task<MatchDTO?> GetMatchByUserId(int userId);
    Task StartMatch(int userId, StartMatchDTO startMatchDTO);
    Task EndMatch(int matchId);
    Task SelectPictureForRound(int pictureId, int matchId, int roundIndex, int userId);
    Task VoteForSelectedPicture(int roundPictureId, int matchId, int roundIndex, int userId);
}
