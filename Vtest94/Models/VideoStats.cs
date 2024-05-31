using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Vtest94.Models
{
    public class VideoStats
    {
        [Key]
        public int Id { get; set; }

        public int VideoId { get; set; } // Reference to the Video table

        public int ViewCount { get; set; } = 0;
        public int LikeCount { get; set; } = 0;
        public int DislikeCount { get; set; } = 0;
    }
}
