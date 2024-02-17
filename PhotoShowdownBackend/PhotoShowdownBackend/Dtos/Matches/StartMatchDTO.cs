using System.ComponentModel.DataAnnotations;

namespace PhotoShowdownBackend.Dtos.Matches;

public class StartMatchDTO
{
    public int MatchId { get; set; }
    public string[] Sentences { get; set; } = new string[0];
    public int? PictureSelectionTimeSeconds { get; set; }
    public int? VoteTimeSeconds { get; set; }
    [Range(1, 100)]
    public int? NumOfVotesToWin { get; set; }
    [Range(1, 500)]
    public int? NumOfRounds { get; set; }
}
