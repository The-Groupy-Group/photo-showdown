using PhotoShowdownBackend.Dtos.Rounds;

namespace PhotoShowdownBackend.Services.Rounds;

public interface IRoundsService
{
    Task<RoundDTO> StartRound(int matchId, int roundIndex);
    Task<RoundDTO> EndRound(int matchId, int roundIndex);
}
