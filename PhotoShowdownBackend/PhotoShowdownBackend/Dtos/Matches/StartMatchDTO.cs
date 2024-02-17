namespace PhotoShowdownBackend.Dtos.Matches;

public class StartMatchDTO
{
    public int MatchId { get; set; }
    public string[] Sentences { get; set; } = new string[0];
    public int? PictureSelectionTimeSeconds { get; set; }
    public int? VoteTimeSeconds { get; set; }
    public int? NumOfVotesToWin { get; set; }
    public int? NumOfRounds { get; set; }
}
