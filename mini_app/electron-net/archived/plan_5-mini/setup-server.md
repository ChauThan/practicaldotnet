## Setup Server — Backend (.NET Core Web API)

### Contract
- Inputs: none (server will choose a free TCP port on localhost).
- Outputs: prints a single-line readiness message to stdout: `PORT_READY:[port]` and starts Kestrel listening on that port.
- Success criteria: process prints `PORT_READY:\d+` before running; GET /api/status returns JSON `{ status: "ok", port: n }`.

### Files to create / edit
- `Server/Program.cs` — host setup and port-finding helper.
- `Server/Controllers/StatusController.cs` — `GET /api/status` returning JSON status and port.
- `Server/Server.csproj` — project file (created by `dotnet new webapi`).
- `Server/README.md` — build/run instructions and notes about publishing.

### Implementation steps
1. Create the project skeleton:
   ```pwsh
   dotnet new webapi -o Server -n Backend
   ```
2. Implement a helper that finds an available TCP port. Simple approach:
   - Use a temporary `TcpListener` bound to port 0 to obtain an ephemeral port, then stop it and use that numeric port for Kestrel.
3. Configure Kestrel to listen on `localhost:[port]` using `builder.WebHost.ConfigureKestrel(...)` or `builder.WebHost.UseUrls(...)`.
4. Write `Console.WriteLine($"PORT_READY:{port}")` before calling `host.Run()` so a parent process can parse it.
5. Add `StatusController` exposing `GET /api/status` that returns `{ status = "ok", port = port }`.

### Commands (pwsh)
- Build:
  ```pwsh
  dotnet build Server
  ```
- Publish (produce an executable on Windows):
  ```pwsh
  dotnet publish Server -c Release -o Server/bin -r win-x64
  ```
- Dev run:
  ```pwsh
  dotnet run --project Server
  ```

### Edge cases & trade-offs
- Race condition: port returned by ephemeral probe can be claimed before Kestrel binds. For a simple boilerplate this is acceptable; document the trade-off. Alternative: bind Kestrel to port 0 and retrieve the actual bound port from server features after start — but that delays reporting until after host starts.
- Port permissions: avoid privileged ports <1024.
- Publish artifact: creating a native `.exe` depends on specifying a RID (e.g., `win-x64`). For cross-platform builds, prefer producing a framework-dependent DLL and let the client spawn `dotnet Backend.dll`.

### Tests / Smoke checks
1. Start the server and confirm stdout contains `PORT_READY:\d+`.
2. `curl http://localhost:[port]/api/status` returns JSON with `status: "ok"` and the same port.

### Quality gates
- Build: `dotnet build` should PASS.
- Minimal verification: server prints port and responds to `/api/status`.
