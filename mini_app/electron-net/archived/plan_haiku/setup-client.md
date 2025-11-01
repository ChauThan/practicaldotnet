# Setup Client Plan

## Overview
Create an Electron desktop application that:
- Spawns the .NET Core backend process using `child_process`
- Parses stdout to extract the dynamic port
- Makes HTTP requests to the backend API
- Displays UI confirmation that app is loaded

## Architecture
```
Client/
├── main.js                 # Main process - spawns backend, listens to stdout
├── preload.js              # Security context for renderer
├── renderer.js             # Renderer process code
├── index.html              # UI template
├── package.json            # Dependencies and scripts
└── resources/
    └── ../Server/bin/Backend.exe  # Path to backend executable
```

## Implementation Steps

### 1. Initialize Electron Project
**File:** `Client/package.json`

**Requirements:**
- Set up Node.js project with Electron
- Install required dependencies:
  - `electron`: ^latest
  - `axios` or native fetch: for HTTP requests
  - Optional: `typescript` for type safety

**Setup Commands:**
```bash
cd Client
npm init -y
npm install --save-dev electron
npm install axios
```

**package.json Structure:**
```json
{
  "name": "electron-dotnet-app",
  "version": "1.0.0",
  "main": "main.js",
  "scripts": {
    "start": "electron .",
    "build": "electron-builder"
  },
  "devDependencies": {
    "electron": "^latest"
  },
  "dependencies": {
    "axios": "^latest"
  }
}
```

### 2. Implement Main Process with Backend Spawning
**File:** `Client/main.js`

**Requirements:**
- Import Electron and child_process modules
- Create function to spawn Backend.exe
- Listen to stdout stream
- Extract port using regex pattern `PORT_READY:(\d+)`
- Store port for API calls
- Handle process errors gracefully

**Code Structure:**
```javascript
const { app, BrowserWindow } = require('electron');
const { spawn } = require('child_process');
const path = require('path');
const axios = require('axios');

let backendProcess = null;
let apiPort = null;

function startBackend() {
  const backendPath = path.join(__dirname, '../Server/bin/Backend.exe');
  
  backendProcess = spawn(backendPath);
  
  backendProcess.stdout.on('data', (data) => {
    const output = data.toString();
    console.log(`[Backend] ${output}`);
    
    // Extract port using regex
    const portMatch = output.match(/PORT_READY:(\d+)/);
    if (portMatch) {
      apiPort = parseInt(portMatch[1], 10);
      console.log(`✓ Backend ready on port: ${apiPort}`);
      makeApiCall();
    }
  });
  
  backendProcess.stderr.on('data', (data) => {
    console.error(`[Backend Error] ${data.toString()}`);
  });
  
  backendProcess.on('close', (code) => {
    console.log(`Backend exited with code ${code}`);
  });
}

function makeApiCall() {
  if (!apiPort) return;
  
  axios.get(`http://localhost:${apiPort}/api/status`)
    .then(response => {
      console.log('✓ API Status:', response.data);
    })
    .catch(error => {
      console.error('✗ API call failed:', error.message);
    });
}
```

### 3. Create Renderer Process with UI
**File:** `Client/renderer.js`

**Requirements:**
- Simple JavaScript for renderer process
- Display status messages
- Show when backend is connected
- Optional: display API response data

**Code Pattern:**
```javascript
// Listen for messages from main process
const status = document.getElementById('status');

// Show loading state
status.textContent = 'Connecting to backend...';

// Update via IPC when backend is ready (optional enhancement)
```

### 4. Create HTML UI Template
**File:** `Client/index.html`

**Requirements:**
- Simple HTML structure
- Display app title
- Status indicator
- Link to renderer.js
- Basic styling

**Template:**
```html
<!DOCTYPE html>
<html>
<head>
  <meta charset="UTF-8">
  <title>Electron .NET App</title>
  <style>
    body {
      font-family: Arial, sans-serif;
      display: flex;
      justify-content: center;
      align-items: center;
      height: 100vh;
      margin: 0;
      background: #f0f0f0;
    }
    .container {
      text-align: center;
      background: white;
      padding: 40px;
      border-radius: 8px;
      box-shadow: 0 2px 10px rgba(0,0,0,0.1);
    }
    h1 { color: #333; }
    #status { color: #666; margin-top: 20px; }
  </style>
</head>
<body>
  <div class="container">
    <h1>Electron + .NET Core Desktop App</h1>
    <p id="status">Loading...</p>
  </div>
  <script src="renderer.js"></script>
</body>
</html>
```

### 5. Create Preload Script (Security)
**File:** `Client/preload.js`

**Requirements:**
- Minimal security context
- Optional: expose safe APIs to renderer
- Prevent direct Node.js access

**Code:**
```javascript
const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('electronAPI', {
  // Expose safe APIs here if needed
});
```

### 6. Complete Main Process Entry Point
**File:** `Client/main.js` (continuation)

**Requirements:**
- Create BrowserWindow on app ready
- Load index.html
- Start backend before creating window
- Handle app quit and cleanup

**Code Addition:**
```javascript
app.on('ready', () => {
  startBackend();
  
  const mainWindow = new BrowserWindow({
    width: 800,
    height: 600,
    webPreferences: {
      preload: path.join(__dirname, 'preload.js'),
      nodeIntegration: false,
      contextIsolation: true
    }
  });
  
  mainWindow.loadFile('index.html');
});

app.on('window-all-closed', () => {
  if (backendProcess) {
    backendProcess.kill();
  }
  app.quit();
});
```

## Development Workflow

### Setup
```bash
cd Client
npm install
```

### Run Development
```bash
npm start
```

### Debugging
- Open DevTools: Press Ctrl+Shift+I in Electron window
- Check console for backend output
- Verify API port extraction from stdout

## Testing Strategy

### Port Detection Testing
1. Run app
2. Check console for `PORT_READY:[PORT]` message
3. Verify port number is extracted correctly
4. Confirm port is numeric and valid (5000-65535)

### API Call Testing
1. After port extraction, verify API call succeeds
2. Check response contains status and port
3. Verify response displays in console

### UI Testing
1. Electron window loads successfully
2. HTML displays with styling
3. Status message shows appropriate state

## Dependencies
- Electron 15+
- Node.js 14+
- axios for HTTP requests
- Windows compatible

## Success Criteria
- ✅ Electron app starts without errors
- ✅ Backend process spawned with `spawn()`
- ✅ Port regex successfully extracts dynamic port
- ✅ HTTP GET request to `/api/status` succeeds
- ✅ Response data logged to console
- ✅ UI displays confirmation of app loaded
- ✅ Graceful shutdown of backend when app closes
