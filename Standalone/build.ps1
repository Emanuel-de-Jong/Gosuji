dotnet publish "..\Gosuji\Gosuji.API" -c Release
dotnet publish "..\Gosuji\Gosuji.Client" -c Release

if (Test-Path "api") {
    Remove-Item -Path "api" -Recurse -Force
}
if (Test-Path "client") {
    Remove-Item -Path "client" -Recurse -Force
}

Copy-Item -Path "..\Gosuji\Gosuji.API\bin\Release\net8.0\publish" -Destination "api" -Recurse
Copy-Item -Path "..\Gosuji\Gosuji.Client\bin\Release\net8.0\publish\wwwroot\.client" -Destination "client" -Recurse

if (Test-Path "builder_out") {
    Remove-Item -Path "builder_out" -Recurse -Force
}

npm run build

