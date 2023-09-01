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
  
    public virtual ICollection<User> Users { get; set; } = new List<User>();

}
