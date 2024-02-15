using PhotoShowdownBackend.Dtos.Users;

namespace PhotoShowdownBackend.Dtos.Matches;

public class MatchDTO
{
    public int Id { get; set; }
    public UserPublicDetailsDTO Owner { get; set; } = null!;
    public List<UserPublicDetailsDTO> Users { get; set; } = new();
    public bool HasStarted { get; set; } = false;
}
