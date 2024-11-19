param(
[string]$defaultWorkingDirectory = "",
[string]$poolname = ""
)

$index

if (-not $defaultWorkingDirectory) {
    $defaultWorkingDirectory = $env:SYSTEM_DEFAULTWORKINGDIRECTORY
}

if (-not $poolname) {
    $poolname = $env:SYSTEM_DEFAULTWORKINGDIRECTORY
}

if ($poolName -eq "FlaviusPool") {
    $index = 5
} elseif ($poolName -eq "Azure Pipelines") {
    $index = 4
}

$folderName = "NuGet"
$appDataPath = [System.Environment]::GetFolderPath('ApplicationData')
$folderPath = Join-Path -Path $appDataPath -ChildPath $folderName


# Get all .csproj files in the repository
$csprojFiles = Get-ChildItem -Path $defaultWorkingDirectory -Recurse -Filter *.sln | Select-Object -ExpandProperty FullName


function UnitTestsExists {
    param (
        [string]$solutionPath
    )

     $testAssemblie = Get-ChildItem -Path $solutionPath -Recurse -Include "*UnitTests.dll" | Where-Object { $_.FullName -notlike "*TestAdapter.dll" -and $_.FullName -notlike "*\obj\*" }

     if ($testAssemblie.Count -ne 0) {
       return $true
    }

    return $false
}

function BuildUnitTests {
    param (
        [string]$solutionPath
    )

    $testAssemblie = Get-ChildItem -Path $solutionPath -Recurse -Include "*UnitTests.dll" | Where-Object { $_.FullName -notlike "*TestAdapter.dll" -and $_.FullName -notlike "*\obj\*" }

    if ($testAssemblie.Count -ne 0) {

        
        & "$vsTestPath" "$testAssemblie" /Logger:"Console;Verbosity=normal" `
    | Tee-Object -FilePath "$defaultWorkingDirectory\AzureLogs\UnitTestsLog.txt" -Append
        

     #    & "$vsTestPath" "$testAssemblie" `
     #   /Logger:"Console;Verbosity=normal" ` | Tee-Object -FilePath "$defaultWorkingDirectory/AzureLogs/MyLog.log" -Appen
       
    }

}

function Get-VSTestConsolePath {
    $possibleLocations = @(
        "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\Extensions\TestPlatform\vstest.console.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Professional\Common7\IDE\Extensions\TestPlatform\vstest.console.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\Extensions\TestPlatform\vstest.console.exe",
        "C:\Program Files\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\Extensions\TestPlatform\vstest.console.exe",
        "C:\Program Files\Microsoft Visual Studio\2019\Professional\Common7\IDE\Extensions\TestPlatform\vstest.console.exe",
        "C:\Program Files\Microsoft Visual Studio\2019\BuildTools\Common7\IDE\Extensions\TestPlatform\vstest.console.exe",
        "C:\Program Files\Microsoft Visual Studio\2019\Community\Common7\IDE\Extensions\TestPlatform\vstest.console.exe"
    )

    foreach ($location in $possibleLocations) {
        if (Test-Path $location) {
            return $location
        }
    }

    Write-Host "VSTest.Console.exe not found in the standard locations." -ForegroundColor Red
    return $null
}

$vsTestPath = Get-VSTestConsolePath

# Function to find MSBuild location by checking common paths
function Get-MSBuildLocation {
    # Array of possible paths where MSBuild might be located
    $possiblePaths = @(
        "$env:ProgramFiles(x86)\Microsoft Visual Studio\2022\*\MSBuild\Current\Bin\MSBuild.exe",
        "$env:ProgramFiles(x86)\Microsoft Visual Studio\2019\*\MSBuild\Current\Bin\MSBuild.exe",
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
$msbuildArguments = "/flp:logfile=$defaultWorkingDirectory/Logs/MyLog.log;append=true"

Write-Host ">>>>>>> : $poolName, $index"

foreach ($project in $csprojFiles) {
        if (Test-Path -Path $folderPath) {
    Remove-Item -Path $folderPath -Recurse -Force
    }

    if (Test-Path -Path $project) {

        MSBuild "/bl" -m "$project" "/t:Restore" $nugetRestoreArguments -p:RestorePackagesConfig=true
        MSBuild  -m "$project" "/t:Rebuild" $msbuildArguments

        if (! $?) {  write-Host "msbuild failed" -ForegroundColor Red ; }

        $itemFolder = $project -split '\\'
        $joinedString = ($itemFolder[0..($itemFolder.Count - 2)] -join '\')

        $unitTestExists = UnitTestsExists -solutionPath $joinedString

        if ($unitTestExists){
            BuildUnitTests -solutionPath $joinedString
        }

        if (Test-Path -Path $joinedString) {
            Write-Host "~~~  Following will be deleted: $joinedString"
            Remove-Item -Path $joinedString -Recurse -Force
        }
    }

}