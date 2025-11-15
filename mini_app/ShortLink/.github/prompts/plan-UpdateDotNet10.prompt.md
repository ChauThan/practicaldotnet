## Plan: Update to .NET 10

Update the .NET framework version from 6.0 to 10.0 across the ShortLink solution, including changing TargetFramework in project files and updating NuGet package versions for compatibility.

### Steps
1. Change `<TargetFramework>net6.0</TargetFramework>` to `net10.0` in `ShortLink.Api/ShortLink.Api.csproj`.
2. Change `<TargetFramework>net6.0</TargetFramework>` to `net10.0` in `ShortLink.Core/ShortLink.Core.csproj`.
3. Update package versions to `10.0.0` in `ShortLink.Api/ShortLink.Api.csproj` for Microsoft.AspNetCore.Mvc.NewtonsoftJson, Microsoft.EntityFrameworkCore, Microsoft.EntityFrameworkCore.SqlServer, and Swashbuckle.AspNetCore.
4. Update Microsoft.Extensions.DependencyInjection version to `10.0.0` in `ShortLink.Core/ShortLink.Core.csproj`.

### Further Considerations
1. Confirm .NET 10 SDK is installed and compatible packages are available.
2. Test the application build and runtime after updates to ensure no breaking changes.
