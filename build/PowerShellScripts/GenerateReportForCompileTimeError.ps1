param(
[string]$directoryPath = "",
[string]$resultPath = "",
[string]$reportName = "",
[string]$poolName = ""
)

$agentDirectory

if (-not $directoryPath) {
    $directoryPath = $env:SYSTEM_DEFAULTWORKINGDIRECTORY
}

if (-not $resultPath) {
    $resultPath = $env:SYSTEM_DEFAULTWORKINGDIRECTORY
}

if (-not $poolName) {
    $poolName = $env:SYSTEM_DEFAULTWORKINGDIRECTORY
}

if (-not $reportName) {
    $reportName = "Report"
}

# Create the directory if it doesn't exist
if (-not (Test-Path -Path $directoryPath)) {
    New-Item -ItemType Directory -Path $directoryPath -Force
}

if (-not (Test-Path -Path $resultPath)) {
    New-Item -ItemType Directory -Path $resultPath -Force
}

# Define file paths
$logFile = "$directoryPath\MyLog.log"      # Path to your input log file
$reportFile = "$directoryPath\DemoReport.txt" # Path to your output report file

# Initialize flags and data
$inBuildFailedSection = $false
$inTestFailedSection = $false

# Clear the report file if it already exists
Clear-Content -Path $reportFile -ErrorAction Ignore

# Read the log file line by line
Get-Content $logFile | ForEach-Object {
    $line = $_

    # Check if the line contains "Build FAILED."
    if ($line -match "Build FAILED.") {
        $inBuildFailedSection = $true
    }

    # If we're in the failed build section, append the line to the report file
    if ($inBuildFailedSection) {
        Add-Content -Path $reportFile -Value $line
    }

    # Check if the line contains "Time Elapsed" and stop copying lines
    if ($line -match "Time Elapsed") {
        $inBuildFailedSection = $false
    }

    if ($line -match "Starting test execution, please wait...") {
        $inTestFailedSection = $true
    }

    if ($inTestFailedSection) {
        Add-Content -Path $reportFile -Value $line
    }

    if ($line -match "Total time:") {
        $inTestFailedSection = $false
    }

}

# Define file paths
$inputFile = "$directoryPath\DemoReport.txt"   # Path to the input file
$outputFile = "$directoryPath\DemoReportWithTitle.txt"  # Path to the output file

# Initialize flags and variables
$processing = $false
$processedLine = $false

# Clear the output file if it already exists
Clear-Content -Path $outputFile -ErrorAction Ignore

if (Test-Path $inputFile) {
# Read the input file line by line
Get-Content $inputFile | ForEach-Object {
    $line = $_

    # Check if we are processing a section
    if ($processing) {
        if (-not $processedLine -and $line -ne "") {

            if ($poolName -eq "FlaviusPool") {
                $line = $line -replace "C:\\agent\\_work\\11\\s\\", "~" 
            } elseif ($poolName -eq "Azure Pipelines") {
                $line = $line -replace "D:\\a\\1\\s\\", "~" 
            }

            # Truncate at the first backslash
            if ($line -match "\\") {
                $line = $line -split "\\", 2 | Select-Object -First 1
            }

            # Write the processed line to the output file
            Add-Content -Path $outputFile -Value $line

            # Set flag to indicate we've processed this line
            $processedLine = $true
        } else {
            # Copy remaining lines as they are until the next "Build FAILED."
            if ($line -match "Build FAILED.") {
                # Add a blank line for separation if needed
                Add-Content -Path $outputFile -Value ""

                # Reset processing flags
                $processing = $true
                $processedLine = $false
            } else {
                Add-Content -Path $outputFile -Value $line
            }
        }
    }

    # Detect the start of a new section
    if ($line -match "Build FAILED.") {
        # Start processing after this line
        $processing = $true
        $processedLine = $false
    } else {
        # Copy lines that are not part of any section being processed
        if (-not $processing) {
            Add-Content -Path $outputFile -Value $line
        }
    }
}

# Define file paths
$input1 = "$directoryPath\DemoReportWithTitle.txt"   # Path to the input file
$output1 = "$directoryPath\FilteredFile.txt"  # Path to the output file

# Clear the output file if it already exists
Clear-Content -Path $output1 -ErrorAction Ignore

# Read the input file line by line and filter out lines containing 'warning'
Get-Content $input1 | ForEach-Object {
    $line = $_

    # Check if the line does NOT contain the word 'warning'
    if ($line -notmatch "warning" -and $inTestFailedSection -eq $false) {
        # Write the line to the output file
        Add-Content -Path $output1 -Value $line
    }

    if ($line -match "Failed ") {
        Add-Content -Path $output1 -Value $line
        $inTestFailedSection = $true
    }

    if ($inTestFailedSection) {
        Add-Content -Path $output1 -Value $line
    }

    if ($line -match "Stack Trace:") {
        $inTestFailedSection = $false
    }

}

# Define file paths
$input2 = "$directoryPath\FilteredFile.txt"   # Path to the input file
$output2 = "$directoryPath\OnlyErrors.txt"  # Path to the output file

# Clear the output file if it already exists
Clear-Content -Path $output2 -ErrorAction Ignore

# Read the input file line by line and filter out lines containing "->" or "Time Elapsed"
Get-Content $input2 | ForEach-Object {
    $line = $_

    # Check if the line does NOT contain "->" or "Time Elapsed"
    if ($line -notmatch "->" -and $line -notmatch "Time Elapsed") {
        # Write the line to the output file
        Add-Content -Path $output2 -Value $line
    }
}

# Define file paths
$input3 = "$directoryPath\OnlyErrors.txt"   # Path to the input file
$output3 = "$directoryPath\WithoutEmptyLines.txt"  # Path to the output file

# Clear the output file if it already exists
Clear-Content -Path $output3 -ErrorAction Ignore

# Read the input file line by line and filter out empty lines
Get-Content $input3 | ForEach-Object {
    $line = $_

    # Check if the line is not empty
    if ($line.Trim() -ne "") {
        # Write the non-empty line to the output file
        Add-Content -Path $output3 -Value $line
    }
}

# Define file paths
$input4 = "$directoryPath\WithoutEmptyLines.txt"   # Path to the input file
$output4 = "$directoryPath\AddEmptyLine.txt"  # Path to the output file

# Clear the output file if it already exists
Clear-Content -Path $output4 -ErrorAction Ignore

# Read the input file line by line
Get-Content $input4 | ForEach-Object {
    $line = $_

    # Write the current line to the output file
    Add-Content -Path $output4 -Value $line

    # If the line contains the character "~", add an empty line after it
    if ($line -match "~") {
        Add-Content -Path $output4 -Value ""
    }
}

# Define file paths
$input5 = "$directoryPath\AddEmptyLine.txt"   # Path to the input file
$output5 = "$directoryPath\ReplaceString.txt"  # Path to the output file

# Clear the output file if it already exists
Clear-Content -Path $output5 -ErrorAction Ignore

Get-Content $input5 | ForEach-Object {
    $line = $_

    # If the line contains "(s)", replace it with an empty line
    if ($line -match "Warning\(s\)|Error\(s\)") {
    Add-Content -Path $output5 -Value ""
} else {
    # Otherwise, write the line as it is
    Add-Content -Path $output5 -Value $line
}

}

# Define file paths
$input6 = "$directoryPath\ReplaceString.txt"   # Path to the input file
$output6 = "$resultPath" + "$reportName"  # Path to the output file

# Clear the output file if it already exists
Clear-Content -Path $output6 -ErrorAction Ignore

# Read the input file line by line
Get-Content $input6 | ForEach-Object {
    $line = $_

    # If the line contains '~', keep only the part after '~'
    if ($line -match "~") {
        # Split the line at the first occurrence of '~' and keep the part after it
        $line = $line -split "~", 2 | Select-Object -Last 1
    }

    # Write the modified or original line to the output file
    Add-Content -Path $output6 -Value $line
}
}else{
    $output6 = "$resultPath" + "$reportName"
    Clear-Content -Path $output6 -ErrorAction Ignore
    Add-Content -Path $output6 -Value "No error Found"
}