using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vtest94.Data;

namespace Vtest94.ViewComponents
{
    public class HeaderCategoriesViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public HeaderCategoriesViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _context.Categories
            .Select(c => new
            {
                Id = c.Id,
                Name = c.Name,
                VideoCount = c.Videos.Count
            })
            .OrderBy(c => c.Name)
            .ToListAsync();

            return View(categories);
        }
    }
}
