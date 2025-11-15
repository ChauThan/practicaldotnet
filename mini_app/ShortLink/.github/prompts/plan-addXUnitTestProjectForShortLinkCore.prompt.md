## Plan: Add xUnit Test Project for ShortLink.Core

Create a new xUnit test project named 'ShortLink.Core.Tests' in the .NET solution, add it to the solution file, and configure a project reference to 'ShortLink.Core' to enable unit testing of the core logic, targeting .NET 10 for compatibility.

### Steps
1. Create a new directory `ShortLink.Core.Tests` at the solution root and generate an xUnit project using `dotnet new xunit --framework net10.0`.
2. Add the new project to `ShortLink.sln` using `dotnet sln add ShortLink.Core.Tests/ShortLink.Core.Tests.csproj`.
3. Add a project reference to `ShortLink.Core` in `ShortLink.Core.Tests.csproj` by editing the file to include `<ProjectReference Include="..\ShortLink.Core\ShortLink.Core.csproj" />`.
4. Build the solution with `dotnet build` to verify references and configurations.
5. Run initial tests with `dotnet test` to confirm xUnit setup and discovery.

### Further Considerations
1. Ensure .NET SDK 10.0.100 is installed and active, as specified in `global.json`.
2. Optionally add `xunit.runner.visualstudio` package for better Visual Studio integration.
