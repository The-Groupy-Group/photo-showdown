namespace PhotoShowdownBackend.Models;

public class RoundPicture
{
    public int Id { get; set; }
    public int PictureId { get; set; }
    public int? UserId { get; set; }
    public int MatchId { get; set; }
    public int RoundIndex { get; set; }
    public User User { get; set; } = null!;
    public Round Round { get; set; } = null!;
    public Picture Picture { get; set; } = null!;
    public Match Match { get; set; } = null!;
}
