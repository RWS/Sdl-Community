using System;
using System.Diagnostics;
using System.Reflection;

namespace Sdl.Community.YourProductivity.Services
{
	public class VersioningService
    {
        public string PluginVersion { get; set; }
        public VersioningService()
        {
            Initialize();
        }

        private void Initialize()
        {
            PluginVersion = GetPluginVersion();
        }
		       
        public static string GetPluginVersion()
        {
            var assembly = typeof(ShareService).Assembly;
            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var fullVersion = versionInfo.FileVersion;
            return fullVersion;
        }

        public Version GetStudioVersion()
        {
            var currentDomain = AppDomain.CurrentDomain;
            var assembly = Assembly.LoadFile(string.Format(@"{0}\{1}", currentDomain.BaseDirectory, "SDLTradosStudio.exe"));
            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return new Version(versionInfo.FileVersion);
        }

        public string GetStudioVersionFriendly()
        {
            var version = GetStudioVersion();
            var result = "studio2017";
            if (version.Major == 11) result = "studio2014";
            if (version.Major == 12) result = "studio2015";

            return result;
        }
    }
}
