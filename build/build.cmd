@echo off

set PJ_D_ROOT=%~dp0..

rem External Tools: powershell (from PowerShell)
set POWERSHELL=powershell

rem Detect powershell
%POWERSHELL% /? 1>nul 2>&1
if errorlevel 1 (
   echo %POWERSHELL%: NOT FOUND
   exit /b 1
)
echo %POWERSHELL%: Found in path

cd "%PJ_D_ROOT%"
%POWERSHELL% -ExecutionPolicy Unrestricted -File .\build.ps1 -Configuration Release