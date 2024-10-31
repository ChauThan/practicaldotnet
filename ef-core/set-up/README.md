# Setting Up EF Core
This is a sample guide on how to set up EF Core. Here are some important notes:
- Use `dotnet ef` for handling migrations.
- Create a `tool-manifest` for local tool installation: `dotnet new tool-manifest`.
- Install the EF tool locally: `dotnet tool install dotnet-ef`.
- Ensure the Database class has a constructor with a DbContextOptions parameter to use the EF tool.

Let's get started!