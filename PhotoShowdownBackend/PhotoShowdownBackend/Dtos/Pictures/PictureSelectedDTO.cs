using PhotoShowdownBackend.Dtos.Users;

namespace PhotoShowdownBackend.Dtos.Pictures;

public class PictureSelectedDTO
{
    public int NumOfVotes { get; set; }
    public UserPublicDetailsDTO? SelectedByUser { get; set; }
    public List<UserPublicDetailsDTO> UsersVoted { get; set; } = new ();
}