using PhotoShowdownBackend.Dtos.RoundPictures;
using PhotoShowdownBackend.Dtos.Rounds;

namespace PhotoShowdownBackend.Services.Rounds;

public interface IRoundsService
{
    Task<RoundDTO> StartRound(int matchId, int roundIndex);
    Task<RoundDTO> EndRound(int matchId, int roundIndex);
    Task<RoundDTO> GetCurrentRound(int matchId);
    Task SelectPicture(int pictureId, int matchId, int roundIndex, int userId);
    Task<PictureSelectedDTO> VoteForSelectedPicture(int roundPictureId, int matchId, int roundIndex, int userId);
    Task<RoundDTO> StartVotePhase(int matchId, int roundIndex);
}
