; -- setup.iss --
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!

[Setup]
AppName=Studio Migration Utility
AppPublisher=SDL Community Developers
AppPublisherURL=https://community.sdl.com/
AppVersion=2.2.0.0
DisableDirPage = yes
DisableWelcomePage = yes
AllowNoIcons = yes
DefaultDirName={pf32}\SDL\SDL Community\Studio Migration Utility
Compression=lzma2
SolidCompression=yes
OutputDir=userdocs:Inno Setup Examples Output
PrivilegesRequired = admin
DefaultGroupName=SDL\SDL Community

[Files]
Source: "c:\Work\Git\SDL-Community\Studio Migration Utility\Sdl.Community.StudioMigrationUtility\bin\Release\Sdl.Community.StudioMigrationUtility.exe"; DestDir: "{app}"
Source: "c:\Work\Git\SDL-Community\Studio Migration Utility\Sdl.Community.StudioMigrationUtility\bin\Release\Sdl.Community.StudioMigrationUtility.exe.config"; DestDir: "{app}"
Source: "c:\Work\Git\SDL-Community\Studio Migration Utility\Sdl.Community.StudioMigrationUtility\bin\Release\Sdl.Community.Controls.dll"; DestDir: "{app}"
Source: "c:\Work\Git\SDL-Community\Studio Migration Utility\Sdl.Community.StudioMigrationUtility\bin\Release\ObjectListView.dll"; DestDir: "{app}"
Source: "c:\Work\Git\SDL-Community\Studio Migration Utility\Sdl.Community.StudioMigrationUtility\bin\Release\Resources\migrate.ico"; DestDir: "{app}"

[UninstallDelete]
Type: filesandordirs; Name: "{pf32}\SDL\SDL Community\Studio Migration Utility"

[Icons]
Name: {group}\Studio Migration Utility; Filename: {app}\Sdl.Community.StudioMigrationUtility.exe; WorkingDir: {app}; IconFilename: {app}\migrate.ico; Comment: "Studio Migration Utility";
Name: {commondesktop}\Studio Migration Utility; Filename: {app}\Sdl.Community.StudioMigrationUtility.exe; WorkingDir: {app}; IconFilename: {app}\migrate.ico; Comment: "Studio Migration Utility";
[Code]
function GetUninstallString(): String;
var
  sUnInstPath: String;
  sUnInstallString: String;
begin
  sUnInstPath := ExpandConstant('Software\Microsoft\Windows\CurrentVersion\Uninstall\{#emit SetupSetting("AppName")}_is1');
  sUnInstallString := '';
  if not RegQueryStringValue(HKLM, sUnInstPath, 'UninstallString', sUnInstallString) then
    RegQueryStringValue(HKCU, sUnInstPath, 'UninstallString', sUnInstallString);
  Result := sUnInstallString;
end;
function IsUpgrade(): Boolean;
begin
  Result := (GetUninstallString() <> '');
end;
function UnInstallOldVersion(): Integer;
var
  sUnInstallString: String;
  iResultCode: Integer;
begin
// Return Values:
// 1 - uninstall string is empty
// 2 - error executing the UnInstallString
// 3 - successfully executed the UnInstallString

  // default return value
  Result := 0;

  // get the uninstall string of the old app
  sUnInstallString := GetUninstallString();
  if sUnInstallString <> '' then begin
    sUnInstallString := RemoveQuotes(sUnInstallString);
    if Exec(sUnInstallString, '/SILENT /NORESTART /SUPPRESSMSGBOXES','', SW_HIDE, ewWaitUntilTerminated, iResultCode) then
      Result := 3
    else
      Result := 2;
  end else
    Result := 1;
end;

/////////////////////////////////////////////////////////////////////
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if (CurStep=ssInstall) then
  begin
    if (IsUpgrade()) then
    begin
      UnInstallOldVersion();
    end;
  end;
end;
