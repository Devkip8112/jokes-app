using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAspNetApp.Models;

public class Joke
{
    public int Id { get; set; }

    [Required]
    [StringLength(500)]
    public string Setup { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Punchline { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public required string CreatedByUserId { get; set; }

    public DateTime? LastModifiedAt { get; set; }

    public bool IsPrivate { get; set; } = false;

    [ForeignKey("CreatedByUserId")]
    public ApplicationUser CreatedBy { get; set; } = null!;
} 