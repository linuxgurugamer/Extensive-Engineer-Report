
copy Extensive-Engineer-Report\bin\Release\ExtensiveEngineerReport.dll GameData\ExtensiveEngineerReport\Plugins
copy ExtensiveEngineerReport.cfg GameData\ExtensiveEngineerReport\ModuleManager
copy Extensive-Engineer-Report\TagModules\TagModules.cfg GameData\ExtensiveEngineerReport\ModuleManager
copy LICENSE.txt GameData\ExtensiveEngineerReport
copy README.md .GameData\ExtensiveEngineerReport

copy ExtensiveEngineerReport.version GameData\ExtensiveEngineerReport

copy ShipSections\ShipSections\bin\Release\ShipSections\ShipSections.dll GameData\ShipSections\Plugins

copy ShipSections\ShipSections\ShipSections.cfg GameData\ShipSections\ModuleManager
copy ShipSections\ShipSections\ShipSections.png GameData\ShipSections\Textures

copy ShipSections.version GameData\ShipSections\

copy ModuleManager*.dll GameData

set RELEASEDIR=d:\Users\jbb\release
set ZIP="c:\Program Files\7-zip\7z.exe"

copy GameData\ExtensiveEngineerReport\ExtensiveEngineerReport.version a.version
set VERSIONFILE=a.version
rem The following requires the JQ program, available here: https://stedolan.github.io/jq/download/
c:\local\jq-win64  ".VERSION.MAJOR" %VERSIONFILE% >tmpfile
set /P major=<tmpfile

c:\local\jq-win64  ".VERSION.MINOR"  %VERSIONFILE% >tmpfile
set /P minor=<tmpfile

c:\local\jq-win64  ".VERSION.PATCH"  %VERSIONFILE% >tmpfile
set /P patch=<tmpfile

c:\local\jq-win64  ".VERSION.BUILD"  %VERSIONFILE% >tmpfile
set /P build=<tmpfile
del tmpfile
set VERSION=%major%.%minor%.%patch%
if "%build%" NEQ "0"  set VERSION=%VERSION%.%build%

echo Version:  %VERSION%
del a.version


set FILE="%RELEASEDIR%\ExtensiveEngineerReport-%VERSION%.zip"
IF EXIST %FILE% del /F %FILE%
%ZIP% a -tzip %FILE% GameData

pause


set FILE="%RELEASEDIR%\ShipSections-%VERSION%.zip"
IF EXIST %FILE% del /F %FILE%
%ZIP% a -tzip %FILE% GameData/ShipSections
pause