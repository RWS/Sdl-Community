using System.Diagnostics.Contracts;
using Sdl.Community.CleanUpTasks.Contracts;

namespace Sdl.Community.CleanUpTasks
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