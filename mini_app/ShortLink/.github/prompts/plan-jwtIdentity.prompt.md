Plan: Add ASP.NET Core Identity + JWT authentication

Goal: Protect admin-only endpoints so that creating and deleting short links require an admin JWT token. Keep redirecting short links public.

High-level Steps

1. Add required NuGet packages
   - Microsoft.AspNetCore.Authentication.JwtBearer (API project)
   - Microsoft.AspNetCore.Identity.EntityFrameworkCore (Infrastructure project)
   - Microsoft.IdentityModel.Tokens (API project)

2. Add the Identity user model
   - Create `ApplicationUser` in `ShortLink.Infrastructure/Identity/ApplicationUser.cs`.
   - Optionally keep any domain properties required for the app.

3. Update DbContext
   - Change `ShortLink.Infrastructure/ShortLinkDbContext.cs` to inherit `IdentityDbContext<ApplicationUser>`.
   - Keep Link entity config; Identity will add AspNet tables as new entities.

4. Configure JWT and Identity in API
   - Add `Jwt` settings in `ShortLink.Api/appsettings.json`:
     ```json
     "Jwt": {
       "Key": "<strong-secret>",
       "Issuer": "ShortLinkApi",
       "Audience": "ShortLinkClients",
       "ExpiresMinutes": 60
     }
     ```
   - Register Identity and JWT in `Program.cs` / `Startup.cs`:
     - `services.AddIdentity<ApplicationUser, IdentityRole>()...`
     - `services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)` and `AddJwtBearer`
     - `app.UseAuthentication(); app.UseAuthorization();`

5. Add TokenService
   - New service to create JWT access tokens from `UserManager` and role claims.
   - File: `ShortLink.Api/Services/TokenService.cs`.

6. Add AuthController
   - File: `ShortLink.Api/Controllers/AuthController.cs`.
   - POST `/api/auth/login` accepts username/password, returns `Bearer <token>`.
   - Optionally `register` for dev; use seeding for production.

7. Protect endpoints
   - Mark `CreateLink` and `DeleteLink` methods or `LinksController` with `[Authorize(Roles = "Admin")]`.
   - Keep `RedirectController` and `GET /{shortCode}` `[AllowAnonymous]` so public users can redirect.

8. Create migrations
   - After updating the DbContext, run EF migrations to create Identity tables.
   - Example:
     ```pwsh
     dotnet ef migrations add AddIdentity -p ShortLink.Infrastructure -s ShortLink.Api -c ShortLinkDbContext
     dotnet ef database update -p ShortLink.Infrastructure -s ShortLink.Api -c ShortLinkDbContext
     ```

9. Add seeder for Admin user + role
   - Implement `IdentitySeeder` that runs on startup to create Admin role and a seeded admin user (password from env var or secrets).

Security considerations

- Store `Jwt:Key` securely (User Secrets in dev, Azure Key Vault or environment variable in production).
- Use `IConfiguration` to read JWT settings in `TokenService`.
- Use `PasswordValidators` / `SignInManager` to safely authenticate.
- Keep token lifetime reasonably short and optionally implement refresh tokens for a better UX.

Files to change

- `ShortLink.Infrastructure/ShortLinkDbContext.cs` (inherit `IdentityDbContext<ApplicationUser>`)
- `ShortLink.Infrastructure/Identity/ApplicationUser.cs` (new)
- `ShortLink.Api/Program.cs` or `ShortLink.Api/Startup.cs` (Identity + JWT registration)
- `ShortLink.Api/appsettings.json` (add JWT block)
- `ShortLink.Api/Controllers/AuthController.cs` (new)
- `ShortLink.Api/Services/TokenService.cs` (new)
- `ShortLink.Api/Controllers/LinksController.cs` (add `[Authorize]` on create/delete)
- `ShortLink.Api/Controllers/RedirectController.cs` (ensure `[AllowAnonymous]` on redirect endpoint)

Migrations

- `AddIdentity` — to create ASP.NET Identity tables
- Optional `AddRefreshTokens` — if you persist long-lived refresh tokens

Next steps

1. Create the `ApplicationUser` and update `ShortLinkDbContext`.
2. Add `Jwt` settings and register Identity + JWT.
3. Add `TokenService`, `AuthController`, and seeding.
4. Protect admin endpoints and keep redirect public.

Notes

- This file is a first draft of the plan and can be refined. Secrets or admin credentials must be set with environment variables or secrets.
- If you prefer Identity to be kept in a separate DbContext, that approach can be accommodated but will require extra wiring.

End of plan
