using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyAspNetApp.Data;
using MyAspNetApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyAspNetApp.Pages.Admin;

[Authorize(Roles = "Admin")]
public class AddSampleJokesModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AddSampleJokesModel> _logger;

    public AddSampleJokesModel(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<AddSampleJokesModel> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public class SampleJoke
    {
        public int Id { get; set; }
        public string Setup { get; set; } = string.Empty;
        public string Punchline { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }

    [BindProperty]
    public List<int> SelectedJokes { get; set; } = new();

    [BindProperty]
    public string? SelectedUserId { get; set; }

    [BindProperty]
    public bool MakePrivate { get; set; }

    public List<SampleJoke> SampleJokes { get; set; } = new();
    public List<SelectListItem> Users { get; set; } = new();
    public string? StatusMessage { get; set; }
    public bool IsError { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        // Load sample jokes
        SampleJokes.AddRange(new List<SampleJoke>
        {
            new SampleJoke
            {
                Id = 1,
                Setup = "Why don't scientists trust atoms?",
                Punchline = "Because they make up everything!",
                Category = "Science"
            },
            new SampleJoke
            {
                Id = 2,
                Setup = "Why did the scarecrow win an award?",
                Punchline = "Because he was outstanding in his field!",
                Category = "Puns"
            },
            new SampleJoke
            {
                Id = 3,
                Setup = "Why don't programmers like the outdoors?",
                Punchline = "Too many bugs!",
                Category = "Programming"
            },
            new SampleJoke
            {
                Id = 4,
                Setup = "Why did the tomato turn red?",
                Punchline = "Because it saw the salad dressing!",
                Category = "Food"
            },
            new SampleJoke
            {
                Id = 5,
                Setup = "Why did the book go to the doctor?",
                Punchline = "It had a spine problem!",
                Category = "Classic"
            },
            new SampleJoke
            {
                Id = 6,
                Setup = "Why do programmers prefer dark mode?",
                Punchline = "Because light attracts bugs!",
                Category = "Programming"
            },
            new SampleJoke
            {
                Id = 7,
                Setup = "Why do seagulls fly over the sea?",
                Punchline = "Because if they flew over the bay, they'd be bagels!",
                Category = "Puns"
            },
            new SampleJoke
            {
                Id = 8,
                Setup = "What do you call a bear with no teeth?",
                Punchline = "A gummy bear!",
                Category = "Classic"
            },
            new SampleJoke
            {
                Id = 9,
                Setup = "Why did the computer go to the doctor?",
                Punchline = "It had a virus!",
                Category = "Technology"
            },
            new SampleJoke
            {
                Id = 10,
                Setup = "Why did the cookie go to the doctor?",
                Punchline = "Because it was feeling crumbly!",
                Category = "Food"
            }
        });

        // Load users
        var users = await _userManager.Users.ToListAsync();
        Users = users.Select(u => new SelectListItem
        {
            Value = u.Id,
            Text = $"{u.FirstName} {u.LastName} ({u.Email})"
        }).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (string.IsNullOrEmpty(SelectedUserId))
        {
            StatusMessage = "Please select a user.";
            IsError = true;
            return Page();
        }

        var selectedUser = await _userManager.FindByIdAsync(SelectedUserId);
        if (selectedUser == null)
        {
            StatusMessage = "Selected user not found.";
            IsError = true;
            return Page();
        }

        // Check if any jokes were selected
        if (!SelectedJokes.Any())
        {
            StatusMessage = "Please select at least one joke to add.";
            IsError = true;
            return Page();
        }

        try
        {
            // First, ensure we have the user with all relationships loaded
            selectedUser = await _userManager.Users
                .Include(u => u.Jokes)
                .FirstOrDefaultAsync(u => u.Id == SelectedUserId);

            if (selectedUser == null)
            {
                StatusMessage = "Selected user not found.";
                IsError = true;
                return Page();
            }

            foreach (var jokeId in SelectedJokes)
            {
                var sampleJoke = SampleJokes.FirstOrDefault(j => j.Id == jokeId);
                if (sampleJoke == null)
                {
                    continue; // Skip invalid joke IDs
                }

                var joke = new Joke
                {
                    Setup = sampleJoke.Setup,
                    Punchline = sampleJoke.Punchline,
                    Category = sampleJoke.Category,
                    IsPrivate = MakePrivate,
                    CreatedAt = DateTime.UtcNow,
                    LastModifiedAt = DateTime.UtcNow,
                    CreatedByUserId = selectedUser.Id,
                    CreatedBy = selectedUser // Set the navigation property
                };

                _context.Jokes.Add(joke);
            }

            // Log the number of jokes being added
            _logger.LogInformation("Adding {JokeCount} jokes for user {UserId}", SelectedJokes.Count, SelectedUserId);

            await _context.SaveChangesAsync();
            StatusMessage = selectedUser != null 
                ? $"Successfully added {SelectedJokes.Count} jokes for {selectedUser.FirstName} {selectedUser.LastName}"
                : $"Successfully added {SelectedJokes.Count} jokes for user {SelectedUserId}";
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while adding jokes");
            StatusMessage = "Database error occurred while adding jokes. Please try again.";
            IsError = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while adding jokes");
            StatusMessage = "An unexpected error occurred while adding jokes. Please try again.";
            IsError = true;
        }

        StatusMessage = selectedUser != null 
            ? $"Successfully added {SelectedJokes.Count} jokes for {selectedUser.FirstName} {selectedUser.LastName}"
            : $"Successfully added {SelectedJokes.Count} jokes for user {SelectedUserId}";
        return Page();
    }
}
