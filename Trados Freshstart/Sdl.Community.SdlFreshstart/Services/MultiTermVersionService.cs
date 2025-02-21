using Microsoft.Win32;
using Sdl.Community.SdlFreshstart.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Sdl.Community.SdlFreshstart.Services
{
    public class MultiTermVersionService
    {
        private const string InstallLocation64Bit = @"SOFTWARE\Wow6432Node";
        private const string InstallLocation32Bit = @"SOFTWARE";

        private readonly Dictionary<string, string> _supportedMultiTermVersions = new Dictionary<string, string>
        {
            {"MTCore14", "SDL MultiTerm 2017"},
            {"MTCore15", "SDL MultiTerm 2019"},
            {"MTCore16", "SDL MultiTerm 2021"},
            {"MTCore17", "SDL MultiTerm 2022"},
            {"MTCore18", "SDL MultiTerm 2024"}
        };

        private readonly List<MultiTermInstalledVersion> _installedMultiTermVersions;

        public MultiTermVersionService()
        {
            _installedMultiTermVersions = new List<MultiTermInstalledVersion>();
            Initialize();
        }

        private void Initialize()
        {
            var node = Environment.Is64BitOperatingSystem ? InstallLocation64Bit : InstallLocation32Bit;
            var sdlRegistryKey = Registry.LocalMachine.OpenSubKey(node);

            if (sdlRegistryKey == null) return;
            var index = 0;
            foreach (var supportedMultiTermVersion in _supportedMultiTermVersions)
            {
                var sdlOrTrados = index < 3 ? "SDL" : "Trados";
                var registryPath = $@"{node}\{sdlOrTrados}\";
                FindAndCreateMultiTermVersion(registryPath, supportedMultiTermVersion.Key, supportedMultiTermVersion.Value);
                index++;
            }
        }

        private void FindAndCreateMultiTermVersion(string registryPath, string multiTermVersion, string multiTermPublicVersion)
        {
            var multiTermKey = Registry.LocalMachine.OpenSubKey($@"{registryPath}\{multiTermVersion}\{"Installer"}\{"PersistedProperties"}");
            if (multiTermKey != null)
            {
                CreateMultiTermVersion(multiTermKey, multiTermVersion, multiTermPublicVersion);
            }
        }

        private void CreateMultiTermVersion(RegistryKey multiTermKey, string version, string publicVersion)
        {
            var installLocation = multiTermKey.GetValue("CoreINSTALLDIR").ToString();
            var fullVersion = GetMultiTermFullVersion(installLocation);

            _installedMultiTermVersions.Add(new MultiTermInstalledVersion
            {
                Version = version,
                PublicVersion = publicVersion,
                InstallPath = installLocation,
                ExecutableVersion = new Version(fullVersion)
            });
        }

        private static string GetMultiTermFullVersion(string installLocation)
        {
            var assembly = Assembly.LoadFile($@"{installLocation}\{"MultiTerm.exe"}");
            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var fullVersion = versionInfo.FileVersion;
            return fullVersion;
        }

        public List<MultiTermInstalledVersion> GetInstalledMultiTermVersions()
        {
            return _installedMultiTermVersions;
        }

        public MultiTermInstalledVersion GetMultiTermVersion()
        {
            var assembly = Assembly.LoadFile($@"{AppDomain.CurrentDomain.BaseDirectory}\{"MultiTerm.exe"}");
            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var currentVersion = new Version(versionInfo.FileVersion);
            var installedMultiTermVersion = _installedMultiTermVersions.Find(x => x.ExecutableVersion.MajorRevision.Equals(currentVersion.MajorRevision));

            var multiTermVersion = new MultiTermInstalledVersion
            {
                InstallPath = assembly.Location,
                Version = installedMultiTermVersion.Version,
                PublicVersion = installedMultiTermVersion.PublicVersion,
                ExecutableVersion = currentVersion
            };

            return multiTermVersion;
        }
    }
}
