param(
[string]$directoryPath = ""
)

if (-not $directoryPath) {
    $directoryPath = $env:SYSTEM_DEFAULTWORKINGDIRECTORY
}

$folderName = "NuGet"
$appDataPath = [System.Environment]::GetFolderPath('ApplicationData')
$folderPath = Join-Path -Path $appDataPath -ChildPath $folderName

# Define the root directory of your repository
$repoPath = "D:\a\1\s\"

# Get all .csproj files in the repository
$csprojFiles = Get-ChildItem -Path $repoPath -Recurse -Filter *.sln | Select-Object -ExpandProperty FullName

# Function to find MSBuild location by checking common paths
function Get-MSBuildLocation {
    # Array of possible paths where MSBuild might be located
    $possiblePaths = @(
        "$env:ProgramFiles(x86)\Microsoft Visual Studio\2019\*\MSBuild\Current\Bin\MSBuild.exe",
        "$env:ProgramFiles(x86)\Microsoft Visual Studio\2022\*\MSBuild\Current\Bin\MSBuild.exe",
        "$env:ProgramFiles(x86)\Microsoft Visual Studio\2017\*\MSBuild\15.0\Bin\MSBuild.exe",
        "$env:ProgramFiles\Microsoft Visual Studio\2022\*\MSBuild\Current\Bin\MSBuild.exe",
        "$env:ProgramFiles\Microsoft Visual Studio\2019\*\MSBuild\Current\Bin\MSBuild.exe",
        "$env:ProgramFiles\dotnet\sdk\*\MSBuild.dll"
    )

    # Loop through each possible path to check if MSBuild.exe exists
    foreach ($path in $possiblePaths) {
        $msbuild = Get-ChildItem -Path $path -ErrorAction SilentlyContinue

        # If MSBuild is found, return the path
        if ($msbuild) {
            return $msbuild.FullName
        }
    }

    # If MSBuild was not found in the common paths, display a message
    Write-Host "MSBuild.exe not found in the common locations." -ForegroundColor Red
    return $null
}

# Call the function and store the MSBuild location
$msbuildLocation = Get-MSBuildLocation

Set-Alias MSBuild -Value $msbuildLocation;


$feedName = 'SDLNuget'
$nugetRestoreArguments = "/p:RestoreSources=https://pkgs.dev.azure.com/sdl/_packaging/$feedName/nuget/v3/index.json"
$msbuildArguments = "/flp:logfile=$directoryPath/AzureLogs/MyLog.log;append=true"

foreach ($project in $csprojFiles) {
        if (Test-Path -Path $folderPath) {
    Remove-Item -Path $folderPath -Recurse -Force
    }
  #  MSBuild "/bl" -m "$project" "/t:Restore" /p:nugetInteractive=true $nugetRestoreArguments
    MSBuild "/bl" -m "$project" "/t:Restore" $nugetRestoreArguments -p:RestorePackagesConfig=true
    MSBuild  -m "$project" "/t:Rebuild" $msbuildArguments
    if (! $?) {  write-Host "msbuild failed" -ForegroundColor Red ; }
}