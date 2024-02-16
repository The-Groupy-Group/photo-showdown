using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoShowdownBackend.Models;

[Table("Matches")]
public class Match
{
    [Key] // Primary Key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
    public int Id { get; set; }

    [Required] // Not nullable
    public int OwnerId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [InverseProperty("Match")]
    public virtual ICollection<MatchConnection> MatchConnections { get; set; } = new List<MatchConnection>();

    public virtual User Owner { get; set; } = null!;

    public virtual ICollection<Round> Rounds { get; set; } = new List<Round>();
}
