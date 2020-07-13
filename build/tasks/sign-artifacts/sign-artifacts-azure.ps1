param(
[string]$sigingListFile = "$psscriptroot\filestosign.csv",
[string]$signingRoot = "$PSScriptRoot\..\..\..\..",
[string]$KeyVaultUrl = "https://sdl-lt-keyvault.vault.azure.net/",
[string]$TimestampServer="http://timestamp.digicert.com",
[string]$CertificateName="SDLPLCAuthenticode"
)

$SigningClientId = "unset"
$SigningSecret = "unset"

if ($null -ne $env:KeyVaultUrl)
{
    $KeyVaultUrl = $env:KeyVaultUrl
}

if ($null -ne $env:CurrentCertificateName)
{
    $CertificateName = $env:CurrentCertificateName
}

if ($env:SigningSecret -ne $null)
{
    $SigningSecret = $env:SigningSecret
}

if ($env:SigningClientId -ne $null)
{
    $SigningClientId = $env:SigningClientId
}

#files to sign are now in a csv to keep the code generic -Header "Description", "Path"
$FilesToSign = import-csv -Path $sigingListFile


$buildRoot="$psscriptroot\..\..\..\.."
$SrcRoot="$psscriptroot\..\..\.."

$ToolPath=[System.IO.Path]::GetFullPath("$SrcRoot\tools\azuresigntool.exe");
$signingRoot=[System.IO.Path]::GetFullPath("$signingRoot");

function AuthenticodeSign
{
 param( [string]$path, [string]$description)
	
 attrib -r "$path" 
 & "$ToolPath" sign -kvu "$KeyVaultUrl" -kvi "$SigningClientId" -kvs "$SigningSecret" -kvc "$CertificateName" -d "$description" -tr "$TimestampServer" -td sha512 -fd sha512 -v "$path"
}

foreach ($item in $FilesToSign)
{
    $path=[System.IO.Path]::GetFullPath($signingRoot + $item.Path);
    AuthenticodeSign "$path" $item.Description
}
