using System;
using System.IO;

namespace Sdl.Community.PostEdit.Versions
{
    public class Constants
    {
        public static readonly string PostEditCompareBackupFolder =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Trados AppStore",
                "PostEdit.Compare", "Backup");

        public static readonly string PostEditCompareSettingsFolder =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Trados AppStore",
                "PostEdit.Compare");

        public static readonly string PostEditCompareDefaultReportsFolder =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PostEdit.Compare",
                "Reports");
    }
}