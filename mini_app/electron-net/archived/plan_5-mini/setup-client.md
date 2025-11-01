## Setup Client — Electron App (Main + Renderer)

### Contract
- Inputs: client spawns backend executable located under `Server/bin` (prefers `Backend.exe` on Windows).
- Outputs: reads backend stdout for `PORT_READY:(\d+)`, performs a `GET /api/status` to `http://localhost:[port]` and logs the JSON result. Renderer shows a simple page confirming the app loaded.
- Success criteria: client detects the port, calls `/api/status`, prints the response JSON, and renderer loads `index.html`.

### Files to create
- `Client/package.json` — npm metadata with `start` script (electron .).
- `Client/main.js` — Electron main process: spawn backend, parse stdout, call API, and create BrowserWindow.
- `Client/index.html` — minimal renderer HTML to confirm load.
- `Client/renderer.js` — optional renderer script.
- `Client/README.md` — build/run instructions and notes about spawn path.

### Implementation notes (main.js)
- Use Node's `child_process.spawn` to start the backend executable:
  - Prefer absolute path resolution: `path.resolve(__dirname, '..', 'Server', 'bin', 'Backend.exe')`.
  - Fallbacks: if `.exe` not present, try `Backend.dll` and spawn `dotnet Backend.dll`.
- Listen to `child.stdout.on('data', chunk => { buffer += chunk.toString(); match with /PORT_READY:(\d+)/ })`.
- Implement a timeout (e.g., 20s) to fail gracefully if no port is announced.
- After extract port, call `fetch('http://localhost:'+port+'/api/status')` and log the result.
- Create a BrowserWindow and load `index.html`.

### Example behavior (pseudocode)
```
spawn backend
wait for stdout match PORT_READY:(\d+)
fetch http://localhost:port/api/status
console.log(response)
open BrowserWindow -> index.html
```

### Commands (pwsh)
- Initialize & install (once):
  ```pwsh
  cd Client
  npm init -y
  npm install --save-dev electron
  ```
- Run (development):
  ```pwsh
  npm run start
  # where package.json has: "start": "electron ."
  ```

### Edge cases & notes
- stdout chunks: data can be split across multiple `data` events. Use a buffer and run the regex against the accumulated text.
- Spawn path: ensure the client resolves the correct path in both dev and published flows. Use a small resolution function: check for exe, then dll+dotnet, then `dotnet run` development fallback.
- Child process lifecycle: forward `exit` and `error` events to the Electron console and consider shutting down the app when backend exits unexpectedly.

### Tests / Smoke checks
1. Publish or run the backend so a binary/DLL is available.
2. `npm run start` and confirm the main process console shows the port detection and `/api/status` JSON.
