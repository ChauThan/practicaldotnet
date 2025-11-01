# Integration Plan - Server and Client

## Overview
Integrate the Electron Client and .NET Core Server into a complete working monorepo application where:
- Client spawns and communicates with Server
- Both components work together seamlessly
- Application can be built, tested, and deployed as a unit

## Architecture Overview
```
electron-net/ (monorepo root)
├── Client/
│   ├── main.js
│   ├── renderer.js
│   ├── preload.js
│   ├── index.html
│   ├── package.json
│   └── node_modules/
├── Server/
│   ├── Program.cs
│   ├── Controllers/StatusController.cs
│   ├── Server.csproj
│   └── bin/Backend.exe (after build)
├── package.json (root - optional)
└── README.md
```

## Integration Steps

### Phase 1: Build Pipeline Setup

#### 1.1 Create Root Build Scripts
**File:** Root `package.json` (optional but recommended)

**Purpose:** Coordinate builds for both Client and Server

```json
{
  "name": "electron-net-app",
  "version": "1.0.0",
  "description": "Electron Desktop App with .NET Core Backend",
  "scripts": {
    "build": "npm run build:server && npm run build:client",
    "build:server": "cd Server && dotnet publish -c Release -o bin --self-contained -r win-x64",
    "build:client": "cd Client && npm install",
    "dev": "npm run build:server && cd Client && npm start",
    "clean": "cd Server && dotnet clean && cd ../Client && rimraf node_modules"
  }
}
```

**Alternative:** Create PowerShell build scripts for Windows environment
**File:** `build.ps1`

```powershell
# Build Server
Write-Host "Building Server..."
cd Server
dotnet publish -c Release -o bin --self-contained -r win-x64
cd ..

# Setup Client
Write-Host "Setting up Client..."
cd Client
npm install
cd ..

Write-Host "Build complete!"
```

#### 1.2 Server Build Configuration
**File:** `Server/Server.csproj`

**Key Settings:**
- Output type: Exe
- Runtime identifier: win-x64
- Self-contained: true
- Output path: bin/
- Assembly name: Backend

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
    <SelfContained>true</SelfContained>
    <AssemblyName>Backend</AssemblyName>
    <OutputType>Exe</OutputType>
  </PropertyGroup>
</Project>
```

#### 1.3 Client Dependencies
**File:** `Client/package.json`

**Key Dependencies:**
- Electron: Desktop framework
- axios: HTTP client
- dotenv: Environment configuration (optional)

### Phase 2: Communication Protocol

#### 2.1 Port Discovery Handshake
**Sequence:**
```
1. Client spawns Server process
2. Server discovers available port
3. Server prints: PORT_READY:[PORT] to stdout
4. Client regex extracts port from stdout
5. Client stores port in apiPort variable
6. Client proceeds with API calls
```

**Timing Considerations:**
- Allow 2-3 second buffer for Server startup
- Implement retry logic if port detection fails
- Timeout after 10 seconds if no port reported

#### 2.2 API Contract
**Endpoint:** `GET /api/status`

**Request:**
```
GET http://localhost:[DYNAMIC_PORT]/api/status
Accept: application/json
```

**Response (200 OK):**
```json
{
  "status": "Running",
  "port": 5021,
  "timestamp": "2025-11-01T12:30:45.123Z"
}
```

**Error Handling:**
- Connection refused: Backend not started
- Timeout (>5s): Backend slower than expected
- Invalid JSON: Backend crashed or wrong version

#### 2.3 Client IPC Flow (Optional Enhancement)
**File:** `Client/main.js`

**Purpose:** Send backend status to renderer process

```javascript
ipcMain.handle('get-backend-status', async () => {
  if (!apiPort) return { status: 'disconnected', port: null };
  
  try {
    const response = await axios.get(`http://localhost:${apiPort}/api/status`);
    return { status: 'connected', port: apiPort, ...response.data };
  } catch {
    return { status: 'error', port: apiPort };
  }
});
```

### Phase 3: File Structure Integration

#### 3.1 Backend Executable Path
**In Client/main.js:**

```javascript
// Relative path from Client's perspective
const backendExePath = path.join(__dirname, '../Server/bin/Backend.exe');
```

**Path Resolution:**
- `__dirname`: Client directory
- `../Server/bin/`: Navigate to Server bin folder
- `Backend.exe`: Target executable

**Verification:**
```javascript
if (!fs.existsSync(backendExePath)) {
  throw new Error(`Backend executable not found at: ${backendExePath}`);
}
```

#### 3.2 Resource Organization
```
electron-net/
├── .gitignore           (ignore bin/, obj/, node_modules/)
├── README.md            (project documentation)
├── BUILD.md             (build instructions)
├── Client/
│   └── .env.example     (sample environment config)
├── Server/
│   └── appsettings.json (server config)
└── docs/                (architecture documentation)
```

### Phase 4: Error Handling & Recovery

#### 4.1 Backend Crash Handling
**In Client/main.js:**

```javascript
backendProcess.on('close', (code) => {
  if (code !== 0) {
    console.error(`Backend crashed with code: ${code}`);
    // Notify renderer
    mainWindow.webContents.send('backend-crash', code);
    // Attempt restart or graceful shutdown
    mainWindow.close();
  }
});
```

#### 4.2 API Call Retry Logic
**In Client/main.js:**

```javascript
async function makeApiCallWithRetry(maxRetries = 3) {
  for (let i = 0; i < maxRetries; i++) {
    try {
      const response = await axios.get(
        `http://localhost:${apiPort}/api/status`,
        { timeout: 5000 }
      );
      return response.data;
    } catch (error) {
      if (i < maxRetries - 1) {
        await new Promise(resolve => setTimeout(resolve, 1000));
      } else {
        throw error;
      }
    }
  }
}
```

#### 4.3 Cleanup on Exit
**In Client/main.js:**

```javascript
process.on('exit', () => {
  if (backendProcess && !backendProcess.killed) {
    backendProcess.kill();
  }
});

app.on('window-all-closed', () => {
  if (backendProcess && !backendProcess.killed) {
    backendProcess.kill();
  }
  app.quit();
});
```

### Phase 5: Testing Integration

#### 5.1 Unit Tests
**Server Tests:**
- Port discovery function returns valid port
- StatusController returns correct JSON structure
- Port number matches what was discovered

**Client Tests:**
- Port regex correctly extracts port from string
- API call succeeds with mocked response
- Error handling works for connection failures

#### 5.2 Integration Tests
**E2E Workflow:**
1. Start application
2. Verify Backend process spawned
3. Verify PORT_READY message appears
4. Verify port extracted correctly
5. Verify API call succeeds
6. Verify response data matches expected format
7. Verify UI updates
8. Verify graceful shutdown

#### 5.3 Manual Testing Checklist
- [ ] Run `npm run build` succeeds
- [ ] Backend executable exists in `Server/bin/Backend.exe`
- [ ] Client app starts with `npm start`
- [ ] Console shows `PORT_READY:[PORT]`
- [ ] API call succeeds and returns data
- [ ] Port in response matches extracted port
- [ ] Closing app kills backend process
- [ ] Can restart app without port conflicts

### Phase 6: Deployment & Distribution

#### 6.1 Build for Distribution
**File:** Build script for final package

```powershell
# Clean previous builds
Remove-Item -Path "Client/dist" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "Server/bin" -Recurse -Force

# Build Server
cd Server
dotnet publish -c Release -o bin --self-contained -r win-x64 -p:PublishSingleFile=true
cd ..

# Package Client with Electron
cd Client
npm run build
cd ..

Write-Host "Distribution build complete"
```

#### 6.2 Executable Packaging
- Use `electron-builder` for creating installer
- Include Backend.exe in package
- Set correct permissions and dependencies

### Phase 7: Documentation

#### 7.1 README Structure
**File:** `README.md`

Sections:
- Project overview
- Architecture diagram
- Prerequisites (.NET SDK, Node.js versions)
- Installation steps
- Build instructions
- Running the app
- Troubleshooting
- Development guide

#### 7.2 Architecture Documentation
**File:** `docs/ARCHITECTURE.md`

Sections:
- Monorepo structure
- Communication protocol
- Port discovery mechanism
- API contract
- Error handling strategy

## Integration Checklist

### Pre-Integration
- [ ] Server builds successfully to `bin/Backend.exe`
- [ ] Server prints `PORT_READY:[PORT]` to stdout
- [ ] Server `/api/status` endpoint works correctly
- [ ] Client Electron app initializes without errors
- [ ] Client has spawn and axios dependencies

### During Integration
- [ ] Backend path correctly resolves in Client
- [ ] Port regex pattern works with sample strings
- [ ] API calls use dynamic port correctly
- [ ] Error handling prevents crashes

### Post-Integration
- [ ] Full build pipeline works end-to-end
- [ ] E2E test passes: spawn → port → API → response
- [ ] Graceful shutdown implemented
- [ ] No file permission issues
- [ ] Documentation complete and accurate

## Success Criteria
- ✅ Single command (`npm run build`) builds entire project
- ✅ Client successfully spawns Server
- ✅ Port discovery works reliably
- ✅ API communication succeeds
- ✅ Graceful error handling
- ✅ Clean startup and shutdown
- ✅ Ready for packaging and distribution

## Timeline Estimate
1. **Phase 1 (Build Pipeline):** 1-2 hours
2. **Phase 2 (Communication):** 2-3 hours
3. **Phase 3 (File Structure):** 1 hour
4. **Phase 4 (Error Handling):** 2 hours
5. **Phase 5 (Testing):** 2-3 hours
6. **Phase 6-7 (Docs & Polish):** 2 hours

**Total:** ~12-14 hours for complete integration
