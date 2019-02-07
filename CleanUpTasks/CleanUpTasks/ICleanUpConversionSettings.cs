using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Sdl.Community.CleanUpTasks.Contracts;

namespace Sdl.Community.CleanUpTasks
{
	[ContractClass(typeof(ICleanUpConversionSettingsContract))]
    public interface ICleanUpConversionSettings : ISettings
    {
        Dictionary<string, bool> ConversionFiles { get; set; }
        string LastFileDirectory { get; set; }
        bool UseConversionSettings { get; set; }
        bool ApplyToNonTranslatables { get; set; }
    }
}