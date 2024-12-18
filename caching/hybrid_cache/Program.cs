using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<Service>();

#pragma warning disable EXTEXP0018

builder.Services.AddHybridCache();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration.GetConnectionString("RedisConnectionString");
});

#pragma warning restore EXTEXP0018

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("{id}/no-cache", ([FromServices] Service service, string id) => service.GetDataFromSourceAsync(id));

app.MapGet("{id}/hybrid", ([FromServices] Service service, string id) => service.GetOrCreateAsync(id));
app.MapPost("{id}/hybrid/set", ([FromServices] Service service, string id) => service.SetAsync(id));
app.MapPost("{id}/hybrid/remove", ([FromServices] Service service, string id) => service.RemoveAsync(id));
app.MapGet("{id}/hybrid/distributed-cache",
    ([FromServices] Service service, string id) => service.GetOrCreateUsingDistributedCacheAsync(id));

app.MapGet("{id}/l2", ([FromServices] Service service, string id) => service.GetL2CacheAsync(id));

app.Run();

public class Service(HybridCache cache, IDistributedCache l2)
{
    public async ValueTask<Item> GetOrCreateAsync(string id, CancellationToken cancellationToken = default)
    {
        return await cache.GetOrCreateAsync(
            GetCacheKey(id),
            async cancel => await GetDataFromSourceAsync(id, cancel),
            cancellationToken: cancellationToken);
    }

    public async ValueTask<Item> GetOrCreate2Async(string id, CancellationToken cancellationToken = default)
    {
        // The alternative overload might reduce some overhead from captured variables and per-instance callbacks,
        // but at the expense of more complex code
        return await cache.GetOrCreateAsync(
            GetCacheKey(id),
            (id, obj: this),
            static async (state, token) => await state.obj.GetDataFromSourceAsync(state.id, token),
            cancellationToken: cancellationToken
        );
    }

    public async ValueTask<Item?> GetL2CacheAsync(string id, CancellationToken cancellationToken = default)
    {
        var bytes = await l2.GetAsync(id, cancellationToken);
        Utf8JsonReader reader = new Utf8JsonReader(bytes);
        
        return JsonSerializer.Deserialize<Item>(ref reader);
    }
    
    public async ValueTask<Item> GetOrCreateUsingDistributedCacheAsync(string id, CancellationToken cancellationToken = default)
    {
        return await cache.GetOrCreateAsync(
            GetCacheKey(id),
            async cancelToken => await GetDataFromSourceAsync(id, cancelToken),
            cancellationToken: cancellationToken);
    }

    public async ValueTask SetAsync(string id, CancellationToken cancellationToken = default)
    {
        var data = await GetDataFromSourceAsync(id, cancellationToken);
        await cache.SetAsync(GetCacheKey(id), data, cancellationToken: cancellationToken);
    }

    public async ValueTask RemoveAsync(string id, CancellationToken cancellationToken = default)
    {
        await cache.RemoveAsync(GetCacheKey(id), cancellationToken);
    }

    public ValueTask RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("This feature is still under development");
    }

    public async ValueTask<Item> GetDataFromSourceAsync(string id, CancellationToken cancellationToken = default)
    {
        await Task.Delay(500, cancellationToken);
        return new Item(id);
    }
    
    private string GetCacheKey(string id) => $"HybridCache:{id}";
}

public record Item(string Id)
{
    public string Name { get; } = $"Name {Id}";
}