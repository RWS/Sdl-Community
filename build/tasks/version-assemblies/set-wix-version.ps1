param(
[string]$ProductVersion="16.0.0.0",
[string]$ProductIdentifier="StudioLocal",
[string]$FullProductName="SDL Trados Studio $Env:Computername",
[string]$InstallerDisplayName="SDL Trados Studio $Env:Computername",
[string]$WixFile="$psscriptroot\..\..\..\Deployment\Wix\StudioDefines.wxi"
)



$WixFile=[System.IO.Path]::GetFullPath($WixFile);
$XmlContents = New-Object -TypeName XML


$vers=[Version]::new($ProductVersion)

#$vers.Major is the first part of the version type in which we store the major verson number
#$vers.Build is the third part of the version type in which we store the patch/cu number
#$vers.Revision is the fourth part of the version type in which we store the build number

$InstallerVersion="$($vers.Major).$($vers.Minor).$($vers.Revision)"

write-output "InstallerVersion=$InstallerVersion"
write-output "ProductIdentifier=$ProductIdentifier"


if (Test-Path $WixFile)
{
    $XmlContents.Load($WixFile)
    
    $XMLDefines = Select-Xml -xpath "//processing-instruction('define')" -xml $XmlContents

    foreach ($define in $XMLDefines )
    {
        if ($define.Node.Value -like "*productVersion*")
        {
            #Msi version
            $define.Node.Value = "productVersion = `"$InstallerVersion`""
        }

        if ($define.Node.Value -like "*versionedName*")
        {
            #ProductIdentifier e.g. studio15
            $define.Node.Value = "versionedName = `"$ProductIdentifier`""
        }

        if ($define.Node.Value -clike "*displayName*")
        {
            if (-not ([string]::IsNullOrEmpty($InstallerDisplayName)))
            {
                $define.Node.Value = "displayName = `"$InstallerDisplayName`""
            }
        }
	
        if ($define.Node.Value -like "*unversionedDisplayName*")
        {
            if (-not ([string]::IsNullOrEmpty($FullProductName)))
            {
                $define.Node.Value = "unversionedDisplayName = `"$FullProductName`""
            }
        }
        
    }
    $XmlContents.Save("$WixFile")
}
else
{
    write-output "$WixFile is not found"
    exit
}