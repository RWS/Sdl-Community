param(
[string]$fileToSign = "$PSScriptRoot\signme.exe",
[string]$SigningDescription = "An Example Installer",
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


$SrcRoot="$psscriptroot\..\..\.."
$ToolPath=[System.IO.Path]::GetFullPath("$SrcRoot\tools\azuresigntool.exe");


#Write-Debug "$ToolPath sign -kvu $KeyVaultUrl -kvi $SigningClientId -kvs $SigningSecret -kvc $CertificateName -d $SigningDescription -tr $TimestampServer -td sha512 -fd sha512 -v $fileToSign"
attrib -r "$fileToSign" 

& "$ToolPath" sign -kvu "$KeyVaultUrl" -kvi "$SigningClientId" -kvs "$SigningSecret" -kvc "$CertificateName" -d "$SigningDescription" -tr "$TimestampServer" -td sha512 -fd sha512 -v "$fileToSign"


