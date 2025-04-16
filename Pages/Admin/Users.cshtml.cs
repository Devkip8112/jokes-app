using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyAspNetApp.Data;
using MyAspNetApp.Models;

namespace MyAspNetApp.Pages.Admin;

[Authorize(Roles = "Admin")]
public class UsersModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UsersModel> _logger;

    public UsersModel(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        ILogger<UsersModel> logger)
    {
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }

    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int JokesCount { get; set; }
        public bool IsAdmin { get; set; }
    }

    public List<UserViewModel> Users { get; set; } = new();
    public string? StatusMessage { get; set; }
    public bool IsError { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadUsers();
        return Page();
    }

    private async Task LoadUsers()
    {
        var users = await _userManager.Users
            .Include(u => u.Jokes)
            .OrderBy(u => u.Email)
            .ToListAsync();

        Users = new List<UserViewModel>();

        foreach (var user in users)
        {
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            Users.Add(new UserViewModel
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedAt = user.CreatedAt,
                JokesCount = user.Jokes?.Count ?? 0,
                IsAdmin = isAdmin
            });
        }
    }

    public async Task<IActionResult> OnPostMakeAdminAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            StatusMessage = "User not found.";
            IsError = true;
            await LoadUsers();
            return Page();
        }

        var result = await _userManager.AddToRoleAsync(user, "Admin");
        if (result.Succeeded)
        {
            _logger.LogInformation($"User {user.Email} was made admin by {User.Identity?.Name}");
            StatusMessage = $"{user.Email} is now an admin.";
        }
        else
        {
            StatusMessage = "Failed to make user an admin.";
            IsError = true;
        }

        await LoadUsers();
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveAdminAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            StatusMessage = "User not found.";
            IsError = true;
            await LoadUsers();
            return Page();
        }

        // Prevent removing admin role from the main admin account
        if (user.Email == "admin@gmail.com")
        {
            StatusMessage = "Cannot remove admin role from the main admin account.";
            IsError = true;
            await LoadUsers();
            return Page();
        }

        var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
        if (result.Succeeded)
        {
            _logger.LogInformation($"Admin role removed from {user.Email} by {User.Identity?.Name}");
            StatusMessage = $"Admin role removed from {user.Email}.";
        }
        else
        {
            StatusMessage = "Failed to remove admin role.";
            IsError = true;
        }

        await LoadUsers();
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            StatusMessage = "User not found.";
            IsError = true;
            await LoadUsers();
            return Page();
        }

        // Prevent deleting the main admin account
        if (user.Email == "admin@gmail.com")
        {
            StatusMessage = "Cannot delete the main admin account.";
            IsError = true;
            await LoadUsers();
            return Page();
        }

        // Delete user's jokes first
        var jokes = await _context.Jokes
            .Where(j => j.CreatedByUserId == userId)
            .ToListAsync();
        _context.Jokes.RemoveRange(jokes);
        await _context.SaveChangesAsync();

        // Delete user's favorites
        var favorites = await _context.FavoriteJokes
            .Where(f => f.UserId == userId)
            .ToListAsync();
        _context.FavoriteJokes.RemoveRange(favorites);
        await _context.SaveChangesAsync();

        // Delete the user
        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            _logger.LogInformation($"User {user.Email} was deleted by {User.Identity?.Name}");
            StatusMessage = $"User {user.Email} has been deleted.";
        }
        else
        {
            StatusMessage = "Failed to delete user.";
            IsError = true;
        }

        await LoadUsers();
        return Page();
    }
}
