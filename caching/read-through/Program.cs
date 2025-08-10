using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add Redis connection multiplexer as a singleton
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost:6379")
);

var app = builder.Build();

app.MapGet("/data/{key}", async (string key, IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    var cachedValue = await db.StringGetAsync(key);

    if (cachedValue.HasValue)
    {
        return Results.Ok(new { source = "cache", value = cachedValue.ToString() });
    }

    // Simulate fetching from a data source (e.g., database)
    var dataSourceValue = $"Value for {key} at {DateTime.Now}";

    // Store in Redis for future requests
    await db.StringSetAsync(key, dataSourceValue, TimeSpan.FromMinutes(5));

    return Results.Ok(new { source = "data source", value = dataSourceValue });
});

app.Run();