## Integrate Server and Client — Monorepo Wiring & Run Flows

### Goal
Wire the `Client` and `Server` folders so the Electron client can spawn the backend produced by the Server project. Provide clear dev and publish flows and a small smoke script to validate end-to-end behavior.

### Desired behavior
- Developer runs a small set of commands and either:
  - Starts the backend separately (dev flow) and runs the client, OR
  - Publishes the backend to `Server/bin` and runs the client which spawns the executable.

### Integration steps
1. Publish server binary to `Server/bin` (recommended for Windows dev):
   ```pwsh
   dotnet publish Server -c Release -o Server/bin -r win-x64
   ```
   This produces `Server/bin/Backend.exe` on Windows. For cross-platform/dev, you may publish a DLL instead and spawn `dotnet Backend.dll`.
2. Implement robust spawn logic in `Client/main.js`:
   - Resolve absolute path to `Server/bin/Backend.exe`.
   - If exe exists, spawn it directly.
   - Else if `Backend.dll` exists, spawn `dotnet Backend.dll`.
   - Else fall back to optionally running `dotnet run --project Server` (for dev) — or fail with a clear error message.
3. Add timeouts and error handling for the `PORT_READY` wait (e.g., 20s) and surface helpful logs to developer.

### Dev flow (fast iteration)
1. Start backend in a separate terminal:
   ```pwsh
   dotnet run --project Server
   ```
2. Start client:
   ```pwsh
   cd Client
   npm run start
   ```

### Publish flow (single-run spawn)
1. From repo root publish server for Windows:
   ```pwsh
   dotnet publish Server -c Release -o Server/bin -r win-x64
   ```
2. Install client deps and start:
   ```pwsh
   cd Client
   npm install
   npm run start
   ```

### Helper smoke script (optional)
Create `run-smoke.ps1` at repo root that:
- publishes server to `Server/bin`,
- starts electron from `Client` (or starts client which spawns exe),
- tails logs and verifies that `PORT_READY:` is emitted and the client logs the `/api/status` response.

### Notes & trade-offs
- Producing a native exe requires a runtime identifier (RID) for `dotnet publish`. If you want to keep cross-platform behavior, prefer publishing a framework dependent DLL and let the client spawn `dotnet`.
- The ephemeral-port probe (Server) can have a race condition — document that this is an acceptable trade-off for a simple dev boilerplate.

### Quick checklist for successful integration
- [ ] Server prints `PORT_READY:[port]` when started.
- [ ] Client detects the port and performs `GET /api/status`.
- [ ] Renderer loads `index.html`.
