# ShortLink Project

## Overview
ShortLink is a lightweight URL shortening microservice. It provides an HTTP API to create short links for long URLs and to resolve, list, or delete stored short links.

## Solution structure (high level)
The solution is an ASP.NET Core Web API (`ShortLink.Api`) plus a class library with the core business implementation (`ShortLink.Core`). Both projects are part of the `ShortLink.sln` solution.

### Visual overview
Here's a simple diagram that shows how the pieces connect:

```
Client (browser / CLI)
   |
   |  HTTP REST
   v
ShortLink.Api (ASP.NET Core Web API)
   ├─ Controllers/LinksController.cs  (exposes HTTP endpoints for /api/links)
   ├─ Program.cs / Startup.cs         (app & DI configuration)
   └─ appsettings.json                (config)
        |
        | depends-on
        v
ShortLink.Core (business logic)
   ├─ Interfaces/ILinkRepository.cs   (repository contract)
   ├─ Models/Link.cs                  (data model)
   └─ Services/LinkService.cs         (business logic; implements repository use)
```

Simplified request flow:
1. Client POSTs long URL to `/api/links` -> `LinksController` -> `LinkService` creates link and returns short id.
2. Client GETs `/api/links/{id}` -> `LinksController` -> `LinkService` -> repository returns `Link` model -> returns details.
3. Client DELETEs `/api/links/{id}` -> `LinksController` -> `LinkService` -> repository removes link.

### Project layout
The solution contains two projects:

- `ShortLink.Api` — the ASP.NET Core Web API for HTTP endpoints and app configuration.
- `ShortLink.Core` — the class library that contains business logic, models, and service interfaces.

## Setup & quick start
1. Clone the repo and restore NuGet packages.

```pwsh
git clone <repository-url>
cd ShortLink
dotnet restore
```

2. Run the API from the `ShortLink.Api` folder:

```pwsh
cd ShortLink.Api
dotnet run
```

3. Try a basic flow with curl or httpie:

```pwsh
# create a short link
curl -X POST -H "Content-Type: application/json" -d '{"url": "https://example.com/long-url"}' http://localhost:5000/api/links

# retrieve the created link
curl http://localhost:5000/api/links/{id}

# delete a link
curl -X DELETE http://localhost:5000/api/links/{id}
```

## Contributing
Contributions welcome — send a PR, add tests, document features, or improve performance.

## EF Core + SQLite support
This project ships with an in-memory repository for development and tests. There is also an EF Core backed repository using SQLite.

How to enable EF-backed repository locally:

1. Open `ShortLink.Api/appsettings.json` and set `UseEfRepository` to `true` (default is `false`).

2. Build and run migrations (this creates `shortlink.db` file by default):

```pwsh
dotnet tool restore
dotnet ef migrations add InitialCreate -s ShortLink.Api -p ShortLink.Infrastructure -o Migrations
dotnet ef database update -s ShortLink.Api -p ShortLink.Infrastructure
```

3. Start the API (it will use the EF repository when `UseEfRepository` is `true`).

Notes:
- The `ShortLink.Infrastructure` project contains `ShortLinkDbContext` and `EfLinkRepository`.
- Migrations are committed to `ShortLink.Infrastructure/Migrations`.

## License
This project is licensed under the MIT License. See the LICENSE file for details.