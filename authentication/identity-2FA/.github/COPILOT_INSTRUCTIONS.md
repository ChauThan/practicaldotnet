# GitHub Copilot Instructions for This Solution

## General Guidelines
- Follow C# and .NET best practices for clean, maintainable, and secure code.
- Use dependency injection for services and avoid static classes unless necessary.
- Use async/await for all I/O-bound operations.
- Write clear XML documentation for public methods and classes.
- Use PascalCase for class, method, and property names; camelCase for local variables and parameters.

## Solution Structure
- `App.Api`: ASP.NET Core Web API project. Contains controllers, startup logic, and API configuration.
- `App.Application`: Application layer. Contains business logic, features, and service interfaces.
- `App.Domain`: Domain layer. Contains entity models and domain logic.
- `App.Infrastructure`: Infrastructure layer. Contains EF Core DbContext, migrations, and data access implementations.

## Authentication & Identity
- Use ASP.NET Core Identity for user and role management.
- Implement 2FA (Two-Factor Authentication) using built-in Identity features or external providers (e.g., authenticator apps, email, SMS).
- Store sensitive data securely. Never log or expose secrets, passwords, or tokens.
- Use `UserManager`, `SignInManager`, and `RoleManager` for user operations.
- Always validate user input and sanitize data before processing.

## API Design
- Use RESTful conventions for controller actions.
- Return appropriate HTTP status codes and error messages.
- Use DTOs (Data Transfer Objects) for API input/output, avoid exposing domain entities directly.

## Testing
- Write unit and integration tests for all business logic and API endpoints.
- Use mock dependencies for unit tests.

## Security
- Always validate JWT tokens and authentication cookies.
- Protect endpoints with `[Authorize]` attributes as needed.
- Use HTTPS for all API endpoints.

## Comments for Copilot
- When generating code, prefer using existing patterns and conventions found in this solution.
- If unsure about a service or method, check the corresponding `ServiceExtensions` or `Feature` folders for examples.
- For new features, follow the structure: Controller → Application Feature → Domain Entity/DTO → Infrastructure (if needed).

---

_This file provides Copilot with context and rules for generating code in this repository. Update as the project evolves._
