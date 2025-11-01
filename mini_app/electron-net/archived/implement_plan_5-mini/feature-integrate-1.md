---
goal: "Feature: Integrate Server and Client (spawn + stdout port protocol)"
version: 1.0
date_created: 2025-11-01
last_updated: 2025-11-01
owner: Team/Dev
status: 'Planned'
tags: ["feature","integration","electron","dotnet","e2e"]
---

# Introduction

![Status: Planned](https://img.shields.io/badge/status-Planned-blue)

This plan defines deterministic integration steps to wire the Electron Client and .NET Server together in a monorepo. It covers runtime launching, stdout protocol (`PORT_READY:[port]`), path resolution for development and packaged modes, robust parsing, error handling, packaging considerations, and automated e2e verification.

## 1. Requirements & Constraints

- **REQ-001**: Client must be able to launch Server executable located at `Server/bin/Backend.exe` (primary), or `Server/bin/Debug/net6.0/Server.exe` (dev fallback). The integration tasks must specify these paths explicitly.
- **REQ-002**: The only allowed protocol for port discovery is the stdout message `PORT_READY:(\d+)`.
- **REQ-003**: Integration must include e2e test script `scripts/run_e2e.ps1` that performs an automated run and verification.
- **CON-001**: Integration tasks must produce deterministic, machine-parseable checks and exit codes for CI use.

## 2. Implementation Steps

### Implementation Phase 1

- GOAL-001: Define and implement the spawn contract in `Client/main.js`.

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-001 | In `Client/main.js` add `const backendCandidates = [ path.join(__dirname, '..', 'Server', 'bin', 'Backend.exe'), path.join(__dirname, '..', 'Server', 'bin', 'Debug', 'net6.0', 'Server.exe') ];` Attempt candidates in order and log the chosen path. | | |
| TASK-002 | Ensure spawn uses `{ stdio: ['ignore','pipe','pipe'] }` and binary is executed with no shell to avoid quoting differences. | | |

Completion criteria: `Client/main.js` deterministically chooses a candidate path and spawns the binary without shell.

### Implementation Phase 2

- GOAL-002: Robust stdout parsing and retries.

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-003 | Implement a `StdoutPortParser` function/class in `Client/main.js` that buffers `child.stdout` chunks and applies regex `/PORT_READY:(\d+)/`. The parser must handle partial lines, multiple matches, and emit a single `port` event. | | |
| TASK-004 | Implement a timeout (configurable 10000 ms) that kills child and fails with a non-zero exit code if `PORT_READY` not received. Also implement a health-check retry policy: if HTTP GET to `/api/status` fails, attempt up to 3 attempts spaced 500ms apart before failing. | | |

Completion criteria: Parser emits the port exactly once and retries when initial fetch fails.

### Implementation Phase 3

- GOAL-003: Packaging and path resolution for production builds.

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-005 | Document packaging expectations in `README.md` for packaging artifacts (e.g., when packaged, place Server binary in `resources/app/Server/Backend.exe` and adjust `backendCandidates` accordingly). Provide explicit relative runtime paths. | | |
| TASK-006 | Add an environment variable fallback `process.env.BACKEND_PATH` so packagers can explicitly set path to backend binary. Document use. | | |

Completion criteria: README contains clear instructions and client code respects `BACKEND_PATH` when set.

### Implementation Phase 4

- GOAL-004: Automated E2E verification script and CI-friendly output.

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-007 | Add `scripts/run_e2e.ps1` at repo root that performs: build Server (`dotnet build Server/`), start Client in headless mode or capture console output (`Start-Process -FilePath 'node' ...` or `electron .` capturing stdout), wait for `PORT_READY`, perform `Invoke-WebRequest` to `/api/status`, assert JSON, kill processes, and exit 0 on success or non-zero on fail. | | |
| TASK-008 | Add `package.json` script in root or Client: `e2e` that invokes `powershell -File ../scripts/run_e2e.ps1` for Windows. | | |

Completion criteria: `scripts/run_e2e.ps1` returns exit code 0 when integration is healthy and non-zero otherwise. Output should include explicit lines: `[e2e] PASS` or `[e2e] FAIL: <reason>`.

## 3. Alternatives

- **ALT-001**: Use a named-pipe or domain socket instead of stdout protocol. Rejected to keep protocol simple and cross-platform via stdout.
- **ALT-002**: Use a file-based handshake. Rejected because stdout is faster and avoids file cleanup.

## 4. Dependencies

- **DEP-001**: Electron and Node.js for Client testing.
- **DEP-002**: .NET SDK for building Server.

## 5. Files

- **FILE-001**: `Client/main.js` — spawn logic and parser.
- **FILE-002**: `Client/package.json` — start/e2e scripts.
- **FILE-003**: `scripts/run_e2e.ps1` — automated integration test harness.
- **FILE-004**: `README.md` (repo root) — integration/run instructions.

## 6. Testing

- **TEST-001**: CI job that runs `dotnet build Server/`, `npm ci` in `Client/`, and `powershell -File scripts/run_e2e.ps1`. Job passes on exit code 0.
- **TEST-002**: Local manual test steps provided in README for troubleshooting.

## 7. Risks & Assumptions

- **RISK-001**: Path differences between development (unpacked exe) and packaged (ASAR/resources) cause file-not-found. Mitigation: support env var `BACKEND_PATH` and multiple candidate paths.
- **ASSUMPTION-001**: Packaging system will include the Server binary in the packaged electron app under a known relative path.

## 8. Related Specifications / Further Reading

- `Client` plan: `/plan/feature-client-1.md`
- `Server` plan: `/plan/feature-server-1.md`
