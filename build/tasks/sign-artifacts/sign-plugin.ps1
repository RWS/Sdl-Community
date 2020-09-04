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

write-output "load plugin framework from nuget cache"

$Plugins = Resolve-Path -Path "$env:userprofile\.nuget\packages\sdl.core.pluginframework\*\lib\net45\Sdl.Core.PluginFramework.PackageSupport.dll" | Sort-Object Path –Descending

write-output "plugin assemblies found:" + $Plugins.Count

write-output $Plugins
write-output ""

$pluginpath = $Plugins[0].Path

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
