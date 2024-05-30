using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Vtest94.Models
{
    public class VideoView
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VideoId { get; set; }

        [ForeignKey("VideoId")]
        public virtual Video Video { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        public DateTime ViewedAt { get; set; }
    }
}
