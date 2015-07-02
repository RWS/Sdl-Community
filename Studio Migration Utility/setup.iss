; -- setup.iss --
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!

[Setup]
AppName=Studio Migration Utility
AppPublisher=SDL Community Developers
AppPublisherURL=https://community.sdl.com/
AppVersion=1.0.0.0
DisableDirPage = yes
DisableWelcomePage = yes
AllowNoIcons = yes
DefaultDirName={pf32}\SDL\SDL Community\Studio Migration Utility
Compression=lzma2
SolidCompression=yes
OutputDir=userdocs:Inno Setup Examples Output
PrivilegesRequired = admin

[Files]
Source: "c:\Work\Git\SDL-Community\Studio Migration Utility\Sdl.Community.StudioMigrationUtility\bin\Release\Sdl.Community.StudioMigrationUtility.exe"; DestDir: "{app}"
Source: "c:\Work\Git\SDL-Community\Studio Migration Utility\Sdl.Community.StudioMigrationUtility\bin\Release\Sdl.Community.StudioMigrationUtility.exe.config"; DestDir: "{app}"
Source: "c:\Work\Git\SDL-Community\Studio Migration Utility\Sdl.Community.StudioMigrationUtility\bin\Release\Sdl.Community.Controls.dll"; DestDir: "{app}"
Source: "c:\Work\Git\SDL-Community\Studio Migration Utility\Sdl.Community.StudioMigrationUtility\bin\Release\ObjectListView.dll"; DestDir: "{app}"
Source: "c:\Work\Git\SDL-Community\Studio Migration Utility\Sdl.Community.StudioMigrationUtility\bin\Release\Resources\migrate.ico"; DestDir: "{app}"

[UninstallDelete]
Type: filesandordirs; Name: "{pf32}\SDL\SDL Community\Studio Migration Utility"

[Icons]
Name: {group}\SDL\SDL Community\Studio Migration Utility; Filename: {app}\Sdl.Community.StudioMigrationUtility.exe; WorkingDir: {app}; IconFilename: {app}\migrate.ico; Comment: "Studio Migration Utility";
Name: {commondesktop}\Studio Migration Utility; Filename: {app}\Sdl.Community.StudioMigrationUtility.exe; WorkingDir: {app}; IconFilename: {app}\migrate.ico; Comment: "Studio Migration Utility";

