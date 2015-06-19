@echo off
for /f "delims=" %%i in ('dir %1\*.xml /B ') do (
echo f | groupfile  %1\%%i
)
pause
