using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAspNetApp.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; }

    public DateTime? LastModifiedAt { get; set; }

    public virtual ICollection<Joke> Jokes { get; set; } = new List<Joke>();

    public virtual ICollection<FavoriteJoke> FavoriteJokes { get; set; } = new List<FavoriteJoke>();
} 