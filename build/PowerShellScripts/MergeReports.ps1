param(
[string]$directoryPath = "",
[string]$outputFile = "",
[string]$file1 = "",
[string]$file2 = ""
)

if (-not $directoryPath) {
    $directoryPath = $env:SYSTEM_DEFAULTWORKINGDIRECTORY
}

if (-not $outputFile) {
    $outputFile = $env:SYSTEM_DEFAULTWORKINGDIRECTORY
}

if (-not $file1) {
    $file1 = $env:SYSTEM_DEFAULTWORKINGDIRECTORY
}

if (-not $file2) {
    $file2 = $env:SYSTEM_DEFAULTWORKINGDIRECTORY
}

Get-Content -Path $file1, ($directoryPath + $file2) | Set-Content -Path $outputFile