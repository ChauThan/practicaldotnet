const { app, BrowserWindow } = require('electron');
const { spawn } = require('child_process');
const http = require('http');
const path = require('path');

let mainWindow;
let serverProcess;

function createWindow() {
    mainWindow = new BrowserWindow({
        width: 800,
        height: 600,
        webPreferences: {
            nodeIntegration: true,
            contextIsolation: false
        }
    });

    mainWindow.loadFile('index.html');
}

function startServer(retryCount = 0) {
    const maxRetries = 3;

    let serverPath;
    let serverArgs = [];
    let serverCwd;

    if (app.isPackaged) {
        // In packaged app, server executable is in resources
        serverPath = path.join(process.resourcesPath, 'Backend.exe');
        serverCwd = path.dirname(serverPath);
    } else {
        // In development, use dotnet run
        serverPath = 'dotnet';
        serverArgs = ['run'];
        serverCwd = path.join(__dirname, '../Server');
    }

    console.log('Starting server (attempt', retryCount + 1, '):', serverPath, serverArgs, 'cwd:', serverCwd);
    serverProcess = spawn(serverPath, serverArgs, {
        cwd: serverCwd,
        stdio: ['pipe', 'pipe', 'pipe']
    });

    let portDetected = false;
    const portTimeout = setTimeout(() => {
        if (!portDetected) {
            console.error('Timeout waiting for server port');
            serverProcess.kill();
            if (retryCount < maxRetries) {
                console.log('Retrying server start...');
                setTimeout(() => startServer(retryCount + 1), 2000);
            } else {
                if (mainWindow) {
                    mainWindow.webContents.send('server-status', { status: 'error', message: 'Failed to start server after multiple attempts' });
                }
            }
        }
    }, 10000); // 10 second timeout

    serverProcess.stdout.on('data', (data) => {
        const output = data.toString();
        console.log('Server output:', output);

        const portMatch = output.match(/PORT_READY:(\d+)/);
        if (portMatch) {
            portDetected = true;
            clearTimeout(portTimeout);
            const port = portMatch[1];
            console.log('Detected port:', port);
            setTimeout(() => checkServerStatus(port), 1000);
        }
    });

    serverProcess.stderr.on('data', (data) => {
        console.error('Server error:', data.toString());
    });

    serverProcess.on('close', (code) => {
        console.log('Server process exited with code', code);
        if (code !== 0 && !portDetected) {
            if (retryCount < maxRetries) {
                console.log('Server exited unexpectedly, retrying...');
                setTimeout(() => startServer(retryCount + 1), 2000);
            } else {
                if (mainWindow) {
                    mainWindow.webContents.send('server-status', { status: 'error', message: `Server exited with code ${code}` });
                }
            }
        }
    });

    serverProcess.on('error', (err) => {
        console.error('Failed to start server:', err);
        if (retryCount < maxRetries) {
            console.log('Failed to spawn server, retrying...');
            setTimeout(() => startServer(retryCount + 1), 2000);
        } else {
            if (mainWindow) {
                mainWindow.webContents.send('server-status', { status: 'error', message: `Failed to spawn server: ${err.message}` });
            }
        }
    });
}

function checkServerStatus(port) {
    const options = {
        hostname: 'localhost',
        port: port,
        path: '/api/status',
        method: 'GET'
    };

    const req = http.request(options, (res) => {
        let data = '';
        res.on('data', (chunk) => {
            data += chunk;
        });
        res.on('end', () => {
            console.log('Server status response:', data);
            if (mainWindow) {
                mainWindow.webContents.send('server-status', { port, status: data });
            }
        });
    });

    req.on('error', (err) => {
        console.error('Error checking server status:', err.message);
        if (mainWindow) {
            mainWindow.webContents.send('server-status', { port, status: 'error' });
        }
    });

    req.end();
}

app.whenReady().then(() => {
    createWindow();
    startServer();

    app.on('activate', () => {
        if (BrowserWindow.getAllWindows().length === 0) {
            createWindow();
        }
    });
});

app.on('window-all-closed', () => {
    if (serverProcess) {
        serverProcess.kill();
    }
    if (process.platform !== 'darwin') {
        app.quit();
    }
});