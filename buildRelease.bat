
copy bin\Release\ExtensiveEngineerReport.dll ..\GameData\ExtensiveEngineerReport\Plugins
copy ExtensiveEngineerReport.cfg ..\GameData\ExtensiveEngineerReport\ModuleManager
copy TagModules\TagModules.cfg ..\GameData\ExtensiveEngineerReport\ModuleManager
copy LICENSE.txt ..\GameData\ExtensiveEngineerReport
copy README.md ..\GameData\ExtensiveEngineerReport

copy ExtensiveEngineerReport.version ..\GameData\ExtensiveEngineerReport

copy ..\ShipSections\ShipSections\bin\Release\ShipSections\ShipSections.dll ..\GameData\ShipSections\Plugins

copy ..\ShipSections\ShipSections\ShipSections.cfg ..\GameData\ShipSections\ModuleManager
copy ..\ShipSections\ShipSections\ShipSections.png ..\GameData\ShipSections\Textures

copy ..\ShipSections\ShipSections.version ..\GameData\ShipSections\

cd ..
copy ..\ModuleManager*.dll GameData

set DEFHOMEDRIVE=d:
set DEFHOMEDIR=%DEFHOMEDRIVE%%HOMEPATH%
set HOMEDIR=
set HOMEDRIVE=%CD:~0,2%

set RELEASEDIR=d:\Users\jbb\release
set ZIP="c:\Program Files\7-zip\7z.exe"
echo Default homedir: %DEFHOMEDIR%

rem set /p HOMEDIR= "Enter Home directory, or <CR> for default: "

if "%HOMEDIR%" == "" (
set HOMEDIR=%DEFHOMEDIR%
) 
echo %HOMEDIR%

SET _test=%HOMEDIR:~1,1%
if "%_test%" == ":" (
set HOMEDRIVE=%HOMEDIR:~0,2%
)


type GameData\ExtensiveEngineerReport\ExtensiveEngineerReport.version
set /p VERSION= "Enter version: "


set FILE="%RELEASEDIR%\ExtensiveEngineerReport-%VERSION%.zip"
IF EXIST %FILE% del /F %FILE%
%ZIP% a -tzip %FILE% GameData

pause

type GameData\ShipSections\ShipSections.version
set /p VERSION= "Enter version: "


set FILE="%RELEASEDIR%\ShipSections-%VERSION%.zip"
IF EXIST %FILE% del /F %FILE%
%ZIP% a -tzip %FILE% GameData/ShipSections
pause