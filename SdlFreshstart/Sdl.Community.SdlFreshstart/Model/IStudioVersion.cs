using System;
using System.ComponentModel;

namespace Sdl.Community.SdlFreshstart.Model
{
	public interface IStudioVersion : INotifyPropertyChanged
	{
		string PublicVersion { get; set; }
		string LocalPluginsFolder { get; set; }
		string LocalTradosLogsFolder { get; set; }
		string RoamingPluginsFolder { get; set; }
		string GeneralSettingsFolder { get; set; }
		string DocumentsPath { get; set; }
		string ProgramDataPluginsFolder { get; set; }
		string ProgramDataLicenseFolder { get; set; }
		string ProgramDataUpdatesFolder { get; set; }
		string ProgramDataProjectTemplatesFolder { get; set; }
		string ProjectApiPath { get; set; }
		Version ExecutableVersion { get; set; }
		int MajorVersion { get; }
		string LegacyVersion { get; }
		bool IsSelected { get; set; }
		string VersionWithEdition { get; }
		string ShortVersion { get; set; }
		string Edition { get; set; }
		string CacheFolderName { get; }
		string ProgramDataPackagePath { get; }
	}
}