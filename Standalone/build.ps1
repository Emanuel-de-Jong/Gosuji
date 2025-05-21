Write-Host "Usage: build.ps1 [-k] [-sp]"
Write-Host "  -k    Copy KataGoData from Gosuji.API if exists. Makes testing faster but should not be in a public build."
Write-Host "  -sp   Skip publishing the .NET projects. Useful when there were no code changes."
Write-Host ""

$shouldCopyKataGoData = $false
$shouldSkipPublish = $false

foreach ($arg in $args) {
    if ($arg -eq "-k") { $shouldCopyKataGoData = $true }
    if ($arg -eq "-sp") { $shouldSkipPublish = $true }
}

$apiPath = "..\Gosuji\Gosuji.API"
$clientPath = "..\Gosuji\Gosuji.Client"

$apiPublishPath = "$($apiPath)\bin\Release\net8.0\publish"
$clientPublishPath = "$($clientPath)\bin\Release\net8.0\publish"
if (-Not $shouldSkipPublish) {
    if (Test-Path $apiPublishPath) {
        Remove-Item -Path $apiPublishPath -Recurse -Force
    }
    if (Test-Path $clientPublishPath) {
        Remove-Item -Path $clientPublishPath -Recurse -Force
    }

    dotnet publish $apiPath -c Release
    dotnet publish $clientPath -c Release
}

$appApiPath = "app\api"
if (Test-Path $appApiPath) {
    Remove-Item -Path $appApiPath -Recurse -Force
}
$appClientPath = "app\client"
if (Test-Path $appClientPath) {
    Remove-Item -Path $appClientPath -Recurse -Force
}

Copy-Item -Path $apiPublishPath -Destination "app\api" -Recurse
Copy-Item -Path "$($clientPublishPath)\wwwroot\.client" -Destination "app\client" -Recurse

if (Test-Path "build") {
    Remove-Item -Path "build" -Recurse -Force
}

npm --prefix "app" run build

$kataGoDataPath = "..\Gosuji\Gosuji.API\Resources\KataGo\OpenCL\KataGoData"
if ($shouldCopyKataGoData -and (Test-Path $kataGoDataPath)) {
    Copy-Item -Path $kataGoDataPath -Destination "build\win-unpacked\resources\app\api\Resources\KataGo\OpenCL\KataGoData" -Recurse
}
