using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vtest94.Data;

namespace Vtest94.ViewComponents
{
    public class LatestVideosViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public LatestVideosViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var videos = await _context.Videos
            .OrderByDescending(v => v.UploadedDate)
            .Take(5) // Latest 5 videos
            .ToListAsync();

            return View(videos);
        }
    }
}
