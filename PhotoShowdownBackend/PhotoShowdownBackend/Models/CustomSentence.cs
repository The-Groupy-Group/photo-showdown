using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoShowdownBackend.Models
{

    [Table("CustomSentences")]
    public class CustomSentence
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Match")]
        public int MatchId { get; set; }

        public string Sentence { get; set; } = null!;

        [ForeignKey("MatchId")]
        public virtual Match Match { get; set; } = null!;

    }
}
