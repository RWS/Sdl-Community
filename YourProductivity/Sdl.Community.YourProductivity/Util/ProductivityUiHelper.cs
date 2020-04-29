using System;
using System.Diagnostics;
using System.Reflection;

namespace Sdl.Community.YourProductivity.Util
{
	public static class ProductivityUiHelper
    {
        public static bool IsGreaterThanCu10()
        {
            var minimumVersion = new Version("11.2.4409.10");
            var assembly = Assembly.LoadFile(string.Format(@"{0}\{1}", AppDomain.CurrentDomain.BaseDirectory, "SDLTradosStudio.exe"));
            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var currentVersion = new Version(versionInfo.FileVersion);
            return currentVersion.CompareTo(minimumVersion) >= 0;
        }
    }
}