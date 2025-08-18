# Memory Cache Example

This project demonstrates how to use ASP.NET Core's in-memory caching (`IMemoryCache`) in a minimal API application.

## Features
- Adds the `IMemoryCache` service to the DI container
- Provides a `/cache` endpoint that:
  - Returns a cached value if present
  - Sets and caches a value for 60 seconds if not present

## Usage
1. Build and run the application:
   ```pwsh
   dotnet run
   ```
2. Access the cache endpoint:
   - Open your browser or use curl:
     ```pwsh
     curl http://localhost:5000/cache
     ```
   - The first request sets and returns the cached value. Subsequent requests within 60 seconds return the same value.

## Code Example
```csharp
app.MapGet("/cache", (IMemoryCache cache) =>
{
    string cacheKey = "myData";
    if (!cache.TryGetValue(cacheKey, out string? cachedValue))
    {
        cachedValue = $"Cached at {DateTime.Now}";
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
        cache.Set(cacheKey, cachedValue, cacheEntryOptions);
    }
    return cachedValue;
});
```

## Requirements
- .NET 9.0 SDK

## License
MIT
