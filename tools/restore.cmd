@ECHO OFF
dotnet restore %~dp0..\
pushd %~dp0..
tools\nuget.exe restore "src\Microsoft.Diagnostics.Correlation.Signing\Microsoft.Diagnostics.Correlation.Signing.csproj" -PackagesDirectory packages
popd