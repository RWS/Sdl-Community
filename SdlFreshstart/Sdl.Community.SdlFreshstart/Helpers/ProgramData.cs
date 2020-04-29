using System;
using System.Collections.Generic;
using System.IO;
using Sdl.Community.SdlFreshstart.Model;

namespace Sdl.Community.SdlFreshstart.Helpers
{
	public static class ProgramData
	{
		private static string _backupFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL", "StudioCleanup");
		public static readonly Log Log = Log.Instance;

		public static List<LocationDetails> GetProgramDataMajorFolderPath(StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
		{
			var studioDetails = new List<LocationDetails>();
			try
			{
				foreach (var studioVersion in studioVersions)
				{
					var majorFolderPath = string.Format(@"C:\ProgramData\SDL\SDL Trados Studio\{0}", studioVersion.MajorVersionNumber);
					var directoryInfo = new DirectoryInfo(majorFolderPath);
					var details = new LocationDetails
					{
						OriginalFilePath = majorFolderPath,
						BackupFilePath = Path.Combine(_backupFolderPath, studioVersion.DisplayName, "ProgramData", directoryInfo.Name),
						Alias = selectedLocation?.Alias,
						Version = studioVersion?.DisplayName
					};
					studioDetails.Add(details);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.GetProgramDataMajorFolderPath} {ex.Message}\n {ex.StackTrace}");
			}
			return studioDetails;
		}

		public static List<LocationDetails> GetProgramDataMajorFullFolderPath(StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
		{
			var studioDetails = new List<LocationDetails>();
			try
			{
				foreach (var studioVersion in studioVersions)
				{
					var majorFolderPath = string.Format(@"C:\ProgramData\SDL\SDL Trados Studio\{0}.0.0.0", studioVersion.MajorVersionNumber);
					var directoryInfo = new DirectoryInfo(majorFolderPath);
					var details = new LocationDetails
					{
						OriginalFilePath = majorFolderPath,
						BackupFilePath = Path.Combine(_backupFolderPath, studioVersion.DisplayName, "ProgramData", directoryInfo.Name),
						Alias = selectedLocation?.Alias,
						Version = studioVersion?.DisplayName
					};
					studioDetails.Add(details);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.GetProgramDataMajorFullFolderPath} {ex.Message}\n {ex.StackTrace}");
			}
			return studioDetails;
		}

		public static List<LocationDetails> GetProgramDataFolderPath(StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
		{
			var studioDetails = new List<LocationDetails>();
			try
			{
				foreach (var studioVersion in studioVersions)
				{
					var programDataFolderPath = string.Format(@"C:\ProgramData\SDL\SDL Trados Studio\{0}\Updates", studioVersion.FolderName);
					var directoryInfo = new DirectoryInfo(programDataFolderPath);
					var details = new LocationDetails
					{
						OriginalFilePath = programDataFolderPath,
						BackupFilePath = Path.Combine(_backupFolderPath, studioVersion.DisplayName, "ProgramData", directoryInfo.Name),
						Alias = selectedLocation?.Alias,
						Version = studioVersion?.DisplayName
					};
					studioDetails.Add(details);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.GetProgramDataFolderPath} {ex.Message}\n {ex.StackTrace}");
			}
			return studioDetails;
		}
	}
}