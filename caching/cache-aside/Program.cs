using StackExchange.Redis;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add Redis connection multiplexer as a singleton
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect("localhost:6379"));

var app = builder.Build();

// Simple in-memory "database"
var database = new Dictionary<string, string>
{
    ["1"] = "Value from DB for key 1",
    ["2"] = "Value from DB for key 2"
};

app.MapGet("/data/{key}", async (string key, IConnectionMultiplexer redis) =>
{
    var cache = redis.GetDatabase();
    var cacheKey = $"data:{key}";

    // Try to get from cache
    var cachedValue = await cache.StringGetAsync(cacheKey);
    if (cachedValue.HasValue)
    {
        return Results.Ok(new { source = "cache", value = cachedValue.ToString() });
    }

    // Cache miss: get from "database"
    if (database.TryGetValue(key, out var dbValue))
    {
        // Store in cache for next time
        await cache.StringSetAsync(cacheKey, dbValue);
        return Results.Ok(new { source = "database", value = dbValue });
    }

    return Results.NotFound();
});

app.MapPost("/data/{key}", async (string key, HttpRequest request, IConnectionMultiplexer redis) =>
{
    var cache = redis.GetDatabase();
    var cacheKey = $"data:{key}";

    // Read value from request body
    var body = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(request.Body);
    if (body == null || !body.TryGetValue("value", out var newValue))
        return Results.BadRequest();

    // Update "database"
    database[key] = newValue;

    // Invalidate cache
    await cache.KeyDeleteAsync(cacheKey);

    return Results.Ok(new { key, value = newValue });
});

app.Run();
