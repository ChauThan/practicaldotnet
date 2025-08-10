# Write-Through Caching Sample (.NET)

This project demonstrates a simple write-through caching pattern using ASP.NET Core and Redis (via StackExchange.Redis). It features an in-memory dictionary as the backing store and Redis as the cache.

## How It Works
- **GET /get/{key}**: Retrieves a value by key. Checks Redis cache first; if not found, falls back to the in-memory database and repopulates the cache.
- **POST /set/{key}**: Sets a value for a key. Updates both the in-memory database and the Redis cache (write-through).

## Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Redis](https://redis.io/) running locally on `localhost:6379`

## Running the App
1. Start Redis locally.
2. Run the app:
   ```pwsh
   dotnet run
   ```
3. Use an HTTP client (e.g., curl, Postman) to interact with the endpoints.

## Example Requests
### Set a Value
```bash
curl -X POST http://localhost:5000/set/mykey -H "Content-Type: application/json" -d '{"value":"hello world"}'
```

### Get a Value
```bash
curl http://localhost:5000/get/mykey
```

## Endpoints
- `GET /get/{key}`: Returns `{ source: "cache" | "database", value: string }` or 404 if not found.
- `POST /set/{key}`: Accepts `{ "value": string }` JSON body. Returns `{ key, value }`.

## Notes
- This sample uses an in-memory dictionary for demonstration. In production, replace with a persistent database.
- The cache is repopulated on reads if the value is found in the database but not in Redis.

## License
MIT
