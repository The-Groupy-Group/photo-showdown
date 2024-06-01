using PhotoShowdownBackend.Consts;
using PhotoShowdownBackend.Dtos.Rounds;
using PhotoShowdownBackend.Dtos.Users;

namespace PhotoShowdownBackend.Dtos.Matches;

public class MatchDTO
{
    public int Id { get; set; }
    public UserInMatchDTO Owner { get; set; } = null!;
    public List<UserInMatchDTO> Users { get; set; } = new();
    public MatchStates MatchState { get; set; }
    public RoundDTO? Round { get; set; } 
}
