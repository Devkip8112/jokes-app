using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyAspNetApp.Models;

namespace MyAspNetApp.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Joke> Jokes { get; set; }
    public DbSet<FavoriteJoke> FavoriteJokes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Joke>()
            .HasOne(j => j.CreatedBy)
            .WithMany()
            .HasForeignKey(j => j.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FavoriteJoke>()
            .HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<FavoriteJoke>()
            .HasOne(f => f.Joke)
            .WithMany()
            .HasForeignKey(f => f.JokeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Add unique constraint to prevent duplicate favorites
        builder.Entity<FavoriteJoke>()
            .HasIndex(f => new { f.UserId, f.JokeId })
            .IsUnique();
    }
} 