using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PhotoShowdownBackend.Models;

public class Round
{
    [Key]
    [Column(Order = 0)]
    public int MatchId { get; set; }

    [Key]
    [Column(Order = 1)]
    public int RoundId { get; set; }

    public int? WinnerId { get; set; }

    [ForeignKey("WinnerId")]
    public User? Winner { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [ForeignKey("MatchId")]
    public Match Match { get; set; } = null!;
}
