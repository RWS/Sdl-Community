param(
[string]$fileToSign = "$psscriptroot\*.sdlplugin"
)

$SigningSecret = "unset"
$CertFile=[System.IO.Path]::GetFullPath("$PSScriptRoot\OpenXCert.pfx");
$fileToSign=[System.IO.Path]::GetFullPath("$fileToSign");



if ($null -ne $env:OpenXSecret)
{
    $SigningSecret = $env:OpenXSecret
}

if ($null -ne $env:OpenXCert_secureFilePath)
{
    $CertFile = $env:OpenXCert_secureFilePath
}

write-output "CertFile: $CertFile"

attrib -r "$fileToSign"

$pluginpath = "$env:outputpath\Sdl.PluginFramework.PackageSupport.dll"
write-output "trying $pluginpath"

[Reflection.Assembly]::LoadFile($pluginpath)

write-output "open package $fileToSign"
$PluginPackage=[Sdl.Core.PluginFramework.PackageSupport.PluginPackage]::New($fileToSign,[System.IO.FileAccess]"ReadWrite")

if ($null -ne $PluginPackage)
{
    write-output "sign package"
    $PluginPackage.Sign($CertFile, $SigningSecret);

    if ($PluginPackage.IsSigned())
    {
        write-output "Plugin is signed"
    }

    $PluginPackage.dispose()
}
