using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAspNetApp.Data;
using MyAspNetApp.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace MyAspNetApp.Pages;

[Authorize]
public class AddJokeModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AddJokeModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 10)]
        public string Setup { get; set; } = string.Empty;

        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 10)]
        public string Punchline { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        public bool IsPrivate { get; set; }
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Debug: Log the model state
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            // Add error messages to the model state
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        // Debug: Log user info
        System.Diagnostics.Debug.WriteLine($"User ID: {user.Id}");
        System.Diagnostics.Debug.WriteLine($"User Name: {user.UserName}");

        // Check if user is admin
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        if (!isAdmin)
        {
            // Count the number of jokes for this user
            var jokeCount = await _context.Jokes.CountAsync(j => j.CreatedByUserId == user.Id);
            if (jokeCount >= 5)
            {
                ModelState.AddModelError(string.Empty, "You have reached the maximum number of jokes you can add.");
                return Page();
            }
        }

        var joke = new Joke
        {
            Setup = Input.Setup,
            Punchline = Input.Punchline,
            Category = Input.Category,
            IsPrivate = Input.IsPrivate,
            CreatedByUserId = user.Id
        };

        try
        {
            // Check if context is null
            if (_context == null)
            {
                throw new InvalidOperationException("Database context is null");
            }

            // Add the joke
            _context.Jokes.Add(joke);
            
            // Save changes
            var result = await _context.SaveChangesAsync();
            System.Diagnostics.Debug.WriteLine($"SaveChanges result: {result} records affected");
            
            return RedirectToPage("/Index", new { message = "Joke added successfully!" });
        }
        catch (DbUpdateException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Database update error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException?.Message}");
            ModelState.AddModelError(string.Empty, $"Database error: {ex.Message}");
            return Page();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"General error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
            return Page();
        }
    }
}
