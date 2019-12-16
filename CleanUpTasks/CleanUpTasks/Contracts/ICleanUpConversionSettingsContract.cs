using System.Collections.Generic;
using System.Globalization;
using Sdl.Core.Settings;

namespace Sdl.Community.CleanUpTasks.Contracts
{
	internal abstract class ICleanUpConversionSettingsContract : ICleanUpConversionSettings
    {
        public bool ApplyToNonTranslatables { get; set; }

        public Dictionary<string, bool> ConversionFiles { get; set; }

        public string LastFileDirectory { get; set; }

        public ISettingsGroup Settings { get; set; }

        public CultureInfo SourceCulture { get; set; }

        public bool UseConversionSettings { get; set; }
    }
}