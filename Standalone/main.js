const path = require('path');
const { app, BrowserWindow } = require('electron');
const { execFile } = require('child_process');
const express = require('express');

const CLIENT_PATH = 'client';
const API_PATH = 'api';
const PORT = 5001;

const exePath = path.join(__dirname, API_PATH, 'Gosuji.API.exe');
execFile(exePath, [], { cwd: API_PATH }, (error, stdout, stderr) => {
    if (error) {
        console.error(`Error starting the executable: ${error.message}`);
        return;
    }
    if (stderr) {
        console.error(`stderr: ${stderr}`);
    }
    console.log(`stdout: ${stdout}`);
});

const expressApp = express();
expressApp.use(express.static(path.join(__dirname, CLIENT_PATH)));

let clientServer;
let mainWindow;

const createWindow = () => {
    mainWindow = new BrowserWindow({
        width: 800,
        height: 600,
        webPreferences: {
            nodeIntegration: true,
            contextIsolation: false,
        },
    });

    mainWindow.loadURL(`http://localhost:${PORT}`);

    mainWindow.on('closed', () => {
        mainWindow = null;
    });
};

app.whenReady().then(() => {
    clientServer = expressApp.listen(PORT, () => {
        console.log(`Client server running at http://localhost:${PORT}`);

        createWindow();
    });

    app.on('activate', () => {
        if (BrowserWindow.getAllWindows().length === 0) {
            createWindow();
        }
    });
});

app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') {
        app.quit();
    }

    if (clientServer) {
        clientServer.close(() => {
            console.log('Client server closed');
        });
    }
});
