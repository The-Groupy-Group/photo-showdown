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
    [MaxLength(128)]
    public string PicturePath { get; set; } = string.Empty;
    
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [NotMapped]
    public IFormFile PictureFile { get; set; } = null!;
}
