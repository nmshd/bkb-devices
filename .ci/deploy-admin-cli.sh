#!/bin/bash
set -x
set -e

. "./.ci/shared.sh"

requires APIKEY_NUGET_PUBLISHER

export DOTNET_CLI_HOME=/tmp/DOTNET_CLI_HOME
 
rm -rf ./Devices.AdminCli/bin
rm -rf ./Devices.AdminCli/obj
 
dotnet restore ./Devices.AdminCli/Devices.AdminCli.csproj
dotnet build --no-restore ./Devices.AdminCli/Devices.AdminCli.csproj
dotnet pack --configuration Release --no-restore --no-build ./Devices.AdminCli/Devices.AdminCli.csproj
dotnet nuget push ./Devices.AdminCli/bin/Release/*.nupkg -k $APIKEY_NUGET_PUBLISHER -s https://api.nuget.org/v3/index.json --skip-duplicate
