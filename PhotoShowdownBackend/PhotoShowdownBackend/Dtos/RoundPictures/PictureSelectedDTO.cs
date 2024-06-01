using PhotoShowdownBackend.Dtos.Users;

namespace PhotoShowdownBackend.Dtos.RoundPictures;

public class PictureSelectedDTO
{
    public int Id { get; set; } = 0;
    public string PicturePath { get; set; } = null!;
    public int NumOfVotes { get; set; }
    public int? SelectedByUserId { get; set; }
    public List<int> UsersVoted { get; set; } = new();
    public int PictureId { get; set; }
}