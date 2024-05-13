using Microsoft.AspNetCore.Mvc.Rendering;
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
        public SelectList? Categories { get; set; }  // Add this
        public int? SelectedCategoryId { get; set; }  // To capture the selected category ID from the form
    }
}
