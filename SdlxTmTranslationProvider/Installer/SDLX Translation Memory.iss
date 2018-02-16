[Files]
Source: "[ADD CORRECT PATH HERE]\Sdl.Sdk.SdlxTmTranslationProvider.sdlplugin"; DestDir: "{localappdata}\SDL\SDL Trados Studio\10\Plugins\Packages";
Source: "[ADD CORRECT PATH HERE]\Installer\TMS.ico"; DestDir: "{localappdata}\SDL\SDL Trados Studio\10\Plugins\Packages";

[Setup]
AppCopyright=(c) SDL plc
AppName=SDLX Translation Memory Provider
AppVerName=SDLX Translation Provider 10.2.0.7305
PrivilegesRequired=none
CreateAppDir=false
WizardImageBackColor=$00415900
WizardImageFile="C:\Program Files\Inno\Setup 5\WizModernImage-IS.bmp"
WizardImageStretch=false
WizardSmallImageFile="C:\Program Files\Inno\Setup 5\WizModernSmallImage-IS.bmp"
VersionInfoVersion=10.2.0.7305
VersionInfoCompany=SDL plc
VersionInfoCopyright=© 2011 SDL plc
VersionInfoProductName=SDLX Translation Memory
VersionInfoProductVersion=10.2.0.7305
UninstallDisplayIcon={localappdata}\SDL\SDL Trados Studio\10\Plugins\Packages\TMS.ico
OutputBaseFilename=Sdl.Sdk.SdlxTmTranslationProvider

[InstallDelete]
Type: filesandordirs; Name: "{localappdata}\SDL\SDL Trados Studio\10\Plugins\Unpacked\Sdl.Sdk.SdlxTmTranslationProvider";

[UninstallDelete]
Type: filesandordirs; Name: "{localappdata}\SDL\SDL Trados Studio\10\Plugins\Unpacked\Sdl.Sdk.SdlxTmTranslationProvider";
