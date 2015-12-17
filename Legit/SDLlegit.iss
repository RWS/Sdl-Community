[Setup]
AppName=Sdl Legit!
AppPublisher=SDL Community Developers
AppPublisherURL=https://community.sdl.com/
AppVersion=1.2.0.0
DisableDirPage = yes
DisableWelcomePage = yes
AllowNoIcons = yes
DefaultDirName={pf32}\SDL\SDL Community\SDL Legit
Compression=lzma2
SolidCompression=yes
OutputDir=userdocs:Inno Setup Examples Output
PrivilegesRequired = admin
DefaultGroupName=SDL\SDL Community

[Files]
Source: "C:\Users\aghisa\AppData\Roaming\SDL\SDL Trados Studio\12\Plugins\Packages\SDLlegit.exe"; DestDir: "{app}"
Source: "C:\Users\aghisa\AppData\Roaming\SDL\SDL Trados Studio\12\Plugins\Packages\SDLlegit.exe.config"; DestDir: "{app}"
Source: "C:\Users\aghisa\AppData\Roaming\SDL\SDL Trados Studio\12\Plugins\Packages\Studio.AssemblyResolver.dll"; DestDir: "{app}"

[Icons]
Name: "{group}\SDL Legit!"; Filename: "{app}\SDLlegit.exe"; WorkingDir: "{app}"
Name: {commondesktop}\SDL Legit!; Filename: {app}\SDLlegit.exe; WorkingDir: {app};Comment: "SDL Legit!";
Name: "{group}\Uninstall SDL Legit!"; Filename: "{uninstallexe}"

[UninstallDelete]
Type: filesandordirs; Name: "c:\Program Files (x86)\SDL\SDL Trados Studio\Studio4\SDLlegit.exe"

[Code]
function GetUninstallString(): String;
var
  sUnInstPath: String;
  sUnInstallString: String;
  appName: String;
begin
  appName := 'Sdl Legit!';
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