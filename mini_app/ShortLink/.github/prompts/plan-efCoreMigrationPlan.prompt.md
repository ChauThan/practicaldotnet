# EF Core + SQLite Migration Plan for ShortLink

This file captures the plan to convert the in-memory repository to an EF Core backed repository with SQLite for persistent storage. Keep this file for refinement and to track steps while implementing.

## Goals
- Create a `ShortLinkDbContext` that knows about `Link`.
- Implement `EfLinkRepository` that uses `ShortLinkDbContext` and implements `ILinkRepository`.
- Wire the `DbContext` and repo into DI so the app uses the EF-backed repo.
- Create and commit EF migrations to create the database schema (SQLite).
- Keep `InMemoryLinkRepository` for unit tests or fallback.

## Proposed file structure / where to put code
- Project: `ShortLink.Infrastructure` or keep inside `ShortLink.Api` if you prefer fewer projects.
  - `ShortLink.Infrastructure.csproj` (references `ShortLink.Core`)
  - `ShortLinkDbContext.cs` (DbContext class)
  - `Repositories/EfLinkRepository.cs` (implementation of `ILinkRepository`)
  - `Migrations/` (migration files)

## DbContext design
- Class: `ShortLinkDbContext : DbContext`
- Add DbSet: `public DbSet<Link> Links { get; set; }`
- Configure model:
  - `modelBuilder.Entity<Link>().HasKey(l => l.Id);`
  - `modelBuilder.Entity<Link>().HasIndex(l => l.ShortCode).IsUnique();`
  - Optionally configure string length: ShortCode (6-12), OriginalUrl length or leave as text.

## EfLinkRepository (implements ILinkRepository)
- Use constructor injection: `private readonly ShortLinkDbContext _db;`
- Implement methods using EF:
  - `AddLink(Link link)` -> `link.Id = Guid.NewGuid(); await _db.Links.AddAsync(link); await _db.SaveChangesAsync(); return link;`
  - `GetLinkById(Guid id)` -> `_db.Links.FindAsync(id)` or `FirstOrDefaultAsync`.
  - `GetByShortCode(string shortCode)` -> `_db.Links.FirstOrDefaultAsync(l => l.ShortCode == shortCode)`
  - `GetAllLinks()` -> `_db.Links.ToListAsync()`
  - `DeleteLink(Guid id)` -> fetch then `Remove()` + `SaveChangesAsync()`
  - `UpdateLink(Link link)` -> attach/lookup and update properties; call `SaveChangesAsync()`

### Concurrency and uniqueness
- `ShortCode` should have a unique index in the database.
- Consider transactional retry logic if collisions happen during generation.

## Dependency Injection
- In `ShortLink.Api/Startup.cs` (or `Program.cs` for minimal setup):
  - Add to `ConfigureServices`:

```csharp
services.AddDbContext<ShortLinkDbContext>(options =>
    options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

services.AddScoped<ILinkRepository, EfLinkRepository>();
services.AddScoped<ILinkService, LinkService>();
```

- Add `ConnectionStrings` to `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=shortlink.db"
}
```

## Packages & Migrations
- Add EF packages to `ShortLink.Infrastructure` or the project with `DbContext`:
  - `Microsoft.EntityFrameworkCore.Sqlite`
  - `Microsoft.EntityFrameworkCore.Design` (for tools)
  - (optional) `Microsoft.EntityFrameworkCore.Tools`

- Add tooling if needed: `dotnet tool install --global dotnet-ef --version 10.0.0`

- Create initial migration (pwsh):

```pwsh
dotnet ef migrations add InitialCreate -s ShortLink.Api -p ShortLink.Infrastructure -o Migrations
dotnet ef database update -s ShortLink.Api -p ShortLink.Infrastructure
```

Notes:
- `-s` = startup project (the Web Api project), `-p` = project containing DbContext (infrastructure)
- Commit the generated migration files into source control.

## Testing
- Keep `InMemoryLinkRepository` used by unit tests.
- Add integration tests for `EfLinkRepository` using an in-memory SQLite configuration:
  - Use `options.UseSqlite("DataSource=:memory:")` and keep the connection open for the test scope.
  - Test collisions and that `ShortCode` uniqueness is enforced.

## Rollout and fallback
- In development, you can keep `InMemoryLinkRepository` as fallback. Use environment-based DI registration if desired.
- For production, switch to `EfLinkRepository`.

## Optional refinements
- Add seeding data or health checks.
- Add unique ShortCode generation with DB uniqueness check or handle exception when unique constraint fails.

---

This plan mirrors the earlier analysis and is ready for refinement or to be implemented. For the next phase I can scaffold `ShortLinkDbContext` and `EfLinkRepository`, update DI, add packages, and create the initial migration. 
