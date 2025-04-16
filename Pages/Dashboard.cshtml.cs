using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyAspNetApp.Data;
using MyAspNetApp.Models;

namespace MyAspNetApp.Pages;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public int TotalJokes { get; set; }
    public int FavoriteJokesCount { get; set; }
    public int CategoriesCount { get; set; }
    public List<Joke> UserJokes { get; set; } = new();
    public Dictionary<string, int> CategoryDistribution { get; set; } = new();
    public List<ActivityItem> RecentActivity { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToPage("/Account/Login");
        }

        // Get user's jokes
        UserJokes = await _context.Jokes
            .Where(j => j.CreatedByUserId == user.Id)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();

        // Calculate statistics
        TotalJokes = UserJokes.Count;
        
        // Get favorite jokes count
        FavoriteJokesCount = await _context.FavoriteJokes
            .CountAsync(fj => fj.UserId == user.Id);

        // Calculate category distribution
        CategoryDistribution = UserJokes
            .GroupBy(j => j.Category)
            .ToDictionary(g => g.Key, g => g.Count());

        CategoriesCount = CategoryDistribution.Count;

        // Get recent activity
        var recentJokes = UserJokes.Take(5)
            .Select(j => new ActivityItem
            {
                Description = "Created new joke",
                Details = j.Setup,
                Time = j.CreatedAt
            });

        var recentFavorites = await _context.FavoriteJokes
            .Include(fj => fj.Joke)
            .Where(fj => fj.UserId == user.Id)
            .OrderByDescending(fj => fj.Joke.CreatedAt)
            .Take(5)
            .Select(fj => new ActivityItem
            {
                Description = "Favorited joke",
                Details = fj.Joke.Setup,
                Time = fj.Joke.CreatedAt
            })
            .ToListAsync();

        RecentActivity = recentJokes.Concat(recentFavorites)
            .OrderByDescending(a => a.Time)
            .Take(10)
            .ToList();

        return Page();
    }
}

public class ActivityItem
{
    public string Description { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime Time { get; set; }
}
