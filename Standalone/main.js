const path = require('path');
const { app, BrowserWindow } = require('electron');
const { spawn } = require('child_process');
const express = require('express');

const CLIENT_PATH = app.isPackaged
  ? path.join(process.resourcesPath, 'client')
  : path.join(__dirname, 'client');

const API_EXE_PATH = app.isPackaged
  ? path.join(process.resourcesPath, 'api', 'Gosuji.API.exe')
  : path.join(__dirname, 'api', 'Gosuji.API.exe');

const CLIENT_PORT = 5001;

const apiProcess = spawn(API_EXE_PATH, [], {
  cwd: path.dirname(API_EXE_PATH)
});

apiProcess.stdout.on('data', (data) => console.log(`API: ${data}`));
apiProcess.stderr.on('data', (data) => console.error(`API ERROR: ${data}`));

const clientApp = express();
clientApp.use(express.static(CLIENT_PATH));
const clientServer = clientApp.listen(CLIENT_PORT, () => {
  console.log(`Blazor client served at http://localhost:${CLIENT_PORT}`);
});

function createWindow() {
  const win = new BrowserWindow({
    webPreferences: {
      nodeIntegration: false,
      contextIsolation: true,
      webSecurity: false
    }
  });

  win.maximize();
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
