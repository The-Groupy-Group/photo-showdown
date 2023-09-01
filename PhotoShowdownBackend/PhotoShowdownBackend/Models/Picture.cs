using Azure.Core.Pipeline;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoShowdownBackend.Models;

public class Picture
{

    [Key] // Primary Key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
    public int Id { get; set; }

    [Required] // Not nullable
    [ForeignKey("User")]
    public int UserId { get; set; }

    [Required]
    public string PicturePath { get; set; } = string.Empty;

    public User User { get; set; } = null!;

}
