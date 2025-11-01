# Setup Server Plan

## Overview
Create a .NET Core Web API backend that:
- Finds an available TCP port dynamically on localhost
- Reports the port via stdout using `PORT_READY:[PORT]` format
- Exposes a `/api/status` endpoint with port information

## Architecture
```
Server/
├── Program.cs              # Main entry point with port discovery logic
├── Controllers/
│   └── StatusController.cs # API endpoint handler
├── Server.csproj           # Project configuration
└── bin/
    └── Backend.exe         # Compiled executable
```

## Implementation Steps

### 1. Create .NET Core Project Structure
- **Action:** Initialize new ASP.NET Core Web API project
- **Command:** `dotnet new webapi -n Server -o Server`
- **Location:** `Server/` directory in monorepo root
- **Output:** 
  - `Server.csproj`
  - `Program.cs` (minimal hosting setup)
  - Default controller templates

### 2. Implement Port Discovery Function
**File:** `Program.cs`

**Requirements:**
- Create a helper function `GetAvailablePort()` that:
  - Scans localhost for available TCP ports
  - Starts from port 5000 (standard .NET range)
  - Returns first available port
  - Handles socket exceptions gracefully

**Algorithm:**
```
1. Create TcpListener on localhost with port 0 (OS auto-assigns)
2. Get the assigned port from TcpListener
3. Close listener and return port
```

**Code Pattern:**
```csharp
private static int GetAvailablePort()
{
    var listener = new TcpListener(IPAddress.Loopback, 0);
    listener.Start();
    int port = ((IPEndPoint)listener.LocalEndpoint).Port;
    listener.Stop();
    return port;
}
```

### 3. Configure Kestrel for Dynamic Port
**File:** `Program.cs`

**Requirements:**
- Use the discovered port instead of default 5000
- Configure before `app.Run()`
- Print port to stdout BEFORE starting host

**Implementation:**
```csharp
var port = GetAvailablePort();
var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on the dynamic port
builder.WebHost.UseUrls($"http://localhost:{port}");

// Build and print port BEFORE running
var app = builder.Build();
Console.WriteLine($"PORT_READY:{port}");
Console.Out.Flush();
app.Run();
```

### 4. Create StatusController API
**File:** `Controllers/StatusController.cs`

**Requirements:**
- Create `StatusController` inheriting from `ControllerBase`
- Route: `/api/status`
- HTTP Method: GET
- Response: JSON object with:
  - `status`: "Running" or similar confirmation
  - `port`: Current port number
  - `timestamp`: Server start time or current time

**Response Example:**
```json
{
  "status": "Running",
  "port": 5001,
  "timestamp": "2025-11-01T12:00:00Z"
}
```

**Code Pattern:**
```csharp
[ApiController]
[Route("api")]
public class StatusController : ControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        var port = int.Parse(Environment.GetEnvironmentVariable("API_PORT") ?? "5000");
        return Ok(new { status = "Running", port, timestamp = DateTime.UtcNow });
    }
}
```

### 5. Build Configuration
**File:** `Server.csproj`

**Requirements:**
- Ensure self-contained build output
- Output to `bin/` as `Backend.exe`
- Set target framework to .NET 6 or 7
- Configure for Windows runtime

**Build Command:**
```
dotnet publish -c Release -o bin --self-contained -r win-x64
```

## Testing Strategy

### Local Testing
1. Run: `dotnet run` from `Server/` directory
2. Verify: Look for `PORT_READY:[PORT]` in console output
3. Test API: `curl http://localhost:[PORT]/api/status`
4. Verify Response: Check JSON contains port and status

### Build Testing
1. Run: `dotnet publish -c Release -o bin`
2. Execute: `Server/bin/Backend.exe`
3. Verify: Same output and API functionality

## Dependencies
- .NET SDK 6.0+ installed
- Windows build target (win-x64)
- System.Net.Sockets for port discovery

## Success Criteria
- ✅ Port discovery function works reliably
- ✅ `PORT_READY:[PORT]` prints to stdout before server starts
- ✅ `/api/status` endpoint accessible at dynamic port
- ✅ Backend.exe runs from command line and reports port
- ✅ No hardcoded port numbers
