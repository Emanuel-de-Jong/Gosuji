$installDir = Read-Host 'Enter installation directory to place Gosuji directory in)'
$targetDir = Join-Path $installDir 'Gosuji'
if (-not (Test-Path $targetDir)) {
    New-Item -Path $targetDir -ItemType Directory | Out-Null
}

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Get-ChildItem -Path $scriptDir -Recurse | Where-Object {
    $_.FullName -ne $MyInvocation.MyCommand.Definition
} | ForEach-Object {
    $dest = $_.FullName.Replace($scriptDir, $targetDir)
    if ($_.PSIsContainer) {
        if (-not (Test-Path $dest)) {
            New-Item -ItemType Directory -Path $dest | Out-Null
        }
    } else {
        Copy-Item $_.FullName -Destination $dest -Force
    }
}

$exePath = Join-Path $targetDir 'Gosuji.exe'
$startMenu = Join-Path "$env:APPDATA\Microsoft\Windows\Start Menu\Programs" 'Gosuji.lnk'
$desktop = Join-Path ([Environment]::GetFolderPath('Desktop')) 'Gosuji.lnk'
$shell = New-Object -ComObject WScript.Shell
foreach ($shortcut in @($startMenu, $desktop)) {
    $s = $shell.CreateShortcut($shortcut)
    $s.TargetPath = $exePath
    $s.Save()
}

Write-Host 'Installation complete.'
Pause
