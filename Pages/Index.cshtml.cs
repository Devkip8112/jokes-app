using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyAspNetApp.Data;
using MyAspNetApp.Models;
using System.Security.Claims;

namespace MyAspNetApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ApplicationDbContext _context;

    public List<Joke> LatestJokes { get; set; } = new();
    public IList<Joke> Jokes { get; set; }
    public HashSet<int> FavoriteJokeIds { get; set; }

    public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Jokes = await _context.Jokes
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();

        if (User.Identity.IsAuthenticated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            FavoriteJokeIds = new HashSet<int>(
                await _context.FavoriteJokes
                    .Where(f => f.UserId == userId)
                    .Select(f => f.JokeId)
                    .ToListAsync()
            );
        }
        else
        {
            FavoriteJokeIds = new HashSet<int>();
        }

        return Page();
    }
}
