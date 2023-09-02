using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoShowdownBackend.Models;

[Table("MatchConnections")]
public class MatchConnection
{

    [Key] // Primary Key
    [ForeignKey("User")]
    public int UserId { get; set; }

    public int MatchId { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("MatchId")]
    public virtual Match Match { get; set; } = null!;

}
