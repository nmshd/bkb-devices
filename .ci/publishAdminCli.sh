#!/bin/bash
set -e
set -u
set -x

rm -rf ./Devices.AdminCli/bin
rm -rf ./Devices.AdminCli/obj
 
dotnet restore ./Devices.AdminCli/Devices.AdminCli.csproj
dotnet build --configuration Release --no-restore ./Devices.AdminCli/Devices.AdminCli.csproj
dotnet pack --configuration Release --no-restore --no-build ./Devices.AdminCli/Devices.AdminCli.csproj
dotnet nuget push ./Devices.AdminCli/bin/Release/*.nupkg -k $APIKEY_NUGET_PUBLISHER -s https://api.nuget.org/v3/index.json --skip-duplicate
