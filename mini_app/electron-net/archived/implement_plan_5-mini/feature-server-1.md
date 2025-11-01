---
goal: "Feature: Setup Server (Dynamic-port .NET Core Web API)"
version: 1.0
date_created: 2025-11-01
last_updated: 2025-11-01
owner: Team/Dev
status: 'Planned'
tags: ["feature","server","dotnet","kestrel","port-discovery"]
---

# Introduction

![Status: Planned](https://img.shields.io/badge/status-Planned-blue)

This plan describes deterministic, step-by-step tasks to create the Server component for the Electron + .NET Core monorepo. The Server is a .NET Core Web API that chooses an available TCP port on localhost, configures Kestrel to listen on that port, prints the chosen port to stdout in the exact format `PORT_READY:[port]`, and exposes a sample endpoint `GET /api/status`.

## 1. Requirements & Constraints

- **REQ-001**: The Server must be implemented using .NET (>= 9.0) Web API using Kestrel.
- **REQ-002**: Before starting to listen, the process must print exactly `PORT_READY:[port]` (without quotes) to stdout, where [port] is a decimal TCP port number.
- **REQ-003**: The server must bind only to localhost (127.0.0.1) for security.
- **REQ-004**: Provide a sample API endpoint `GET /api/status` that returns JSON with fields { "status": "ok", "port": [port] }.
- **SEC-001**: Do not open firewall rules or listen on 0.0.0.0. Use localhost only.
- **CON-001**: The plan must create files under `Server/` in the repository root.
- **GUD-001**: Keep code minimal and synchronous at startup to ensure the port message appears before other logs.
- **PAT-001**: Use a deterministic port-finding function that binds a temporary socket to port 0 or checks availability using TCPListener.

## 2. Implementation Steps

### Implementation Phase 1

- GOAL-001: Create project skeleton and repository layout for the Server component.

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-001 | Create `Server/` folder and initialize a minimal .NET Web API project `Server/Server.csproj`. Files: `Server/Program.cs`, `Server/Controllers/StatusController.cs` | | |
| TASK-002 | Add a minimal README: `Server/README.md` describing how to build and run the backend and the expected stdout message. | | |

Completion criteria: `dotnet build` in `Server/` completes with no errors.

### Implementation Phase 2

- GOAL-002: Implement dynamic port discovery, Kestrel configuration, and stdout port announcement.

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-003 | Implement function `int FindAvailablePort()` in `Server/Program.cs` at file line 1..200 that deterministically returns an available TCP port on localhost. Implementation must: create a TcpListener(127.0.0.1, 0), start it, read the assigned port, stop it, and return the port. | | |
| TASK-004 | Configure Kestrel to listen on `127.0.0.1:[port]` by using `builder.WebHost.ConfigureKestrel(options => options.Listen(IPAddress.Loopback, port));` in `Server/Program.cs`. Ensure the call occurs after `FindAvailablePort()` and before `host.Run()`. | | |
| TASK-005 | Before calling `host.Run()`, write to stdout exactly `PORT_READY:[port]\n` using `Console.Out.WriteLine($"PORT_READY:{port}");` and flush stdout. Place this on the line immediately before `host.Run()`. | | |

Completion criteria: Running the compiled executable `Server/bin/Debug/net*/Server.exe` prints `PORT_READY:[port]` and then serves HTTP on localhost:[port]. Automated validation: a local HTTP GET to `http://localhost:[port]/api/status` returns 200 with JSON containing the port.

### Implementation Phase 3

- GOAL-003: Implement a sample API endpoint and verify startup order.

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-006 | Create `Server/Controllers/StatusController.cs`. Implement `public class StatusController : ControllerBase` and endpoint `[HttpGet("api/status")] public IActionResult Get()` that returns `new { status = "ok", port = port }`. The controller must access the configured port via an injected IOptions or static property `Server.Port` (explicitly declared). | | |
| TASK-007 | Add minimal logging to confirm requests, but keep stdout ordering deterministic so the `PORT_READY:` line is the first non-trace output. | | |

Completion criteria: `GET /api/status` returns JSON { "status":"ok", "port": <number> } and the port value matches the `PORT_READY:` message.

## 3. Alternatives

- **ALT-001**: Use environment variable PORT if provided by callers. Rejected because requirement states Server must choose an available port and announce it.
- **ALT-002**: Use `Socket` probe instead of `TcpListener`. Both are viable; `TcpListener` is chosen for simplicity and deterministic behavior.

## 4. Dependencies

- **DEP-001**: .NET SDK 9.0 or higher installed on developer machine.
- **DEP-002**: No external NuGet packages required beyond Microsoft.AspNetCore.App.

## 5. Files

- **FILE-001**: `Server/Program.cs` — Main program; contains `FindAvailablePort()` and Kestrel configuration.
- **FILE-002**: `Server/Controllers/StatusController.cs` — Sample controller with `GET /api/status`.
- **FILE-003**: `Server/Server.csproj` — Project file.
- **FILE-004**: `Server/README.md` — Build/run notes and expected stdout protocol.

## 6. Testing

- **TEST-001**: Unit test for `FindAvailablePort()` (optional): verify return is an integer between 1024 and 65535 and that binding to that port succeeds.
- **TEST-002**: Integration test script `Server/tests/run_and_query.ps1` (PowerShell) that: builds the project, runs the produced executable, reads stdout until `PORT_READY:(\d+)` is observed, performs `Invoke-WebRequest http://localhost:[port]/api/status`, and asserts HTTP 200 and JSON body contains correct port. Exit code 0 on success. Place file under `Server/tests/`.

## 7. Risks & Assumptions

- **RISK-001**: Race condition between announcing the port and Kestrel actually listening. Mitigation: obtain port via `TcpListener(0)` and then configure Kestrel to listen on the same numeric port before calling `host.Run()`; this minimizes race.
- **ASSUMPTION-001**: The host OS (Windows dev environment) allows binding to ephemeral ports and the .NET runtime is present.

## 8. Related Specifications / Further Reading

- https://docs.microsoft.com/aspnet/core/fundamentals/servers/kestrel
- https://docs.microsoft.com/dotnet/api/system.net.sockets.tcplistener
