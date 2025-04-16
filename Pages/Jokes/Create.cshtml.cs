using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyAspNetApp.Data;
using MyAspNetApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace MyAspNetApp.Pages.Jokes;

[Authorize] 
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<CreateModel> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    [BindProperty]
    public Joke Joke { get; set; } = default!;

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    _logger.LogError($"Validation error: {error.ErrorMessage}");
                }
            }
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            _logger.LogWarning("User not found when trying to create joke");
            return RedirectToPage("/Account/Login");
        }

        Joke.CreatedByUserId = user.Id;
        Joke.CreatedAt = DateTime.UtcNow;

        try
        {
            _context.Jokes.Add(Joke);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Joke created successfully by user {user.Email}");
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating joke: {ex.Message}");
            ModelState.AddModelError(string.Empty, "An error occurred while saving the joke.");
            return Page();
        }
    }
} 