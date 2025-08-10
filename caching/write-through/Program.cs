using StackExchange.Redis;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add Redis connection
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect("localhost:6379"));

var app = builder.Build();

// Simple in-memory "database"
var database = new Dictionary<string, string>();

app.MapGet("/get/{key}", async (string key, IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    var cachedValue = await db.StringGetAsync(key);

    if (cachedValue.HasValue)
    {
        return Results.Ok(new { source = "cache", value = cachedValue.ToString() });
    }
    else if (database.TryGetValue(key, out var dbValue))
    {
        // Optionally, repopulate cache on read
        await db.StringSetAsync(key, dbValue);
        return Results.Ok(new { source = "database", value = dbValue });
    }
    else
    {
        return Results.NotFound();
    }
});

app.MapPost("/set/{key}", async (string key, HttpRequest request, IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    var value = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(request.Body);

    if (value != null && value.TryGetValue("value", out var newValue))
    {
        // Write-through: update both cache and database
        database[key] = newValue;
        await db.StringSetAsync(key, newValue);
        return Results.Ok(new { key, value = newValue });
    }
    return Results.BadRequest();
});

app.Run();
