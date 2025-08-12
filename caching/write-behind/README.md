# Write-Behind Caching Example

This project demonstrates a simple write-behind caching pattern using ASP.NET Core and StackExchange.Redis. Data is first written to a Redis cache and then asynchronously flushed to a simulated database.

## Features
- **Write-Behind Buffer:** Data is queued and periodically flushed to the database.
- **Redis Integration:** Uses StackExchange.Redis for caching.
- **REST API:**
  - `POST /add`: Adds a value to the cache and queues it for database write.
  - `GET /db`: Retrieves all values from the simulated database.
  - `GET /cache/{key}`: Retrieves a value from the cache by key.

## How It Works
1. Data is added via the `/add` endpoint. It is written to Redis and queued for database write.
2. A background task flushes the buffer to the database every 5 seconds.
3. You can query the database or cache via the provided endpoints.

## Prerequisites
- .NET 9.0 SDK
- Redis server running on `localhost:6379`

## Running the Project
1. Start your Redis server locally.
2. Build and run the project:
   ```pwsh
   dotnet run
   ```
3. Use tools like `curl` or Postman to interact with the API endpoints.

## Example Requests
- Add data:
  ```pwsh
  curl -X POST http://localhost:5000/add -H "Content-Type: application/json" -d '"myValue"'
  ```
- Get all database values:
  ```pwsh
  curl http://localhost:5000/db
  ```
- Get value from cache:
  ```pwsh
  curl http://localhost:5000/cache/myValue
  ```

## License
MIT
