# Cache-Aside Pattern with Redis

This project demonstrates the Cache-Aside pattern using Redis as the caching layer. It is implemented in .NET 9 and uses the StackExchange.Redis library for Redis integration.

## Features
- Retrieve data from an in-memory "database" or Redis cache.
- Automatically cache data on retrieval from the "database".
- Update data in the "database" and invalidate the cache.

## Prerequisites

### Redis
Redis must be installed and running on your local machine or accessible from the application. By default, the application connects to Redis at `localhost:6379`. You can modify this connection string in the `Program.cs` file.

### .NET 9 SDK
Ensure that the .NET 9 SDK is installed on your machine.

## How to Run
1. Clone the repository.
2. Navigate to the `cache-aside` directory.
3. Run the application using the following command:
   ```
   dotnet run
   ```

## Endpoints

### GET `/data/{key}`
Retrieves data for the specified key. If the data is not in the cache, it fetches it from the "database" and stores it in the cache.

### POST `/data/{key}`
Updates the value for the specified key in the "database" and invalidates the cache.

## Example

### Retrieve Data
```bash
curl http://localhost:5000/data/1
```

### Update Data
```bash
curl -X POST http://localhost:5000/data/1 -H "Content-Type: application/json" -d '{"value": "New Value"}'
```
