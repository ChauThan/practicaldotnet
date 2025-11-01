# Setup Server Plan

## Overview
Set up the .NET Core Web API backend that will run on a dynamic port and provide a status endpoint.

## Requirements Analysis
- Technology: .NET Core Web API (C#)
- Key Features:
  - Find an available TCP port on localhost
  - Configure Kestrel to listen on the dynamic port
  - Print port information to stdout in format "PORT_READY:[port]"
  - Provide GET /api/status endpoint returning JSON with status and port

## Implementation Strategy
1. **Project Initialization**
   - Create a new .NET Core Web API project in the Server directory
   - Use .NET CLI to scaffold the project
   - Ensure .NET 6+ is installed (based on current version check)

2. **Dynamic Port Discovery**
   - Implement a method to find an available TCP port
   - Use TcpListener to test port availability
   - Start from a reasonable port range (e.g., 5000-6000)

3. **Kestrel Configuration**
   - Modify Program.cs to configure Kestrel with the dynamic port
   - Set up the web host builder
   - Ensure localhost binding only

4. **Port Reporting**
   - Add console output before starting the host
   - Format: "PORT_READY:[port_number]"
   - Ensure output is flushed immediately

5. **API Controller**
   - Create a StatusController or use default WeatherForecast
   - Implement GET /api/status endpoint
   - Return JSON: {"status": "running", "port": port_number}

6. **Build and Packaging**
   - Configure for self-contained executable
   - Output to Server/bin/Backend.exe (Windows executable)
   - Ensure cross-platform compatibility if needed

## Dependencies
- .NET Core SDK
- No external NuGet packages required initially

## Testing Strategy
- Unit test for port discovery method
- Integration test for API endpoint
- Manual test: run executable and check stdout output

## Potential Challenges
- Port conflicts in development environment
- Firewall/antivirus blocking localhost ports
- Ensuring executable path is correct for Electron spawn

## Success Criteria
- Executable runs and prints PORT_READY message
- API endpoint responds with correct JSON
- No port conflicts during startup