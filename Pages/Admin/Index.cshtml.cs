using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyAspNetApp.Data;
using MyAspNetApp.Models;

namespace MyAspNetApp.Pages.Admin;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public List<Joke> Jokes { get; set; } = new();
    public string SearchString { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync(string searchString, string category)
    {
        SearchString = searchString ?? string.Empty;
        Category = category ?? string.Empty;

        var query = _context.Jokes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(SearchString))
        {
            query = query.Where(j => 
                j.Setup.Contains(SearchString) || 
                j.Punchline.Contains(SearchString));
        }

        if (!string.IsNullOrWhiteSpace(Category))
        {
            query = query.Where(j => j.Category == Category);
        }

        Jokes = await query
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }
} 