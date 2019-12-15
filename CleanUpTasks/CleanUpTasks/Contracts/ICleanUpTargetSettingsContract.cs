using System.Collections.Generic;
using System.Globalization;
using Sdl.Core.Settings;

namespace Sdl.Community.CleanUpTasks.Contracts
{
	internal abstract class ICleanUpTargetSettingsContract : ICleanUpTargetSettings
    {
        public bool ApplyToNonTranslatables { get; set; }

        public string BackupsSaveFolder { get; set; }

        public Dictionary<string, bool> ConversionFiles { get; set; }

        public string LastFileDirectory { get; set; }

        public bool MakeBackups { get; set; }

        public bool OverwriteSdlXliff { get; set; }

        public string SaveFolder { get; set; }

        public bool SaveTarget { get; set; }

        public ISettingsGroup Settings { get; set; }

        public CultureInfo SourceCulture { get; set; }

        public bool UseConversionSettings { get; set; }
    }
}