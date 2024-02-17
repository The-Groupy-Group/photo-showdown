using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Azure.Core.Pipeline;

namespace PhotoShowdownBackend.Models;

[PrimaryKey(nameof(MatchId), nameof(RoundIndex))]
public class Round
{
    [Key, Column(Order = 0)]
    [ForeignKey("Match")]
    public int MatchId { get; set; }

    [Key, Column(Order = 1)]
    public int RoundIndex { get; set; }

    public RoundStates RoundState { get; set; } = 0;

    [ForeignKey("Winner")]
    public int? WinnerId { get; set; }
    
    public string Sentence { get; set; } = string.Empty;

    [ForeignKey("WinnerId")]
    public User? Winner { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [ForeignKey("MatchId")]
    public Match Match { get; set; } = null!;

    [ForeignKey("WinnerId")]
    public User? Winner { get; set; }

    public enum RoundStates
    {
        NotStarted = 0,
        PictureSelection = 1,
        Voting = 2,
        Ended = 3
    }
}
