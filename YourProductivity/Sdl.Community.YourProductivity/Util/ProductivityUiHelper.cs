using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using NLog;
using Sdl.Community.YourProductivity.Services.Persistence;
using Sdl.Community.YourProductivity.UI;

namespace Sdl.Community.YourProductivity.Util
{
    public static class ProductivityUiHelper
    {
        public static bool IsTwitterAccountConfigured(TwitterPersistenceService twitterPersistenceService, Logger logger)
        {
            var isTwitterAccountConfigured = true;
            if (twitterPersistenceService.HasAccountConfigured) return true;
            using (var tForm = new TwitterAccountSetup(twitterPersistenceService))
            {
                var result = tForm.ShowDialog();
                if (result != DialogResult.OK)
                {
                    isTwitterAccountConfigured = false;
                }
            }
            return isTwitterAccountConfigured;
        }

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
