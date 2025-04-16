using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyAspNetApp.Data;
using MyAspNetApp.Models;
using System.Security.Claims;

namespace MyAspNetApp.Pages
{
    public class FavoritesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public FavoritesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<FavoriteJoke> FavoriteJokes { get; set; } = new List<FavoriteJoke>();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            FavoriteJokes = await _context.FavoriteJokes
                .Include(f => f.Joke)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostToggleFavoriteAsync([FromForm] int jokeId)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return new JsonResult(new { success = false, message = "User not authenticated" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return new JsonResult(new { success = false, message = "User ID not found" });
                }

                // Check if the joke exists
                var joke = await _context.Jokes.FindAsync(jokeId);
                if (joke == null)
                {
                    return new JsonResult(new { success = false, message = "Joke not found" });
                }

                var existingFavorite = await _context.FavoriteJokes
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.JokeId == jokeId);

                if (existingFavorite != null)
                {
                    _context.FavoriteJokes.Remove(existingFavorite);
                }
                else
                {
                    _context.FavoriteJokes.Add(new FavoriteJoke
                    {
                        UserId = userId,
                        JokeId = jokeId,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
} 