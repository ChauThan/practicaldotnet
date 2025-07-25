# Authentication & Identity - Clean Architecture

This project demonstrates a clean architecture implementation for authentication and identity management using ASP.NET Core Identity and JWT tokens.

## Clean Architecture Structure

```
┌─────────────────────────────────────────────────────────────────┐
│                        App.Api (Presentation Layer)            │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │   Controllers   │  │    Program.cs   │  │  Configuration  │  │
│  │  - AuthController│  │  - DI Setup     │  │  - appsettings  │  │
│  │  - RolesController│ │  - Middleware   │  │  - launchSettings│ │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                   App.Application (Application Layer)          │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │    Features     │  │   Abstractions  │  │ Service Extensions│ │
│  │  ┌───────────┐  │  │  - IUserService │  │  - DI Registration│ │
│  │  │   Auth    │  │  │  - IRoleService │  │                 │  │
│  │  │ - Login   │  │  │  - IJwtService  │  │                 │  │
│  │  │ - Register│  │  │  - ISignInService│ │                 │  │
│  │  └───────────┘  │  └─────────────────┘  └─────────────────┘  │
│  │  ┌───────────┐  │                                           │
│  │  │   Roles   │  │        Uses MediatR Pattern               │
│  │  │ - Create  │  │        CQRS Implementation                │
│  │  │ - Assign  │  │                                           │
│  │  └───────────┘  │                                           │
│  └─────────────────┘                                           │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                     App.Domain (Domain Layer)                  │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │    Entities     │  │   Value Objects │  │    Results      │  │
│  │ - ApplicationUser│ │                 │  │ - SignInResult  │  │
│  │ - ApplicationRole│ │                 │  │                 │  │
│  │ - Product       │  │                 │  │                 │  │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘  │
│                                                                 │
│  Core business logic and domain rules (No dependencies)        │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                App.Infrastructure (Infrastructure Layer)       │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │   Persistence   │  │    Services     │  │   Migrations    │  │
│  │ - DbContext     │  │ - UserService   │  │ - EF Migrations │  │
│  │ - Repositories  │  │ - RoleService   │  │                 │  │
│  │                 │  │ - JwtService    │  │                 │  │
│  │                 │  │ - SignInService │  │                 │  │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘  │
│                                                                 │
│  External concerns (Database, External APIs, File System)      │
└─────────────────────────────────────────────────────────────────┘
```

## Project Dependencies

```
App.Api
├── App.Application
├── App.Infrastructure
└── App.Domain

App.Application
└── App.Domain

App.Infrastructure
├── App.Application
└── App.Domain

App.Domain
└── (No dependencies - Pure domain logic)
```

## Key Architectural Principles

### 1. **Dependency Inversion**
- Higher-level modules (Application) define abstractions (interfaces)
- Lower-level modules (Infrastructure) implement these abstractions
- Dependencies point inward toward the domain

### 2. **Separation of Concerns**
- **Domain**: Business entities and core logic
- **Application**: Use cases and business workflows (CQRS with MediatR)
- **Infrastructure**: Data access, external services, JWT generation
- **Presentation**: API controllers, HTTP concerns

### 3. **Clean Architecture Benefits**
- **Testability**: Easy to unit test business logic
- **Maintainability**: Clear separation of concerns
- **Flexibility**: Easy to swap out infrastructure components
- **Independence**: Domain layer has no external dependencies

## Technologies Used

- **ASP.NET Core 9.0** - Web API framework
- **Entity Framework Core** - ORM for data access
- **ASP.NET Core Identity** - Authentication and authorization
- **JWT Bearer Tokens** - Stateless authentication
- **MediatR** - CQRS pattern implementation
- **SQL Server** - Database provider

## Project Structure Details

### App.Domain
```
├── ApplicationUser.cs      # Identity user entity
├── ApplicationRole.cs      # Identity role entity
├── Product.cs             # Business entity example
└── SignInResult.cs        # Domain result object
```

### App.Application
```
├── Abstractions/          # Interface definitions
│   ├── IJwtService.cs
│   ├── IRoleService.cs
│   ├── ISignInService.cs
│   └── IUserService.cs
├── Feature/              # CQRS commands/queries
│   ├── Auth/
│   │   ├── Login.cs
│   │   └── Register.cs
│   └── Roles/
│       ├── AssignRoleToUser.cs
│       └── CreateRole.cs
└── ServiceExtensions/    # DI configuration
    └── ApplicationServiceExtensions.cs
```

### App.Infrastructure
```
├── Persistence/          # Data access layer
│   └── ApplicationDbContext.cs
├── Services/            # Service implementations
│   ├── JwtService.cs
│   ├── RoleService.cs
│   ├── SignInService.cs
│   └── UserService.cs
├── Migrations/          # EF Core migrations
└── ServiceExtensions/   # DI configuration
    └── InfrastructureServiceExtensions.cs
```

### App.Api
```
├── Controllers/         # API endpoints
│   ├── AuthController.cs
│   └── RolesController.cs
├── Program.cs          # Application startup
├── appsettings.json    # Configuration
└── Properties/
    └── launchSettings.json
```

## API Endpoints

### Authentication
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login

### Roles Management
- `POST /api/roles` - Create role
- `POST /api/roles/assign` - Assign role to user

## Getting Started

1. **Clone the repository**
2. **Update connection string** in `appsettings.json`
3. **Run migrations**: `dotnet ef database update`
4. **Start the application**: `dotnet run --project App.Api`
5. **Access API documentation** at `/scalar/v1`

---

This implementation follows Clean Architecture principles ensuring maintainable, testable, and loosely coupled code.
