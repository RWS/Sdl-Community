using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;
using Sdl.Community.StudioMigrationUtility.Model;

namespace Sdl.Community.StudioMigrationUtility.Services
{
    public class StudioVersionService
    {
        private const string InstallLocation64Bit = @"SOFTWARE\Wow6432Node\SDL\";
        private const string InstallLocation32Bit = @"SOFTWARE\SDL";

        private readonly Dictionary<string, string> _supportedStudioVersions = new Dictionary<string, string>
        {
            {"Studio2", "Studio 2011"},
            {"Studio3", "Studio 2014"},
            {"Studio4", "Studio 2015"},
            {"Studio5", "Studio 2017"},
			{"Studio15","Studio 2019" }
        };
        private readonly List<StudioVersion> _installedStudioVersions;

        public StudioVersionService()
        {
            _installedStudioVersions = new List<StudioVersion>();
            Initialize();
        }
        public List<StudioVersion> GetInstalledStudioVersions()
        {
            return _installedStudioVersions;
        }

        private void Initialize()
        {
            var registryPath = Environment.Is64BitOperatingSystem ? InstallLocation64Bit : InstallLocation32Bit;
            var sdlRegistryKey = Registry.LocalMachine.OpenSubKey(registryPath);

            if (sdlRegistryKey == null) return;
            foreach (var supportedStudioVersion in _supportedStudioVersions)
            {
                FindAndCreateStudioVersion(registryPath, supportedStudioVersion.Key, supportedStudioVersion.Value);
            }

        }

        private void FindAndCreateStudioVersion(string registryPath, string studioVersion, string studioPublicVersion)
        {
            var studioKey = Registry.LocalMachine.OpenSubKey(string.Format(@"{0}\{1}", registryPath, studioVersion));
            if (studioKey != null)
            {
                CreateStudioVersion(studioKey, studioVersion, studioPublicVersion);
            }
        }

        private void CreateStudioVersion(RegistryKey studioKey, string version, string publicVersion)
        {
            var installLocation = studioKey.GetValue("InstallLocation").ToString();
            var fullVersion = GetStudioFullVersion(installLocation);


            _installedStudioVersions.Add(new StudioVersion()
            {
                Version = version,
                PublicVersion = publicVersion,
                InstallPath = installLocation,
                ExecutableVersion = new Version(fullVersion)
            });
        }

        private static string GetStudioFullVersion(string installLocation)
        {
            var assembly = Assembly.LoadFile(string.Format(@"{0}\{1}", installLocation, "SDLTradosStudio.exe"));
            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var fullVersion = versionInfo.FileVersion;
            return fullVersion;
        }


    }
}
