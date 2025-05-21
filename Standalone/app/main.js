const path = require('path');
const { app, BrowserWindow } = require('electron');
const { spawn } = require('child_process');
const express = require('express');
const fs = require('fs');

const CLIENT_PORT = 5001;

function sleep(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

function getResourcePath() {
  return app.isPackaged 
    ? path.join(process.resourcesPath, "app")
    : __dirname;
}

const resourcePath = getResourcePath();
const clientPath = path.join(resourcePath, 'client');
const apiPath = path.join(resourcePath, 'api');
const logPath = 'error.log';

const logStream = fs.createWriteStream(logPath, { flags: 'a' });
function logToFile(...args) {
  const message = args.map(arg => (typeof arg === 'string' ? arg : JSON.stringify(arg))).join(' ');
  const timestamp = new Date().toISOString();
  logStream.write(`[${timestamp}] ${message}\n`);
}

['log', 'error', 'warn', 'info'].forEach(method => {
  const orig = console[method];
  console[method] = (...args) => {
    logToFile(...args);
    orig.apply(console, args);
  };
});

app.on('before-quit', (e) => {
  console.log('Closing...');
});

let clientApp;
function setupClientApp() {
  clientApp = express();
  clientApp.use(express.static(clientPath));

  clientApp.use((req, res, next) => {
    if (req.method === 'GET' && !req.path.startsWith('/api')) {
      res.sendFile(path.join(clientPath, 'index.html'));
    } else {
      next();
    }
  });
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
}

let apiProcess, clientServer;
async function run() {
  apiProcess = spawn(path.join(apiPath, 'Gosuji.API.exe'), [], {
    cwd: apiPath
  });

  apiProcess.stdout.on('data', (data) => console.log(`API: ${data}`));
  apiProcess.stderr.on('data', (data) => console.error(`API ERROR: ${data}`));

  setupClientApp();

  clientServer = clientApp.listen(CLIENT_PORT, () => {
    console.log(`Client served at http://localhost:${CLIENT_PORT}`);
  });

  // Wait for API and client to be ready
  await sleep(1000);

  app.whenReady().then(createWindow);
}

run().catch(err => {
  console.error('Application failed:', err);
  app.quit();
});

app.on('window-all-closed', () => {
  if (process.platform === 'darwin') {
    return;
  }

  clientServer.close();

  // Wait for API to save potential game
  setTimeout(() => apiProcess.kill('SIGTERM'), 1500);

  apiProcess.on('close', () => {
    app.quit();
  });
});
