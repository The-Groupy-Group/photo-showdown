using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoShowdownBackend.Models;

[Table("RoundPictures")]
public class RoundPicture
{
    [Key] // Primary Key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
    public int Id { get; set; }

    [ForeignKey("Picture")]
    public int PictureId { get; set; }

    [ForeignKey("User")]
    public int? UserId { get; set; }

    public int MatchId { get; set; }

    public int RoundIndex { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [ForeignKey("MatchId, RoundIndex")]
    public Round Round { get; set; } = null!;

    [ForeignKey("PictureId")]
    public Picture Picture { get; set; } = null!;

    [ForeignKey("MatchId")]
    public Match Match { get; set; } = null!;

    public ICollection<RoundVote> RoundVotes { get; set; } = new List<RoundVote>();
}
