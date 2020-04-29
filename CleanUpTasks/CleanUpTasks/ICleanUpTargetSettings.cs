using System.Diagnostics.Contracts;
using SDLCommunityCleanUpTasks.Contracts;

namespace SDLCommunityCleanUpTasks
{
	[ContractClass(typeof(ICleanUpTargetSettingsContract))]
    public interface ICleanUpTargetSettings : ICleanUpConversionSettings
    {
        string BackupsSaveFolder { get; set; }
        bool MakeBackups { get; set; }
        bool OverwriteSdlXliff { get; set; }
        string SaveFolder { get; set; }
        bool SaveTarget { get; set; }
    }
}