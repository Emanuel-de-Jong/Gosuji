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

if (-Not $shouldSkipPublish) {
    dotnet publish "..\Gosuji\Gosuji.API" -c Release
    dotnet publish "..\Gosuji\Gosuji.Client" -c Release
}

if (Test-Path "app\api") {
    Remove-Item -Path "app\api" -Recurse -Force
}
if (Test-Path "app\client") {
    Remove-Item -Path "app\client" -Recurse -Force
}

Copy-Item -Path "..\Gosuji\Gosuji.API\bin\Release\net8.0\publish" -Destination "app\api" -Recurse
Copy-Item -Path "..\Gosuji\Gosuji.Client\bin\Release\net8.0\publish\wwwroot\.client" -Destination "app\client" -Recurse

if (Test-Path "build") {
    Remove-Item -Path "build" -Recurse -Force
}

npm --prefix "app" run build

$kataGoDataPath = "..\Gosuji\Gosuji.API\Resources\KataGo\OpenCL\KataGoData"
if ($shouldCopyKataGoData -and (Test-Path $kataGoDataPath)) {
    Copy-Item -Path $kataGoDataPath -Destination "build\win-unpacked\resources\app\api\Resources\KataGo\OpenCL\KataGoData" -Recurse
}
