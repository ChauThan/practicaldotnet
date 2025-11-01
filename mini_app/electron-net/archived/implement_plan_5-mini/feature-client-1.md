---
goal: "Feature: Setup Client (Electron frontend that launches backend)"
version: 1.0
date_created: 2025-11-01
last_updated: 2025-11-01
owner: Team/Dev
status: 'Planned'
tags: ["feature","client","electron","node","spawn","stdout"]
---

# Introduction

![Status: Planned](https://img.shields.io/badge/status-Planned-blue)

This plan describes deterministic, step-by-step tasks to create the Client component using Electron (Node.js). The Electron main process will spawn the .NET backend executable (`Server/bin/Backend.exe`) using `child_process.spawn`, listen to its stdout for the `PORT_READY:(\d+)` message, extract the port, and perform a sample HTTP GET `GET /api/status` to the backend. The renderer will be a minimal HTML page confirming the UI is loaded.

## 1. Requirements & Constraints

- **REQ-001**: Client uses Electron (v14+ recommended) and Node.js. JavaScript or TypeScript acceptable; this plan uses JavaScript for simplicity.
- **REQ-002**: The main process must spawn the backend executable path `Server/bin/Backend.exe` (or `Server/bin/Debug/net*/Server.exe` during development). Use an absolute or reliably relative path `../Server/bin/Backend.exe` referenced from `Client/`.
- **REQ-003**: Parse backend stdout for regex `PORT_READY:(\d+)` and extract the port number.
- **REQ-004**: After extracting port, perform `GET http://localhost:[port]/api/status` and print response JSON to Electron console (main process logger).
- **CON-001**: Client must not assume a fixed port; must wait for the `PORT_READY` message.

## 2. Implementation Steps

### Implementation Phase 1

- GOAL-001: Create Electron project skeleton under `Client/`.

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-001 | Create `Client/package.json` with `main: "main.js"`, dependencies `electron` and optionally `node-fetch` (or use built-in `http`/`https`), and scripts: `start` that runs `electron .` | | |
| TASK-002 | Create `Client/main.js` (Electron main process), `Client/index.html`, `Client/renderer.js`. | | |

Completion criteria: `npm install` in `Client/` runs and `npm run start` launches Electron with the sample window.

### Implementation Phase 2

- GOAL-002: Implement main process logic to spawn backend, parse stdout, and call the backend API.

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-003 | In `Client/main.js`, implement `const { spawn } = require('child_process');` and spawn the backend binary: `const child = spawn(pathToBackend, [], { stdio: ['ignore', 'pipe', 'pipe'] });` where `pathToBackend` is `path.join(__dirname, '..', 'Server', 'bin', 'Backend.exe')` or development path; include fallback to `Server/bin/Debug/net6.0/Server.exe`. | | |
| TASK-004 | Listen on `child.stdout` for data chunks. Buffer data until newline, run regex `/PORT_READY:(\d+)/` against the chunk (or full buffer) and extract port string. Implement robust parsing that handles partial lines and multiple messages across chunks. Implement timeout of configurable 10 seconds (REQ-005). | | |
| TASK-005 | After extracting port, perform HTTP GET to `http://localhost:[port]/api/status` using `node-fetch` or `http.get`. Log the response body to the main process console using `console.log`. Save the port to an in-memory variable `backendPort`. | | |
| TASK-006 | Forward backend stdout/stderr lines to the Electron main process console verbatim (prefix with `[backend] `) for debugging. Also handle child `exit`/`error` events and show user-friendly logs. | | |

Completion criteria: Running Electron triggers the backend spawn; main process prints `PORT_READY:[port]` detection and logs JSON response from `/api/status`.

### Implementation Phase 3

- GOAL-003: Implement renderer UI and graceful shutdown.

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-007 | `Client/index.html` shows a minimal page: heading "Electron Client Loaded" and a placeholder where the backend port and status JSON (from main) can be displayed. Use IPC or preload to display the detected port. File: `Client/renderer.js` with code to receive port via `ipcRenderer.invoke('get-backend-port')` (expose via `ipcMain.handle('get-backend-port', ...)`). | | |
| TASK-008 | Implement graceful shutdown: when app quits, ensure child process is killed (use `child.kill()` on `app.on('before-quit')`). | | |

Completion criteria: Renderer shows "Electron Client Loaded", main logs backend status, and quitting Electron stops the backend process.

## 3. Alternatives

- **ALT-001**: Use a Node native module to manage process lifecycle. Rejected for simplicity.
- **ALT-002**: Use a fixed port via environment variable. Rejected due to dynamic-port requirement.

## 4. Dependencies

- **DEP-001**: Node.js installed (recommend v16+).
- **DEP-002**: Electron as a dev dependency.
- **DEP-003**: Optional `node-fetch` for HTTP calls, or use native `http`/`https`.

## 5. Files

- **FILE-001**: `Client/package.json` — scripts and dependencies.
- **FILE-002**: `Client/main.js` — Electron main process that spawns backend and parses stdout.
- **FILE-003**: `Client/preload.js` — safe bridge for renderer to request backend port (if enabling contextIsolation).
- **FILE-004**: `Client/index.html` and `Client/renderer.js` — UI to confirm load and display backend status.

## 6. Testing

- **TEST-001**: Manual e2e: Build the Server, then `cd Client && npm install && npm start`, verify main prints `PORT_READY` detection and logs backend status JSON.
- **TEST-002**: Automated `Client/tests/e2e.ps1` (PowerShell): build server, start electron with headless or capture console output, detect `PORT_READY`, assert `GET /api/status` returns expected JSON. Use `Start-Process -PassThru` and `Wait-Process` with timeout.

## 7. Risks & Assumptions

- **RISK-001**: Backend path may differ between development and packaged modes. Mitigation: main.js should attempt a prioritized list of candidate executable paths and log errors if not found.
- **ASSUMPTION-001**: Backend prints the `PORT_READY:` line early and reliably. Client enforces a 10s timeout and reports failure if not observed.

## 8. Related Specifications / Further Reading

- https://www.electronjs.org/docs/latest/tutorial/quick-start
- https://nodejs.org/api/child_process.html#child_process_child_process_spawn_command_args_options
