using System.ComponentModel.DataAnnotations;
using Vtest94.Models;

namespace Vtest94.ViewModels
{
    public class CreateVideoViewModel
    {
        public Video Video { get; set; }

        [Required(ErrorMessage = "Please select a video file to upload.")]
        public IFormFile VideoFile { get; set; }
        public int ThumbnailFrameTime { get; set; } // Timestamp for thumbnail frame in seconds
    }
}
