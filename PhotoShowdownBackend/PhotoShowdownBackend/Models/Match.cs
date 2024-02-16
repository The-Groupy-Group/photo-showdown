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

    [Range(0, 60)]
    public int PictureSelectionTimeSeconds { get; set; } = 30;

    [Range(0, 60)]
    public int VoteTimeSeconds { get; set; } = 35;

    [Range(0, 100)]
    public int NumOfVotesToWin { get; set; } = 100;

    [Range(0, 500)]
    public int NumOfRounds { get; set; } = 500;

    [InverseProperty("Match")]
    public virtual ICollection<MatchConnection> MatchConnections { get; set; } = new List<MatchConnection>();

    public virtual User Owner { get; set; } = null!;

    [InverseProperty("Match")]
    public virtual ICollection<CustomSentence> CustomSentences { get; set; } = new List<CustomSentence>();

    public virtual ICollection<Round> Rounds { get; set; } = new List<Round>();
}
