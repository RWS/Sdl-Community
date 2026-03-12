$buildDir = "c:\Users\ftritean\source\repos\Sdl-Community\build"

$files = Get-ChildItem -Path $buildDir -Filter "*.yml" | Where-Object { $_.Name -notmatch 'Sonar' -and $_.Name -ne 'DeepLMTProvider.yml' }

$count = 0
$skipped = @()

foreach ($file in $files) {
    $lines = Get-Content $file.FullName

    # Find steps: line index
    $stepsIndex = -1
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match '^steps:') {
            $stepsIndex = $i
            break
        }
    }

    if ($stepsIndex -lt 0) {
        $skipped += $file.Name
        continue
    }

    # Get content before steps
    $beforeSteps = if ($stepsIndex -gt 0) { $lines[0..($stepsIndex - 1)] } else { @() }

    # Check if resources already exists in the content before steps
    $hasResources = ($beforeSteps | Where-Object { $_ -match '^resources:' }).Count -gt 0

    $newLines = @()

    if (-not $hasResources) {
        # Add resources block at the top
        $newLines += "resources:"
        $newLines += "  repositories:"
        $newLines += "  - repository: AppStoreApplicationsRepo"
        $newLines += "    type: git"
        $newLines += "    name: AppStoreApplications/AppStoreApplications"
        $newLines += "    ref: 'Studio_2026'"
        $newLines += ""

        # Add original content before steps (skip leading blank lines)
        $startIdx = 0
        while ($startIdx -lt $beforeSteps.Count -and [string]::IsNullOrWhiteSpace($beforeSteps[$startIdx])) {
            $startIdx++
        }
        if ($startIdx -lt $beforeSteps.Count) {
            $newLines += $beforeSteps[$startIdx..($beforeSteps.Count - 1)]
        }
    } else {
        # Resources already exists - replace the resources block with new one
        $resourcesStart = -1
        $resourcesEnd = -1
        for ($i = 0; $i -lt $beforeSteps.Count; $i++) {
            if ($beforeSteps[$i] -match '^resources:') {
                $resourcesStart = $i
                for ($j = $i + 1; $j -lt $beforeSteps.Count; $j++) {
                    if ($beforeSteps[$j] -match '^[a-zA-Z]') {
                        $resourcesEnd = $j - 1
                        break
                    }
                }
                if ($resourcesEnd -lt 0) { $resourcesEnd = $beforeSteps.Count - 1 }
                break
            }
        }

        # Content before resources block
        if ($resourcesStart -gt 0) {
            $newLines += $beforeSteps[0..($resourcesStart - 1)]
        }

        # New resources block
        $newLines += "resources:"
        $newLines += "  repositories:"
        $newLines += "  - repository: AppStoreApplicationsRepo"
        $newLines += "    type: git"
        $newLines += "    name: AppStoreApplications/AppStoreApplications"
        $newLines += "    ref: 'Studio_2026'"
        $newLines += ""

        # Content after old resources block
        if ($resourcesEnd -lt ($beforeSteps.Count - 1)) {
            $nextIdx = $resourcesEnd + 1
            while ($nextIdx -lt $beforeSteps.Count -and [string]::IsNullOrWhiteSpace($beforeSteps[$nextIdx])) {
                $nextIdx++
            }
            if ($nextIdx -lt $beforeSteps.Count) {
                $newLines += $beforeSteps[$nextIdx..($beforeSteps.Count - 1)]
            }
        }
    }

    # Add new steps block
    $newLines += "steps:"
    $newLines += "  - template: build/jobs/build-sdlplugin.yml@AppStoreApplicationsRepo"
    $newLines += "  - template: build/jobs/publish-build-artifact-task.yml@AppStoreApplicationsRepo"
    $newLines += ""

    # Write using UTF8 no BOM
    $utf8NoBom = New-Object System.Text.UTF8Encoding $false
    [System.IO.File]::WriteAllLines($file.FullName, $newLines, $utf8NoBom)
    $count++
    Write-Host "Updated: $($file.Name)"
}

Write-Host ""
if ($skipped.Count -gt 0) {
    Write-Host "SKIPPED (no 'steps:' found): $($skipped -join ', ')"
}
Write-Host "Total files updated: $count"
