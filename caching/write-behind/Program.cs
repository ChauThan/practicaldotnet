using StackExchange.Redis;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Simulated database
var database = new ConcurrentBag<string>();

// Redis connection
var redis = ConnectionMultiplexer.Connect("localhost:6379");
var cache = redis.GetDatabase();

// Buffer for write-behind
var writeBehindBuffer = new ConcurrentQueue<string>();

// Background task to flush buffer to "database"
var flushTask = Task.Run(async () =>
{
    while (true)
    {
        while (writeBehindBuffer.TryDequeue(out var value))
        {
            database.Add(value); // Simulate DB write
        }
        await Task.Delay(5000); // Flush every 5 seconds
    }
});

// API to add data (write-behind)
app.MapPost("/add", async (HttpRequest request) =>
{
    var value = await request.ReadFromJsonAsync<string>();
    if (value == null)
    {
        return Results.BadRequest("Value cannot be null.");
    }

    await cache.StringSetAsync(value, value); // Write to cache
    writeBehindBuffer.Enqueue(value);         // Queue for DB write
    return Results.Ok("Added to cache, will write to DB soon.");

});

// API to get all data from "database"
app.MapGet("/db", () => database.ToArray());

// API to get from cache
app.MapGet("/cache/{key}", async (string key) =>
{
    var value = await cache.StringGetAsync(key);
    return value.HasValue ? Results.Ok(value.ToString()) : Results.NotFound();
});

app.Run();
