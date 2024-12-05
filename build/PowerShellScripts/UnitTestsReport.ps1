param(
[string]$directoryPath = ""
)

if (-not $directoryPath) {
    $directoryPath = $env:SYSTEM_DEFAULTWORKINGDIRECTORY
}

if (-not (Test-Path -Path $directoryPath)) {
    New-Item -ItemType Directory -Path $directoryPath -Force
}

$logFile = "$directoryPath\UnitTestsLog.txt"
$reportFile = "$directoryPath\UTinfo.txt"

function GetTitle{
    param (
    [string]$lineToSplit
    )

    $title = $lineToSplit -split ' '

    return $title[-1]
}

Clear-Content -Path $reportFile -ErrorAction Ignore

Get-Content $logFile | ForEach-Object {
    $line = $_

    if ($line -match "Discovering:") {
        Add-Content -Path $reportFile -Value (GetTitle -lineToSplit $line)
        Add-Content -Path $reportFile -Value ""
    }

    if ($line -match "Passed ") {
        Add-Content -Path $reportFile -Value $line
    }

    if ($line -match "Failed ") {
        Add-Content -Path $reportFile -Value ("     " + $line)
    }

    if ($line -match "Total time:") {
        Add-Content -Path $reportFile -Value ""
    }
}