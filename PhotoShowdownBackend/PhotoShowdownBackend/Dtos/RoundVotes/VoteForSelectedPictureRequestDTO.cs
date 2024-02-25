namespace PhotoShowdownBackend.Dtos.RoundVotes;

public class VoteForSelectedPictureRequestDTO
{
    public int RoundPictureId { get; set; }
    public int MatchId { get; set; }
    public int RoundIndex { get; set; }
}
