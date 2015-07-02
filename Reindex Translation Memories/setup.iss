; -- setup.iss --
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!

[Setup]
AppName=Reindex Translation Memories
AppPublisher=SDL Community Developers
AppPublisherURL=https://community.sdl.com/
AppVersion=0.2.1.0
DisableDirPage = yes
DisableWelcomePage = yes
AllowNoIcons = yes
DefaultDirName={pf32}\SDL\SDL Trados Studio\Studio3\
Compression=lzma2
SolidCompression=yes
OutputDir=userdocs:Inno Setup Examples Output

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