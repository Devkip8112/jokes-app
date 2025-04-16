using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyAspNetApp.Data;
using MyAspNetApp.Models;

namespace MyAspNetApp.Pages.Jokes;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public EditModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public Joke Joke { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var joke = await _context.Jokes.FirstOrDefaultAsync(m => m.Id == id);
        if (joke == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null || joke.CreatedByUserId != user.Id)
        {
            return Forbid();
        }

        Joke = joke;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var existingJoke = await _context.Jokes.FirstOrDefaultAsync(m => m.Id == Joke.Id);
        if (existingJoke == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null || existingJoke.CreatedByUserId != user.Id)
        {
            return Forbid();
        }

        existingJoke.Setup = Joke.Setup;
        existingJoke.Punchline = Joke.Punchline;
        existingJoke.Category = Joke.Category;
        existingJoke.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
} 