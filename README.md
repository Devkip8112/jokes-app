# Joke Management System

A web application built with ASP.NET Core that allows users to browse, create, and manage jokes. The system includes user authentication, favorites functionality, and admin features.

## Features

- **User Management**
  - User registration and authentication
  - Role-based authorization (Admin role)
  - Custom user profiles with first and last names

- **Joke Management**
  - Browse and search jokes
  - Categorized jokes (Programming, Puns, Classic, Food, Education)
  - Private/Public joke visibility
  - Create, edit, and delete jokes (Admin only)
  - View joke details including creator and creation date

- **Favorites System**
  - Mark jokes as favorites
  - View personal favorite jokes list
  - Remove jokes from favorites

## Technology Stack

- **Backend**
  - ASP.NET Core 8.0
  - Entity Framework Core
  - SQLite Database
  - ASP.NET Core Identity

- **Frontend**
  - Razor Pages
  - Bootstrap CSS
  - JavaScript

## Prerequisites

- .NET 8.0 SDK
- Any code editor (Visual Studio, VS Code, etc.)

## Getting Started

1. Clone the repository:
```bash
git clone [repository-url]
```

2. Navigate to the project directory:
```bash
cd MyAspNetApp
```

3. Apply database migrations:
```bash
dotnet ef database update
```

4. Run the application:
```bash
dotnet run
```

5. Access the application at `http://localhost:5159`

## Default Admin Account

The system comes with a pre-configured admin account:
- Email: admin@gmail.com
- Password: Admin@123

## Database Structure

- **Users** (ApplicationUser)
  - Standard Identity fields
  - FirstName
  - LastName
  - CreatedAt
  - LastModifiedAt

- **Jokes**
  - Setup
  - Punchline
  - Category
  - CreatedAt
  - LastModifiedAt
  - CreatedByUserId
  - IsPrivate

- **FavoriteJokes**
  - UserId
  - JokeId
  - (Unique constraint on UserId-JokeId pair)

## Project Structure

```
MyAspNetApp/
├── Areas/
│   └── Identity/           # Authentication views
├── Data/
│   └── ApplicationDbContext.cs
├── Models/
│   ├── ApplicationUser.cs
│   ├── Joke.cs
│   └── FavoriteJoke.cs
├── Pages/
│   ├── Admin/             # Admin features
│   ├── Jokes/            # Joke management
│   ├── Shared/           # Layout files
│   ├── Favorites.cshtml  # User favorites
│   └── Index.cshtml      # Home page
└── wwwroot/              # Static files
```

## Security Features

- Password requirements:
  - Minimum length: 6 characters
  - Requires digits
  - Requires lowercase and uppercase letters
  - Requires non-alphanumeric characters

- Role-based authorization
- Secure cookie handling
- Cross-Site Request Forgery (CSRF) protection

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

Devki Patel
