using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Azure.Core.Pipeline;

namespace PhotoShowdownBackend.Models;

public class Round
{
    [Key]
    [Column(Order = 0)]
    public int MatchId { get; set; }

    [Key]
    [Column(Order = 1)]
    public int RoundIndex { get; set; }

    public int? WinnerId { get; set; }
    
    public string Sentence { get; set; } = string.Empty;

    [ForeignKey("WinnerId")]
    public User? Winner { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [ForeignKey("MatchId")]
    public Match Match { get; set; } = null!;
}
