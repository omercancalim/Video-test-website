using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vtest94.Models
{
    public class Video
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public string? FileName { get; set; }  // Name of the video file

        public string? ThumbnailName { get; set; }  // Name of the thumbnail image file

        public TimeSpan Duration { get; set; }  // Duration of the video

        public DateTime UploadedDate { get; set; }  // Date the video was uploaded

        public string? UserId { get; set; }

        [ForeignKey("UserId")]  // This ensures EF Core understands the relationship
        public User? User { get; set; }

        public int? CategoryId { get; set; }  // Foreign key

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }  // Navigation property
    }
}
