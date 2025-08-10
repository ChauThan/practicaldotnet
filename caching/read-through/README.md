# Read-Through Caching Example (.NET)

This project demonstrates a simple read-through caching pattern using ASP.NET Core and Redis via the StackExchange.Redis library.

## Features
- ASP.NET Core minimal API
- Redis-backed cache for GET requests
- Simulated data source fallback
- Automatic cache population and expiration

## How It Works
- When a GET request is made to `/data/{key}`:
  - The app checks Redis for the value.
  - If found, it returns the cached value.
  - If not found, it simulates fetching from a data source, stores the result in Redis (with a 5-minute expiration), and returns it.

## Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Redis](https://redis.io/) running locally on `localhost:6379`

## Getting Started
1. Clone the repository:
   ```pwsh
   git clone https://github.com/ChauThan/practicaldotnet.git
   cd practicaldotnet/caching/read-through
   ```
2. Build and run the project:
   ```pwsh
   dotnet build
   dotnet run
   ```
3. Test the API:
   - Use a tool like `curl` or Postman:
     ```pwsh
     curl http://localhost:5000/data/mykey
     ```
   - The first request will fetch from the data source and cache the result. Subsequent requests within 5 minutes will return the cached value.

## Project Structure
- `Program.cs`: Main application logic
- `appsettings.json`: Configuration files
- `read-through.csproj`: Project file

## License
MIT
