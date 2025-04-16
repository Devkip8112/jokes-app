using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyAspNetApp.Data;
using MyAspNetApp.Models;

namespace MyAspNetApp.Pages.Jokes;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
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

        var joke = await _context.Jokes
            .Include(j => j.CreatedBy)
            .FirstOrDefaultAsync(j => j.Id == id);

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

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var joke = await _context.Jokes.FindAsync(id);
        if (joke == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null || joke.CreatedByUserId != user.Id)
        {
            return Forbid();
        }

        _context.Jokes.Remove(joke);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
} 