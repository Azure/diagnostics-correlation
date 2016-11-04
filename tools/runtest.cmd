@ECHO OFF
SET failed=0
for /f %%a in ('dir %~dp0..\test\project.json /b /s ^| find /i "Test"') do dotnet test %%a || set failed=1
EXIT /B %failed%