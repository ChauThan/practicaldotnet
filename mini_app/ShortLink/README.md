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

## Authentication & Admin (Identity + JWT)

ShortLink includes optional ASP.NET Core Identity with JWT authentication. Identity is only activated when `UseEfRepository` is set to `true` in `ShortLink.Api/appsettings.json` (this uses the EF-backed SQLite DB). The redirect endpoint remains public — only admin operations require a JWT.

Enable Identity + JWT locally:

1. Set EF repository and create the database (migrations are included):

```pwsh
# enable EF-backed repository (edit appsettings.json or set env var)
$env:UseEfRepository = 'true'

# ensure migrations are applied
dotnet ef database update -p ShortLink.Infrastructure -s ShortLink.Api -c ShortLinkDbContext
``` 

2. Configure JWT secret (recommended to use secrets or env vars in dev):

```pwsh
# set JWT key (use strong random secret in real deployments)
$env:Jwt__Key = 'your-very-strong-secret'
$env:Jwt__Issuer = 'ShortLinkApi'
$env:Jwt__Audience = 'ShortLinkClients'

# optionally override seeded admin credentials
$env:Admin__User = 'admin'
$env:Admin__Email = 'admin@shortlink.local'
$env:Admin__Password = 'ReplaceWithStrongerPassword!'
```

3. Start the API:

```pwsh
dotnet run --project ShortLink.Api
```

4. Authenticate as admin to get a JWT (AuthController):

```pwsh
# login and get token
curl -X POST -H "Content-Type: application/json" \
   -d '{"username":"admin", "password":"DevAdmin123!"}' \
   http://localhost:5000/api/auth/login

# response: { "token": "<JWT>" }

# use token for admin endpoints
curl -H "Authorization: Bearer <JWT>" \
   -X POST -H "Content-Type: application/json" -d '{"url":"https://example.com"}' \
   http://localhost:5000/api/links
```

Notes on seeding: the `IdentitySeeder` will create an `Admin` role and a seeded admin user when `UseEfRepository` is true and the DB is empty. The seeded password is `DevAdmin123!` by default — override with `Admin__Password` as shown above.

Security considerations (important):

- Never store `Jwt:Key` in source code; use User Secrets, environment variables, or a secret manager.
- Use a long, high-entropy key (e.g., 256+ bits) and rotate it regularly if possible.
- Use HTTPS in production and require secure cookies if sessions are used.
- Consider adding refresh tokens or rotating access tokens for longer sessions; refresh tokens should be stored server-side.
- Limit the privileges of seeded accounts and remove or change the dev password before production.

Next steps and improvements:

- Add registration or admin management endpoints (protected) so admins can be added/removed via API.
- Implement refresh tokens for a better UX with short-lived access tokens.
- Consider storing Identity in a separate DB if you need a different lifecycle for auth data.
- Integrate a secret store (Key Vault, AWS Secrets Manager, etc.) for production secret management.

## License
This project is licensed under the MIT License. See the LICENSE file for details.