using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sdl.Community.SdlFreshstart.Model;

namespace Sdl.Community.SdlFreshstart.Helpers
{
    public static class AppDataFolder
	{
		private static string _backupFolderPath =
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL", "StudioCleanup");

		public static List<LocationDetails> GetRoamingMajorFolderPath(string userName, StudioLocationListItem selectedLocation,
			List<StudioVersionListItem> studioVersions)
		{
			var studioDetails = new List<LocationDetails>();
			foreach (var studioVersion in studioVersions)
			{
				var majorFolderPath = string.Format(@"C:\Users\{0}\AppData\Roaming\SDL\SDL Trados Studio\{1}", userName,
					studioVersion.MajorVersionNumber);
				var directoryInfo = new DirectoryInfo(majorFolderPath);
				var details = new LocationDetails
				{
					OriginalFilePath = majorFolderPath,
					BackupFilePath = Path.Combine(_backupFolderPath, studioVersion.DisplayName, directoryInfo.Name),
					Alias = selectedLocation.Alias,
					Version = studioVersion.DisplayName
				};
				studioDetails.Add(details);
			}
			return studioDetails;
		}

		// Get the last ProjectApi folder version (it might be the case when user has 2 folders for Studio 2019: 15.1.0.0 and 15.2.0.0)
		public static string GetProjectApiPath(string studioMajVersion)
		{
			var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			var projectApiPath = $@"{appDataPath}\SDL\ProjectApi";

			var directoryInfo = new DirectoryInfo(projectApiPath)
				.GetDirectories("*", SearchOption.AllDirectories)
				.Where(d => d.Name.StartsWith(studioMajVersion))
				.OrderByDescending(d => d.LastWriteTimeUtc)?.FirstOrDefault();

			return directoryInfo?.Name;
		}

		public static List<LocationDetails> GetRoamingMajorFullFolderPath(string userName,
			StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
		{
			var studioDetails = new List<LocationDetails>();
			foreach (var studioVersion in studioVersions)
			{
				var majorFolderPath = string.Format(@"C:\Users\{0}\AppData\Roaming\SDL\SDL Trados Studio\{1}.0.0.0", userName,
					studioVersion.MajorVersionNumber);
				var directoryInfo = new DirectoryInfo(majorFolderPath);
				var details = new LocationDetails
				{
					OriginalFilePath = majorFolderPath,
					BackupFilePath = Path.Combine(_backupFolderPath, studioVersion.DisplayName, directoryInfo.Name),
					Alias = selectedLocation.Alias,
					Version = studioVersion.DisplayName
				};
				studioDetails.Add(details);
			}
			return studioDetails;
		}

		public static List<LocationDetails> GetRoamingProjectApiFolderPath(string userName,
			StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
		{
			var studioDetails = new List<LocationDetails>();
			foreach (var studioVersion in studioVersions)
			{
				var projApiFolder = GetProjectApiPath(studioVersion.MajorVersionNumber);
				if (projApiFolder != null)
				{
					var projApiPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\SDL\ProjectApi\{projApiFolder}";
					var directoryInfo = new DirectoryInfo(projApiPath);
					var details = new LocationDetails
					{
						OriginalFilePath = projApiPath,
						BackupFilePath = Path.Combine(_backupFolderPath, studioVersion.DisplayName, "ProjectApi", directoryInfo.Name),
						Alias = selectedLocation.Alias,
						Version = studioVersion.DisplayName
					};
					studioDetails.Add(details);
				}
			}
			return studioDetails;
		}

		public static List<LocationDetails> GetLocalMajorFullFolderPath(string userName,
			StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
		{
			var studioDetails = new List<LocationDetails>();
			foreach (var studioVersion in studioVersions)
			{
				var majorFolderPath = string.Format(@"C:\Users\{0}\AppData\Local\SDL\SDL Trados Studio\{1}.0.0.0", userName,
					studioVersion.MajorVersionNumber);
				var directoryInfo = new DirectoryInfo(majorFolderPath);
				var details = new LocationDetails
				{
					OriginalFilePath = majorFolderPath,
					BackupFilePath = Path.Combine(_backupFolderPath, studioVersion.DisplayName, "SDL Trados Studio", directoryInfo.Name),
					Alias = selectedLocation.Alias,
					Version = studioVersion.DisplayName
				};
				studioDetails.Add(details);
			}
			return studioDetails;
		}

		public static List<LocationDetails> GetLocalMajorFolderPath(string userName, StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
	    {
		    var studioDetails = new List<LocationDetails>();
			foreach (var studioVersion in studioVersions)
		    {
			    var majorFolderPath = string.Format(@"C:\Users\{0}\AppData\Local\SDL\SDL Trados Studio\{1}", userName,
				    studioVersion.MajorVersionNumber);
				var directoryInfo = new DirectoryInfo(majorFolderPath);
			    var details = new LocationDetails
			    {
				    OriginalFilePath = majorFolderPath,
				    BackupFilePath = Path.Combine(_backupFolderPath, studioVersion.DisplayName, "SDL Trados Studio", directoryInfo.Name),
				    Alias = selectedLocation.Alias,
				    Version = studioVersion.DisplayName
				};
			    studioDetails.Add(details);
			}
		    return studioDetails;
		}
    }
}
