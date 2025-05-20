const path = require('path');
const { app, BrowserWindow } = require('electron');
const { spawn } = require('child_process');
const express = require('express');

const CLIENT_PATH = 'client';
const API_PATH = 'api';
const CLIENT_PORT = 5001;

const apiProcess = spawn(path.join(__dirname, API_PATH, 'Gosuji.API.exe'), [], {
    cwd: path.join(__dirname, API_PATH)
});

apiProcess.stdout.on('data', (data) => console.log(`API: ${data}`));
apiProcess.stderr.on('data', (data) => console.error(`API ERROR: ${data}`));

const clientApp = express();
clientApp.use(express.static(path.join(__dirname, CLIENT_PATH)));
const clientServer = clientApp.listen(CLIENT_PORT, () => {
    console.log(`Blazor client served at http://localhost:${CLIENT_PORT}`);
});

function createWindow() {
    const win = new BrowserWindow({
        width: 800,
        height: 600,
        webPreferences: {
            nodeIntegration: true,
            contextIsolation: false,
            webSecurity: false
        }
    });

    win.webContents.openDevTools();

    win.loadURL(`http://localhost:${CLIENT_PORT}`);
}

app.whenReady().then(createWindow);

app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') {
        clientServer.close();
        apiProcess.kill();
        app.quit();
    }
});
