dotnet publish "..\Gosuji\Gosuji.API" -c Release
dotnet publish "..\Gosuji\Gosuji.Client" -c Release

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
