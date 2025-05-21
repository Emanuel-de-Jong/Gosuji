const path = require('path');
const { app, BrowserWindow } = require('electron');
const { spawn } = require('child_process');
const express = require('express');

const CLIENT_PORT = 5001;

function sleep(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

function getResourcePath() {
  return app.isPackaged 
    ? path.join(process.resourcesPath, "app")
    : __dirname;
}

function createWindow() {
  const window = new BrowserWindow({
    webPreferences: {
      nodeIntegration: false,
      contextIsolation: true,
      webSecurity: false
    }
  });

  window.maximize();
  window.loadURL(`http://localhost:${CLIENT_PORT}`);

  return window;
}

let apiProcess, clientApp, clientServer;

async function start() {
  const resourcePath = getResourcePath();
  const clientPath = path.join(resourcePath, 'client');
  const apiPath = path.join(resourcePath, 'api');

  apiProcess = spawn(path.join(apiPath, 'Gosuji.API.exe'), [], {
    cwd: apiPath
  });

  apiProcess.stdout.on('data', (data) => console.log(`API: ${data}`));
  apiProcess.stderr.on('data', (data) => console.error(`API ERROR: ${data}`));

  clientApp = express();
  clientApp.use(express.static(clientPath));
  clientServer = clientApp.listen(CLIENT_PORT, () => {
    console.log(`Client served at http://localhost:${CLIENT_PORT}`);
  });

  // Wait for API and client to be ready
  await sleep(1000);

  app.whenReady().then(createWindow);
}

start();

app.on('window-all-closed', async () => {
  if (process.platform !== 'darwin') {
    clientServer.close();

    // Wait for API to save potential game
    await sleep(1500);
    apiProcess.kill('SIGTERM');

    app.quit();
  }
});
