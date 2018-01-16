using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.StudioCleanupTool.Model;

namespace Sdl.Community.StudioCleanupTool.Helpers
{
    public static class AppDataFolder
	{
		private static string _backupFolderPath =
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL", "StudioCleanup");

		public static List<StudioDetails> GetRoamingMajorFolderPath(string userName, StudioLocationListItem selectedLocation,
			List<StudioVersionListItem> studioVersions)
		{
			var studioDetails = new List<StudioDetails>();
			foreach (var studioVersion in studioVersions)
			{
				var majorFolderPath = string.Format(@"C:\Users\{0}\AppData\Roaming\SDL\SDL Trados Studio\{1}", userName,
					studioVersion.MajorVersionNumber);
				var directoryInfo = new DirectoryInfo(majorFolderPath);
				var details = new StudioDetails
				{
					OriginalFilePath = majorFolderPath,
					BackupFilePath = Path.Combine(_backupFolderPath, studioVersion.DisplayName, directoryInfo.Name),
					Alias = selectedLocation.Alias,
					StudioVersion = studioVersion.DisplayName
				};
				studioDetails.Add(details);
			}
			return studioDetails;
		}

		public static List<StudioDetails> GetRoamingMajorFullFolderPath(string userName,
			StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
		{
			var studioDetails = new List<StudioDetails>();
			foreach (var studioVersion in studioVersions)
			{
				var majorFolderPath = string.Format(@"C:\Users\{0}\AppData\Roaming\SDL\SDL Trados Studio\{1}.0.0.0", userName,
					studioVersion.MajorVersionNumber);
				var directoryInfo = new DirectoryInfo(majorFolderPath);
				var details = new StudioDetails
				{
					OriginalFilePath = majorFolderPath,
					BackupFilePath = Path.Combine(_backupFolderPath, studioVersion.DisplayName, directoryInfo.Name),
					Alias = selectedLocation.Alias,
					StudioVersion = studioVersion.DisplayName
				};
				studioDetails.Add(details);
			}
			return studioDetails;
		}

		public static List<StudioDetails> GetRoamingProjectApiFolderPath(string userName,
			StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
		{
			var studioDetails = new List<StudioDetails>();
			foreach (var studioVersion in studioVersions)
			{
				var projectApiPath = string.Format(@"C:\Users\{0}\AppData\Roaming\SDL\ProjectApi\{1}.0.0.0", userName,
					studioVersion.MajorVersionNumber);
				var directoryInfo = new DirectoryInfo(projectApiPath);
				var details = new StudioDetails
				{
					OriginalFilePath = projectApiPath,
					BackupFilePath = Path.Combine(_backupFolderPath, studioVersion.DisplayName, directoryInfo.Name),
					Alias = selectedLocation.Alias,
					StudioVersion = studioVersion.DisplayName
				};
				studioDetails.Add(details);
			}
			return studioDetails;
		}

		public static List<StudioDetails> GetLocalMajorFullFolderPath(string userName,
			StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
		{
			var studioDetails = new List<StudioDetails>();
			foreach (var studioVersion in studioVersions)
			{
				var majorFolderPath = string.Format(@"C:\Users\{0}\AppData\Local\SDL\SDL Trados Studio\{1}.0.0.0", userName,
					studioVersion.MajorVersionNumber);
				var directoryInfo = new DirectoryInfo(majorFolderPath);
				var details = new StudioDetails
				{
					OriginalFilePath = majorFolderPath,
					BackupFilePath = Path.Combine(_backupFolderPath, studioVersion.DisplayName, directoryInfo.Name),
					Alias = selectedLocation.Alias,
					StudioVersion = studioVersion.DisplayName
				};
				studioDetails.Add(details);
			}
			return studioDetails;
		}

		public static List<StudioDetails> GetLocalMajorFolderPath(string userName, StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
	    {
		    var studioDetails = new List<StudioDetails>();
			foreach (var studioVersion in studioVersions)
		    {
			    var majorFolderPath = string.Format(@"C:\Users\{0}\AppData\Local\SDL\SDL Trados Studio\{1}", userName,
				    studioVersion.MajorVersionNumber);
				var directoryInfo = new DirectoryInfo(majorFolderPath);
			    var details = new StudioDetails
			    {
				    OriginalFilePath = majorFolderPath,
				    BackupFilePath = Path.Combine(_backupFolderPath, studioVersion.DisplayName, directoryInfo.Name),
				    Alias = selectedLocation.Alias,
				    StudioVersion = studioVersion.DisplayName
				};
			    studioDetails.Add(details);
			}
		    return studioDetails;
		}
    }
}
