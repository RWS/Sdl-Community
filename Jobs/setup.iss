; -- setup.iss --
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!

[Setup]
AppName=Jobs
AppPublisher=SDL Community Developers
AppPublisherURL=https://community.sdl.com/
AppVersion=0.2.1.0
DisableDirPage = yes
DisableWelcomePage = yes
AllowNoIcons = yes
DefaultDirName={localappdata}\SDL\SDL Trados Studio\11\Plugins\Packages
Compression=lzma2
SolidCompression=yes
OutputDir=userdocs:Inno Setup Examples Output

[Files]
Source: "c:\Users\rocrisan\AppData\Roaming\SDL\SDL Trados Studio\12\Plugins\Packages\Sdl.Community.Jobs.sdlplugin"; DestDir: "{app}"

[UninstallDelete]
Type: filesandordirs; Name: "{localappdata}\SDL\SDL Trados Studio\11\Plugins\Unpacked\Sdl.Community.Jobs"

