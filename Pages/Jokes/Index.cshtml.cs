using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyAspNetApp.Data;
using MyAspNetApp.Models;

namespace MyAspNetApp.Pages.Jokes;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Joke> Jokes { get; set; } = new List<Joke>();

    public async Task OnGetAsync()
    {
        Jokes = await _context.Jokes
            .Include(j => j.CreatedBy)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }
} 