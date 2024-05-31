using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vtest94.Data;
using Vtest94.Models;
using System.Linq;
using System.Threading.Tasks;

public class TopUsersViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public TopUsersViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var topUsers = await _context.Users
            .Select(u => new
            {
                User = u,
                VideoCount = u.Videos.Count,
                TotalViews = u.Videos.Join(_context.VideoStats,
                                    v => v.Id,
                                    vs => vs.VideoId,
                                    (v, vs) => vs.ViewCount)
                              .Sum()
            })
            .OrderByDescending(u => u.VideoCount)
            .Take(7)
            .ToListAsync();
        return View(topUsers);
    }
}
