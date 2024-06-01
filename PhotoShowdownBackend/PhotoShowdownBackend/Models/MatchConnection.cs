using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoShowdownBackend.Models;

[Table("MatchConnections")]
public class MatchConnection
{

    [Key] // Primary Key
    [ForeignKey("User")]
    public int UserId { get; set; }

    [ForeignKey("Match")]
    public int MatchId { get; set; }

    public bool IsLockedIn { get; set; } = false;

    public double Score { get; set; } = 0d;

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("MatchId")]
    public virtual Match Match { get; set; } = null!;
}
