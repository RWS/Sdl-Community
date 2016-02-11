[Setup]
AppName=Sdl TMRepair
AppPublisher=SDL Community Developers
AppPublisherURL=https://community.sdl.com/
AppVersion=1.0.1.0
DisableDirPage = yes
DisableWelcomePage = yes
AllowNoIcons = yes
DefaultDirName={pf32}\SDL\SDL Community\SDL TMRepair
Compression=lzma2
SolidCompression=yes
OutputDir=userdocs:Inno Setup Examples Output
PrivilegesRequired = admin
DefaultGroupName=SDL\SDL Community

[Files]
Source: "C:\Repository\Sdl-Community\SDLTMRepair\SDLTMRepair\bin\Debug\Sdl.Community.TMRepair.exe"; DestDir: "{app}"
Source: "C:\Repository\Sdl-Community\SDLTMRepair\SDLTMRepair\bin\Debug\Sdl.Community.TMRepair.exe.config"; DestDir: "{app}"


[Icons]
Name: "{group}\SDL TMRepair"; Filename: "{app}\Sdl.Community.TMRepair.exe"; WorkingDir: "{app}"
Name: {commondesktop}\SDL TMRepair; Filename: {app}\Sdl.Community.TMRepair.exe; WorkingDir: {app};Comment: "SDL TMRepair";
Name: "{group}\Uninstall SDL TMRepair"; Filename: "{uninstallexe}"

[UninstallDelete]
Type: filesandordirs; Name: "c:\Program Files (x86)\SDL\SDL Trados Studio\Studio4\Sdl.Community.TMRepair.exe"

[Code]
function GetUninstallString(): String;
var
  sUnInstPath: String;
  sUnInstallString: String;
  appName: String;
begin
  appName := 'Sdl TMRepair';
  sUnInstPath := ExpandConstant('Software\Microsoft\Windows\CurrentVersion\Uninstall\'+appName+'_is1');
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
