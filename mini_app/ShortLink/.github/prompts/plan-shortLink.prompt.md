Plan: Implement ShortLink API core stories

Overview

The goal: implement Story 1 and Story 2 for the ShortLink API, wiring `ILinkService` and `ILinkRepository` and using a simple in-memory repository so the app can run locally. The end result should allow:
- POST /api/links with a long URL -> returns a short link object + location header
- GET /{shortCode} -> permanent redirect to original URL

Project architecture summary

- `ShortLink.Core.Interfaces` contains `ILinkRepository` and `ILinkService`
- `ShortLink.Core.Models` contains the `Link` model
- `ShortLink.Core.Services` contains `LinkService` and `InMemoryLinkRepository`
- `ShortLink.Api` contains the controllers and DI setup in `Startup.cs` or `Program.cs`

Implementation tasks

1. Add DTO for creation in the API project:
   - `ShortLink.Api/Models/CreateLinkRequest.cs` with `public string OriginalUrl { get; set; }`

2. Expand `ILinkRepository` (ShortLink.Core.Interfaces)
   - Add: `Link? GetByShortCode(string shortCode);`
   - Add: `Task<Link?> GetByShortCodeAsync(string shortCode);` (if rest of code uses async)
   - Add (optional): `void IncrementHitCount(Link link);` or `Update(Link link)` for analytics

3. Implement repository methods in `InMemoryLinkRepository`:
   - Implement `GetByShortCode` by searching the in-memory list/dic
   - If analytics added: implement thread-safe increment of `HitCount` using locks

4. Wire `ILinkService` changes:
   - Add `Task<Link> CreateAsync(string originalUrl)` if not present
   - Add `Task<Link?> GetByShortCodeAsync(string code)` or `Task<Link?> ResolveAndTrackAsync(string code)` that increments metrics
   - In `LinkService`, call repository `GetByShortCode` or `ResolveAndTrack`

5. Update API controllers and routes:
   - `ShortLink.Api/Models/CreateLinkRequest.cs` is consumed by `LinksController` POST
   - In `LinksController` POST `/api/links` call `ILinkService.CreateAsync` and return `201 Created` with `Location: /{ShortCode}`
   - Create `ShortLink.Api/Controllers/RedirectController.cs` with route `"{code}"` and `HttpGet` to call `ILinkService.ResolveAndTrackAsync(code)`. If link found, return `RedirectPermanent(link.OriginalUrl)`; else `NotFound()`.

6. DI setup
   - Ensure `Startup` or `Program` registers `ILinkRepository` and `ILinkService` in DI
   - `AddSingleton<ILinkRepository, InMemoryLinkRepository>();` & `AddSingleton<ILinkService, LinkService>();` are fine for in-memory scenario; use `AddScoped` for DB-backed repo later.

7. Update `ShortLink.Core.Models.Link` (optional)
   - Add `public int HitCount { get; set; } = 0;` and `public DateTime? LastAccessed { get; set; }` to support tracking

8. Tests
   - Add tests to `ShortLink.Core.Tests`:
     - `CreateLink_CreatesShortCode` -> using `LinkService`, ensure repository contains new Link
     - `GetByShortCode_ReturnsLink` -> repository returns correct object
     - `RedirectController_ReturnsRedirect` -> if code exists return 301
     - If analytics used: `ResolveAndTrack_IncrementsCount`

9. Run & Validate
   - `dotnet build ShortLink.sln -c Debug`
   - `dotnet run --project ShortLink.Api/ShortLink.Api.csproj`
   - Manual test with curl:
     - `curl -i -X POST http://localhost:5000/api/links -H "Content-Type: application/json" -d "{\"originalUrl\":\"https://example.com/long/path\"}"`
     - `curl -i http://localhost:5000/someCode` should follow redirect (301) to original

Notes

- This plan keeps the in-memory repo simple and thread-safe. In a follow-up, we can implement a persistent repository (EF Core) for production.
- Do not change the unique short code generator unless the tests rely on a 6-character code. The service already ensures uniqueness using repository checks; ensure the repository `GetByShortCode` method is present to make that check efficient.

Next step

- Implement the repository method `GetByShortCode` and the `ResolveAndTrack` service method, add DTO and redirect controller. Then add tests for the new behaviors.
