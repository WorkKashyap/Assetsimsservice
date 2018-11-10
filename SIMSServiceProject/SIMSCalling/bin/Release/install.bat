@ECHO OFF

REM The following directory is for .NET 4.0
set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
set PATH=%PATH%;%DOTNETFX2%

echo Installing IEPPAMS Win Service...
set CURDIR="C:\Program Files (x86)\SIMS\SIMS .net\SIMSServices.exe"
echo ---------------------------------------------------
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil  %CURDIR% 
echo ---------------------------------------------------
pause
echo Done.