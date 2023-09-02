using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoShowdownBackend.Models;

[Table("Users")]
public class User
{
    [Key] // Primary Key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
    public int Id { get; set; }

    [Required] // Not nullable
    [MaxLength(50)] // Maximum length
    public string Username { get; set; } = string.Empty;

    [Required] // Not nullable
    [MaxLength(128)] // Maximum length (adjust as needed)
    public string PasswordHash { get; set; } = string.Empty;

    [Required] // Not nullable
    [EmailAddress]
    [MaxLength(100)] // Maximum length (adjust as needed)
    public string Email { get; set; } = string.Empty;

    [MaxLength(50)] // Maximum length (adjust as needed)
    public string? FirstName { get; set; }

    [MaxLength(50)] // Maximum length (adjust as needed)
    public string? LastName { get; set; }
    public bool IsActive { get; set; } = true;

    public bool IsAdmin { get; set; } = false;


    public int? ConnectionId { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Picture> Pictures { get; set; } = new List<Picture>();

    [ForeignKey("ConnectionId")]
    public virtual MatchConnection MatchConnection { get; set; } = null!;
}
