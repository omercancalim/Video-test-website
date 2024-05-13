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
            .Include(u => u.Videos)
            .OrderByDescending(u => u.Videos.Count) // Assuming you want to sort by number of videos
            .Take(7)
            .ToListAsync();
        return View(topUsers);
    }
}
