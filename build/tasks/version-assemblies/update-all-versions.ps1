param(
[string]$PathToVersion="$psscriptroot\..\..\..",
[string]$VersionString="17.0.0.11594",
[string]$ProductIdentifier="studio17",
[string]$InstallPathToVersion="$psscriptroot\..\..\.."
)


write-output "`$PathToVersion=$PathToVersion"
write-output "`$VersionString=$VersionString"
write-output "`$InstallPathToVersion=$InstallPathToVersion"

#arguments: '-Path "${{ parameters.PathToVersion }}\src\Sdl" -VersionNumber "$(Version.Major).$(Version.Minor).$(Version.Patch).$(ProjectBuildId)" -FilenamePattern "*.info.cs,*.rc,AssemblyInfo.cpp" -Field "AssemblyFileVersion;AssemblyVersionAttribute;FileVersion"'

& "$PSScriptRoot\apply-version-to-sourcefiles.ps1" "$PathToVersion" "$VersionString" "*Info.cs" "AssemblyVersionAttribute;AssemblyVersionAttribute;FileVersion" "$ProductIdentifier"

& "$PSScriptRoot\set-wix-version.ps1" "$VersionString" "$ProductIdentifier" "$FullProductName" "$InstallerDisplayName" "$InstallPathToVersion"
