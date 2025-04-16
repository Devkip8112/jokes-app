using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyAspNetApp.Data;
using MyAspNetApp.Models;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// Seed default jokes
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Create Admin role if it doesn't exist
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Create default admin user
    var adminEmail = "admin@gmail.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FirstName = "Admin",
            LastName = "User",
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(adminUser, "Admin@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    // Add default jokes if they don't exist
    if (!context.Jokes.Any())
    {
        var defaultJokes = new List<Joke>
        {
            new Joke
            {
                Setup = "Why don't programmers like nature?",
                Punchline = "It has too many bugs!",
                Category = "Programming",
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = adminUser.Id
            },
            new Joke
            {
                Setup = "What do you call a bear with no teeth?",
                Punchline = "A gummy bear!",
                Category = "Puns",
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = adminUser.Id
            },
            new Joke
            {
                Setup = "Why did the scarecrow win an award?",
                Punchline = "Because he was outstanding in his field!",
                Category = "Classic",
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = adminUser.Id
            },
            new Joke
            {
                Setup = "What do you call a fake noodle?",
                Punchline = "An impasta!",
                Category = "Food",
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = adminUser.Id
            },
            new Joke
            {
                Setup = "Why did the math book look sad?",
                Punchline = "Because it had too many problems!",
                Category = "Education",
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = adminUser.Id
            },
            new Joke
            {
                Setup = "What do you call a bear that's stuck in the rain?",
                Punchline = "A drizzly bear!",
                Category = "Puns",
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = adminUser.Id
            },
            new Joke
            {
                Setup = "Why don't eggs tell jokes?",
                Punchline = "They'd crack up!",
                Category = "Food",
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = adminUser.Id
            },
            new Joke
            {
                Setup = "What do you call a programmer from Finland?",
                Punchline = "Nerdic!",
                Category = "Programming",
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = adminUser.Id
            },
            new Joke
            {
                Setup = "Why did the coffee file a police report?",
                Punchline = "It got mugged!",
                Category = "Food",
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = adminUser.Id
            },
            new Joke
            {
                Setup = "What do you call a bear with no teeth and no ears?",
                Punchline = "B!",
                Category = "Puns",
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = adminUser.Id
            }
        };

        context.Jokes.AddRange(defaultJokes);
        await context.SaveChangesAsync();
    }
}

app.Run();
