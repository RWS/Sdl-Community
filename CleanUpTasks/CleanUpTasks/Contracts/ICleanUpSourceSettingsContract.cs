using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Sdl.Community.CleanUpTasks.Models;
using Sdl.Core.Settings;

namespace Sdl.Community.CleanUpTasks.Contracts
{
	internal abstract class ICleanUpSourceSettingsContract : ICleanUpSourceSettings
    {
        public bool ApplyToNonTranslatables { get; set; }

        public Dictionary<string, bool> ConversionFiles { get; set; }

        public Dictionary<string, bool> FormatTagList { get; set; }

        public string LastFileDirectory { get; set; }

        public List<Placeholder> Placeholders { get; set; }

        public Dictionary<string, bool> PlaceholderTagList { get; set; }

        public BindingList<SegmentLockItem> SegmentLockList { get; set; }

        public ISettingsGroup Settings { get; set; }

        public CultureInfo SourceCulture { get; set; }

        public List<ContextDef> StructureLockList { get; set; }

        public bool UseContentLocker { get; set; }

        public bool UseConversionSettings { get; set; }

        public bool UseSegmentLocker { get; set; }

        public bool UseStructureLocker { get; set; }

        public bool UseTagCleaner { get; set; }
    }
}