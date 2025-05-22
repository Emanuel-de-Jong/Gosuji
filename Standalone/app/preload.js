const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('electronAPI', {
  saveFile: (fileName, content) => {
    ipcRenderer.send('trigger-save-file', fileName, content);
  }
});
