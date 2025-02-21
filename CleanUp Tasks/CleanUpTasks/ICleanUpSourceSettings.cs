using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using SDLCommunityCleanUpTasks.Contracts;
using SDLCommunityCleanUpTasks.Models;

namespace SDLCommunityCleanUpTasks
{
	[ContractClass(typeof(ICleanUpSourceSettingsContract))]
    public interface ICleanUpSourceSettings : ICleanUpConversionSettings
    {
        Dictionary<string, bool> FormatTagList { get; set; }
        List<Placeholder> Placeholders { get; set; }
        Dictionary<string, bool> PlaceholderTagList { get; set; }
        BindingList<SegmentLockItem> SegmentLockList { get; set; }
        List<ContextDef> StructureLockList { get; set; }
        bool UseContentLocker { get; set; }
        bool UseSegmentLocker { get; set; }
        bool UseStructureLocker { get; set; }
        bool UseTagCleaner { get; set; }
    }
}