const { ipcRenderer } = require('electron');

const statusDiv = document.getElementById('status');

ipcRenderer.on('server-status', (event, data) => {
    console.log('Received server status:', data);
    if (data.status === 'error') {
        statusDiv.className = 'status error';
        statusDiv.textContent = data.message || `Server connection failed`;
    } else {
        try {
            const statusObj = JSON.parse(data.status);
            statusDiv.className = 'status success';
            statusDiv.textContent = `Server connected on port ${data.port}. Status: ${statusObj.status}`;
        } catch (e) {
            statusDiv.className = 'status success';
            statusDiv.textContent = `Server connected on port ${data.port}`;
        }
    }
});