# HybridCache

The HybridCache API bridges some gaps in the IDistributedCache and IMemoryCache APIs. HybridCache is an abstract class with a default implementation that handles most aspects of saving to cache and retrieving from cache.
HybridCache has the following features that the other APIs don't have:
- A unified API for both in-process and out-of-process caching.
- Stampede protection.
- Configurable serialization.

[Source](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/overview?view=aspnetcore-9.0#hybridcache)