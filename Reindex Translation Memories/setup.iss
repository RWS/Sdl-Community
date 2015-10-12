; -- setup.iss --
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!

[Setup]
AppName=Reindex Translation Memories
AppPublisher=SDL Community Developers
AppPublisherURL=https://community.sdl.com/
AppVersion=0.2.3.0
DisableDirPage = yes
DisableWelcomePage = yes
AllowNoIcons = yes
DefaultDirName={pf32}\SDL\SDL Trados Studio\Studio4\
Compression=lzma2
SolidCompression=yes
OutputDir=userdocs:Inno Setup Examples Output
PrivilegesRequired = admin
DefaultGroupName=SDL\SDL Community

[Files]
Source: "c:\Program Files (x86)\SDL\SDL Trados Studio\Studio4\Sdl.Community.ReindexTms.exe"; DestDir: "{app}"
Source: "c:\Program Files (x86)\SDL\SDL Trados Studio\Studio4\Sdl.Community.ReindexTms.exe.config"; DestDir: "{app}"

[Icons]
Name: "{group}\Reindex Translation Memories"; Filename: "{app}\Sdl.Community.ReindexTms.exe"; WorkingDir: "{app}"
Name: {commondesktop}\Reindex Translation Memories; Filename: {app}\Sdl.Community.ReindexTms.exe; WorkingDir: {app};Comment: "Reindex Translation Memories";
Name: "{group}\Uninstall Reindex Translation Memories"; Filename: "{uninstallexe}"

[UninstallDelete]
Type: filesandordirs; Name: "c:\Program Files (x86)\SDL\SDL Trados Studio\Studio4\Sdl.Community.ReindexTms.exe"
Type: filesandordirs; Name: "c:\Program Files (x86)\SDL\SDL Trados Studio\Studio4\Sdl.Community.ReindexTms.exe.config"
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