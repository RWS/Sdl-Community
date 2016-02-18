[Setup]
AppName=TMOptimizer
AppPublisher=SDL Community Developers
AppPublisherURL=https://community.sdl.com/
AppVersion=1.1.0.0
DisableDirPage = yes
DisableWelcomePage = yes
AllowNoIcons = yes
DefaultDirName={pf32}\SDL\SDL Community\TMOptimizer
Compression=lzma2
SolidCompression=yes
OutputDir=userdocs:Inno Setup Examples Output
PrivilegesRequired = admin
DefaultGroupName=SDL\SDL Community
SetupIconFile = "C:\Repository\Sdl-Community\TM Optimizer\Sdl.Community.TMOptimizer\Images\product.ico"
UninstallDisplayIcon={app}\product.ico


[Files]
Source: "C:\Users\aghisa\AppData\Roaming\SDL\SDL Trados Studio\12\Plugins\Packages\Sdl.Community.TMOptimizer.exe"; DestDir: "{app}"
Source: "C:\Users\aghisa\AppData\Roaming\SDL\SDL Trados Studio\12\Plugins\Packages\Sdl.Community.TMOptimizer.exe.config"; DestDir: "{app}"
Source: "C:\Users\aghisa\AppData\Roaming\SDL\SDL Trados Studio\12\Plugins\Packages\Sdl.Community.TMOptimizerLib.dll"; DestDir: "{app}"
Source: "C:\Users\aghisa\AppData\Roaming\SDL\SDL Trados Studio\12\Plugins\Packages\Xceed.Wpf.Toolkit.dll"; DestDir: "{app}"
Source: "C:\Users\aghisa\AppData\Roaming\SDL\SDL Trados Studio\12\Plugins\Packages\Xceed.Wpf.DataGrid.dll"; DestDir: "{app}"
Source: "C:\Users\aghisa\AppData\Roaming\SDL\SDL Trados Studio\12\Plugins\Packages\Xceed.Wpf.AvalonDock.Themes.VS2010.dll"; DestDir: "{app}"
Source: "C:\Users\aghisa\AppData\Roaming\SDL\SDL Trados Studio\12\Plugins\Packages\Xceed.Wpf.AvalonDock.Themes.Metro.dll"; DestDir: "{app}"
Source: "C:\Users\aghisa\AppData\Roaming\SDL\SDL Trados Studio\12\Plugins\Packages\Xceed.Wpf.AvalonDock.Themes.Aero.dll"; DestDir: "{app}"
Source: "C:\Users\aghisa\AppData\Roaming\SDL\SDL Trados Studio\12\Plugins\Packages\Xceed.Wpf.AvalonDock.dll"; DestDir: "{app}"
Source: "C:\Users\aghisa\AppData\Roaming\SDL\SDL Trados Studio\12\Plugins\Packages\Studio.AssemblyResolver.dll"; DestDir: "{app}"
Source: "C:\Repository\Sdl-Community\TM Optimizer\Sdl.Community.TMOptimizer\Images\product.ico"; DestDir: "{app}"; Flags: ignoreversion


[Icons]
Name: {group}\TMOptimizer; Filename: {app}\Sdl.Community.TMOptimizer.exe; WorkingDir: {app}; IconFilename: {app}\product.ico; Comment: "TMOptimizer" 
Name: {commondesktop}\TMOptimizer; Filename: {app}\Sdl.Community.TMOptimizer.exe; WorkingDir: {app}; IconFilename: {app}\product.ico; Comment: "TMOptimizer"
Name: "{group}\Uninstall TMOptimizer"; Filename: "{uninstallexe}"; IconFilename: "{app}\product.ico";Comment:"TmOptimizer"


[UninstallDelete]
Type: filesandordirs; Name: "c:\Program Files (x86)\SDL\SDL Community\TMOptimizer\Sdl.Community.TMOptimizer.exe"

[Code]
function GetUninstallString(): String;
var
  sUnInstPath: String;
  sUnInstallString: String;
  appName: String;
begin
  appName := 'TMOptimizer';
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