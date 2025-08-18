using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add IMemoryCache service
builder.Services.AddMemoryCache();

var app = builder.Build();

app.MapGet("/cache", (IMemoryCache cache) =>
{
    string cacheKey = "myData";
    if (!cache.TryGetValue(cacheKey, out string? cachedValue))
    {
        // Value not in cache, so set it
        cachedValue = $"Cached at {DateTime.Now}";
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
        cache.Set(cacheKey, cachedValue, cacheEntryOptions);
    }
    return cachedValue;
});

app.Run();
