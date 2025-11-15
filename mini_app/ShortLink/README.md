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

## License
This project is licensed under the MIT License. See the LICENSE file for details.