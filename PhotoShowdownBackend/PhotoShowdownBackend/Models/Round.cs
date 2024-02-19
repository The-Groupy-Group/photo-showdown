using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PhotoShowdownBackend.Models;

[PrimaryKey(nameof(MatchId), nameof(RoundIndex))]
public class Round
{
    [Key, Column(Order = 0)]
    [ForeignKey("Match")]
    public int MatchId { get; set; }

    [Key, Column(Order = 1)]
    public int RoundIndex { get; set; } = 0;

    public RoundStates RoundState { get; set; } = 0;

    [ForeignKey("Winner")]
    public int? WinnerId { get; set; }
    
    public string Sentence { get; set; } = string.Empty;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [ForeignKey("MatchId")]
    public Match Match { get; set; } = null!;

    [ForeignKey("WinnerId")]
    public User? Winner { get; set; }

    public enum RoundStates
    {
        PictureSelection,
        Voting,
        Ended
    }
}
