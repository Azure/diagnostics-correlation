@ECHO OFF
dotnet restore %~dp0..\ --source "https://api.nuget.org/v3/index.json" --source "\\ddfiles\Team\Public\Correlation\Nugets\signed"
pushd %~dp0..
tools\nuget.exe restore "src\Microsoft.Diagnostics.Correlation.Signing\Microsoft.Diagnostics.Correlation.Signing.csproj" -PackagesDirectory packages
popd