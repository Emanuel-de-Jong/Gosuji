const path = require('path');
const { app, BrowserWindow } = require('electron');
const { spawn } = require('child_process');
const express = require('express');

const getResourcePath = () => {
  return app.isPackaged 
    ? path.join(process.resourcesPath, "app")
    : __dirname;
};

const resourcePath = getResourcePath();

const CLIENT_PATH = path.join(resourcePath, 'client');
const API_PATH = path.join(resourcePath, 'api');
const CLIENT_PORT = 5001;

const apiProcess = spawn(path.join(API_PATH, 'Gosuji.API.exe'), [], {
  cwd: API_PATH
});

apiProcess.stdout.on('data', (data) => console.log(`API: ${data}`));
apiProcess.stderr.on('data', (data) => console.error(`API ERROR: ${data}`));

const clientApp = express();
clientApp.use(express.static(CLIENT_PATH));
const clientServer = clientApp.listen(CLIENT_PORT, () => {
  console.log(`Client served at http://localhost:${CLIENT_PORT}`);
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
    apiProcess.kill();
    clientServer.close();
    app.quit();
  }
});
