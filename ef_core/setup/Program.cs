using EFCore;
using EFCore.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Database>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
});

var app = builder.Build();
ApplyMigrations(app);

app.MapGet("/", (Database database) =>
{
    var posts = database.Posts.ToList();
    if (posts.Count != 0)
    {
        database.Posts.RemoveRange(posts);
    }

    var newPost = new Post
    {
        Title = "New Post",
        Content = "This is a new post!",
        Author = new Author()
        {
            Name = "John Doe",
            ImageUrl = "https://github.com/JohnDoe"
        }
    };

    database.Posts.Add(newPost);
    
    database.SaveChanges();

    return database.Posts.ToList();
});

app.Run();

static void ApplyMigrations(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<Database>();
    db.Database.Migrate();
}
