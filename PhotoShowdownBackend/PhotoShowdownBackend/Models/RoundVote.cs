namespace PhotoShowdownBackend.Models;

public class RoundVote
{
    public int Id { get; set; }
    public int RoundPictureId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public RoundPicture RoundPicture { get; set; } = null!;
}