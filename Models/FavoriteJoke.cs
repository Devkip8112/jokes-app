using System.ComponentModel.DataAnnotations;

namespace MyAspNetApp.Models;

public class FavoriteJoke
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    public int JokeId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ApplicationUser User { get; set; } = null!;
    public Joke Joke { get; set; } = null!;
} 