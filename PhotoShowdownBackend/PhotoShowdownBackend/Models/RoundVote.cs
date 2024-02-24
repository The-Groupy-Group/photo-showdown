using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PhotoShowdownBackend.Models;

public class RoundVote
{
    [Key] // Primary Key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
    public int Id { get; set; }

    [ForeignKey("RoundPicture")]
    public int RoundPictureId { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }

    [ForeignKey("RoundPictureId")]
    public RoundPicture RoundPicture { get; set; } = null!;

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}