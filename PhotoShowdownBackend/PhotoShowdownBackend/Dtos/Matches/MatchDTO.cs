using PhotoShowdownBackend.Consts;
using PhotoShowdownBackend.Dtos.Rounds;
using PhotoShowdownBackend.Dtos.Users;

namespace PhotoShowdownBackend.Dtos.Matches;

public class MatchDTO
{
    public int Id { get; set; }
    public UserPublicDetailsDTO Owner { get; set; } = null!;
    public List<UserPublicDetailsDTO> Users { get; set; } = new();
    public MatchStates MatchState { get; set; }
    public RoundDTO? Round { get; set; } 
}
