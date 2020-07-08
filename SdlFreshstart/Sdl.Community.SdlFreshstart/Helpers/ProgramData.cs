using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sdl.Community.SdlFreshstart.Model;

namespace Sdl.Community.SdlFreshstart.Helpers
{
	public static class ProgramData
	{
		private static readonly string BackupFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL", "StudioCleanup");
		public static readonly Log Log = Log.Instance;

		public static List<LocationDetails> GetProgramDataMajorFolderPath(StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
		{
			var studioDetails = new List<LocationDetails>();
			try
			{
				foreach (var studioVersion in studioVersions)
				{
					var majorFolderPath = $@"C:\ProgramData\SDL\SDL Trados Studio\{studioVersion.MajorVersionNumber}";

					var directoryInfo = new DirectoryInfo(majorFolderPath);
					if (!directoryInfo.Exists) continue;

					var details = new LocationDetails
					{
						OriginalFilePath = majorFolderPath,
						BackupFilePath = Path.Combine(BackupFolderPath, studioVersion.DisplayName, "ProgramData", directoryInfo.Name),
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
		private static int GetNumericVersion(StudioVersionListItem studioVersion)
		{
			var numericVersion = new string(studioVersion.MajorVersionNumber.TakeWhile(char.IsDigit).ToArray());
			int.TryParse(numericVersion, out var majorVersion);
			return majorVersion;
		}

		public static List<LocationDetails> GetProgramDataMajorFullFolderPath(StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
		{
			var studioDetails = new List<LocationDetails>();
			try
			{
				foreach (var studioVersion in studioVersions)
				{
					var majorVersion = GetNumericVersion(studioVersion);
					var majorFolderPath = majorVersion > 15
						? $@"C:\ProgramData\SDL\SDL Trados Studio\Studio{studioVersion.MajorVersionNumber}"
						: $@"C:\ProgramData\SDL\SDL Trados Studio\{studioVersion.MajorVersionNumber}.0.0.0";

					var directoryInfo = new DirectoryInfo(majorFolderPath);
					if (!directoryInfo.Exists) continue;

					var details = new LocationDetails
					{
						OriginalFilePath = majorFolderPath,
						BackupFilePath = Path.Combine(BackupFolderPath, studioVersion.DisplayName, "ProgramData", directoryInfo.Name),
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
					var programDataFolderPath =
						$@"C:\ProgramData\SDL\SDL Trados Studio\{studioVersion.FolderName}\Updates";

					var directoryInfo = new DirectoryInfo(programDataFolderPath);
					if (!directoryInfo.Exists) continue;

					var details = new LocationDetails
					{
						OriginalFilePath = programDataFolderPath,
						BackupFilePath = Path.Combine(BackupFolderPath, studioVersion.DisplayName, "ProgramData", directoryInfo.Name),
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