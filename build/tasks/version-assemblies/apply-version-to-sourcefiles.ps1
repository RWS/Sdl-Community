##-----------------------------------------------------------------------
## <copyright file="ApplyVersionToAssemblies.ps1">(c) Microsoft Corporation. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
##-----------------------------------------------------------------------
# Look for a 0.0.0.0 pattern in the build number. 
# If found use it to version the assemblies.
#
# For example, if the 'Build number format' build process parameter 
# $(BuildDefinitionName)_$(Year:yyyy).$(Month).$(DayOfMonth)$(Rev:.r)
# then your build numbers come out like this:
# "Build HelloWorld_2013.07.19.1"
# This script would then apply version 2013.07.19.1 to your assemblies.


[CmdletBinding()]
param (

    [Parameter(Mandatory)]
    [String]$Path,

    [Parameter(Mandatory)]
    [string]$VersionNumber,

    $FilenamePattern,

    $Field,

    $productId="Studio17",

    $outputversion="ProductVersion",

    $VersionRegex = "\d+\.\d+\.\d+\.\d+"
)

$DebugPreference ='Continue' 

$ThisYear=Get-Date -Format "yyyy"
$Copyright="Copyright © $ThisYear SDL. All rights reserved."


if ($env:Build_SourceBranch -like "*release*")
{
    $AssemblyConfig = ""
}
else
{
    if ($null -ne $env:Build_SourceBranchName)
    {
        $AssemblyConfig = $env:Build_SourceBranchName
    }

    $shorthash=git rev-parse --short HEAD

    if ($null -ne $shorthash)
    {
        $AssemblyConfig = $AssemblyConfig + " " + $shorthash
    }
    $AssemblyConfig = $AssemblyConfig.trim()
}


$ProductName="SDL Trados Studio"


# Make sure path to source code directory is available
if (-not (Test-Path $Path))
{
    Write-Error "Source directory does not exist: $Path"
    exit 1
}
Write-Debug "Source Directory: $Path"
Write-Debug "Filename Pattern: $FilenamePattern"
Write-Debug "Version Number/Build Number: $VersionNumber"
Write-Debug "Version Filter to extract build number: $VersionRegex"
Write-Debug "Field to update (all if empty): $Field"
Write-Debug "Copyright: $Copyright"
Write-Debug "Output: Version Number Parameter Name: $outputversion"
Write-Debug "AssemblyConfig: $AssemblyConfig"
Write-Debug "ProductName: $ProductName"
Write-Debug "productId: $productId"






#dot source function for getting the file encoding.
. "$psscriptroot\..\Get-FileEncoding\Get-FileEncoding.ps1"

# Get and validate the version data
$VersionData = [regex]::matches($VersionNumber,$VersionRegex)
switch($VersionData.Count)
{
   0        
      { 
         Write-Error "Could not find version number data in $VersionNumber."
         exit 1
      }
   1 {}
   default 
      { 
         Write-Warning "Found more than instance of version data in $VersionNumber." 
         Write-Warning "Will assume first instance is version."
      }
}
$NewVersion = $VersionData[0]
Write-Debug "Extracted Version: $NewVersion"

# Apply the version to the assembly property files
# $files = Get-ChildItem $Path -recurse -include "*Properties*","My Project" | 
    # Where-Object { $_.PSIsContainer } | 
    # foreach { Get-ChildItem -Path $_.FullName -Recurse -include $FilenamePattern }

$files =  Get-ChildItem -Path $Path -Recurse -Include @($FilenamePattern.Split(','))
	
if($files)
{
    Write-Debug "Will apply $NewVersion to $($files.count) files."
	$fields = $field.Split(";")
    foreach ($file in $files) {
        $FileEncoding = Get-FileEncoding -Path $File.FullName
        $filecontent = Get-Content -Path $file.Fullname
        $extension = [System.IO.Path]::GetExtension($file)
        attrib $file -r
        if ([string]::IsNullOrEmpty($field))
        {
            Write-Debug "Updating all version fields"
            $filecontent -replace $VersionRegex, $NewVersion | Out-File $file -Encoding $FileEncoding
        } 
		else 
		{
            $newContent = $filecontent
			foreach ($attr in $fields)
			{
                Write-Debug "Updating only the '$attr' version"
                if($extension -eq ".cs" -or $extension -eq ".cpp")
                {
                    $newContent = $newContent -replace "$attr\(`"$VersionRegex", "$attr(`"$NewVersion" 
                    $newContent = $newContent -replace "AssemblyCopyright\(`".*`"", "AssemblyCopyright(`"$Copyright`""
                    $newContent = $newContent -replace "AssemblyConfiguration\(`".*`"", "AssemblyConfiguration(`"$AssemblyConfig`""
                    $newContent = $newContent -replace "AssemblyInformationalVersion\(`".*`"", "AssemblyInformationalVersion(`"$productId`""

                    $newContent = $newContent -replace "AssemblyVersionAttribute\(`".*`"", "AssemblyVersionAttribute(`"$NewVersion`""
                    $newContent = $newContent -replace "AssemblyFileVersionAttribute\(`".*`"", "AssemblyFileVersionAttribute(`"$NewVersion`""

                    $newContent = $newContent -replace "System.String AssemblyVersion = `".*`"", "System.String AssemblyVersion = `"$NewVersion`""
                    $newContent = $newContent -replace "System.String AssemblyFileVersion = `".*`"", "System.String AssemblyFileVersion = `"$NewVersion`""
                    
                }
                if($extension -eq ".rc")
                {
                    $CPPVersionRegex = $VersionRegex -replace "\." , ","
                    $CPPVersion = $NewVersion -replace "\." , ","
                    $CPPCopyright=$Copyright -replace "©" , "\xa9"
                    $CPPAttr = $attr.ToUpper()
                    $newContent = $newContent -replace "$CPPAttr $CPPVersionRegex", "$CPPAttr $CPPVersion"
                    $newContent = $newContent -replace "`"$attr`", `"$CPPVersionRegex", "`"$attr`", `"$CPPVersion"
                    $newContent = $newContent -replace "`"$attr`", `"$VersionRegex", "`"$attr`", `"$ApiVersion"
                    $newContent = $newContent -replace "`"LegalCopyright`", `".*`"", "`"LegalCopyright`", `"$CPPCopyright`""
                }
                
	        }
            $newContent | Out-File $file -Encoding $FileEncoding
        }
        
        Write-Debug "$file - version applied"
    }
    Write-Debug "Set the output variable '$outputversion' with the value $NewVersion"
    Write-Host "##vso[task.setvariable variable=$outputversion;]$NewVersion"
}
else
{
    Write-Warning "Found no files."
}
