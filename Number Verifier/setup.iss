; -- setup.iss --
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!

[Setup]
AppName=Number Verifier
AppPublisher=SDL Community Developers
AppPublisherURL=https://community.sdl.com/
AppVersion=1.0.3.0
DisableDirPage = yes
DisableWelcomePage = yes
AllowNoIcons = yes
DefaultDirName={localappdata}\SDL\SDL Trados Studio\11\Plugins\Packages
Compression=lzma2
SolidCompression=yes
OutputDir=userdocs:Inno Setup Examples Output

[Files]
Source: "c:\Users\rocrisan\AppData\Roaming\SDL\SDL Trados Studio\11\Plugins\Packages\Sdl.Community.NumberVerifier.sdlplugin"; DestDir: "{app}"

[UninstallDelete]
Type: filesandordirs; Name: "{localappdata}\SDL\SDL Trados Studio\11\Plugins\Unpacked\Sdl.Community.NumberVerifier"