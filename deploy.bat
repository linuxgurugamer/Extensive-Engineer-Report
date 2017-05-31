

copy Extensive-Engineer-Report\bin\Debug\ExtensiveEngineerReport.dll GameData\ExtensiveEngineerReport\Plugins
copy ExtensiveEngineerReport.cfg GameData\ExtensiveEngineerReport\ModuleManager
copy Extensive-Engineer-Report\TagModules\TagModules.cfg GameData\ExtensiveEngineerReport\ModuleManager
copy LICENSE.txt GameData\ExtensiveEngineerReport
copy README.md GameData\ExtensiveEngineerReport



copy ShipSections\ShipSections\bin\Debug\ShipSections\ShipSections.dll GameData\ShipSections\Plugins

copy ShipSections\ShipSections\ShipSections.cfg GameData\ShipSections\ModuleManager
copy ShipSections\ShipSections\ShipSections.png GameData\ShipSections\Textures

cd GameData
xcopy /s /y ExtensiveEngineerReport R:\KSP_1.3.0_dev\GameData\ExtensiveEngineerReport
xcopy /s /y ShipSections R:\KSP_1.3.0_dev\GameData\ShipSections
