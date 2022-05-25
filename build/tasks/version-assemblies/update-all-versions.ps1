param(
[string]$PathToVersion="$psscriptroot\..\..\..",
[string]$VersionString="16.0.0.42",
[string]$ProductIdentifier="studio16",
[string]$InstallPathToVersion="$psscriptroot\..\..\.."
)


write-output "`$PathToVersion=$PathToVersion"
write-output "`$VersionString=$VersionString"
write-output "`$InstallPathToVersion=$InstallPathToVersion"

#arguments: '-Path "${{ parameters.PathToVersion }}\src\Sdl" -VersionNumber "$(Version.Major).$(Version.Minor).$(Version.Patch).$(ProjectBuildId)" -FilenamePattern "*.info.cs,*.rc,AssemblyInfo.cpp" -Field "AssemblyFileVersion;AssemblyVersionAttribute;FileVersion"'

& "$PSScriptRoot\apply-version-to-sourcefiles.ps1" "$PathToVersion" "$VersionString" "*Info.cs" "AssemblyVersionAttribute;AssemblyVersionAttribute;FileVersion" "$ProductIdentifier"

& "$PSScriptRoot\set-wix-version.ps1" "$VersionString" "$ProductIdentifier" "$FullProductName" "$InstallerDisplayName" "$InstallPathToVersion"
