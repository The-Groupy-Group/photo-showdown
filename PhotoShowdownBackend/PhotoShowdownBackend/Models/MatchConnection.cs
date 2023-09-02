using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoShowdownBackend.Models;

[Table("MatchConnections")]
public class MatchConnection
{

    [Key] // Primary Key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
    public int Id { get; set; }


    
    public int UserId { get; set; }


    
    public int MatchId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    [ForeignKey("MatchId")]
    public Match Match { get; set; } 

}
