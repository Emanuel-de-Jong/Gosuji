const path = require('path');
const { app, BrowserWindow } = require('electron/main')
const { execFile } = require('child_process');
const express = require('express');

CLIENT_PATH = 'client'
SERVER_PATH = 'server'

const exePath = path.join(__dirname, 'server', 'Gosuji.API.exe');
execFile(exePath, [], { cwd: SERVER_PATH }, (error, stdout, stderr) => {
    if (error) {
        console.error(`Error starting the executable: ${error.message}`);
        return;
    }
    if (stderr) {
        console.error(`stderr: ${stderr}`);
        return;
    }
    console.log(`stdout: ${stdout}`);
});

const expressApp = express();
expressApp.use(express.static(path.join(__dirname, CLIENT_PATH)));
const port = 7171;

const createWindow = () => {
    const win = new BrowserWindow({
        width: 800,
        height: 600,
        webPreferences: {
            nodeIntegration: true
        },
    })

    return win;
}

app.whenReady().then(() => {
    expressApp.listen(port, () => {
        console.log(`Server running at http://localhost:${port}`);

        win = createWindow()

        win.loadURL(`http://localhost:${port}/index.html`);
    });
})

app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') {
        app.quit()
    }
})
