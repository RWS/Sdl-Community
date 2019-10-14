using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Sdl.Community.SdlFreshstart.Model;
using Sdl.Community.Toolkit.Core;

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

		private static string GetProjectApiFilePath(string studioInstallationPath)
		{
			var applicationVersion = GetApplicationVersion(studioInstallationPath);
			if (applicationVersion == null)
			{
				return null;
			}
			var applicationDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			var projectApiFolder = Path.Combine(applicationDataFolder, Path.Combine("SDL", "ProjectApi"));
			var projectApiFile = Path.Combine(projectApiFolder, applicationVersion, "Sdl.ProjectApi.xml");

			return File.Exists(projectApiFile) ? projectApiFile : null;
		}

		private static string GetApplicationVersion(string studioInstallationPath)
		{
			var assemblyFile = Path.Combine(studioInstallationPath, "Sdl.ProjectApi.dll");
			return File.Exists(assemblyFile) ? AssemblyName.GetAssemblyName(assemblyFile).Version.ToString() : null;
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
			var studioInstalledPaths = new Studio()?.GetInstalledStudioVersion();
			foreach (var studioVersion in studioVersions)
			{
				foreach (var studioPath in studioInstalledPaths)
				{
					if (studioPath.Version.Equals(studioVersion.FolderName))
					{
						var projApiFullPath = GetProjectApiFilePath(studioPath.InstallPath);
						var projApiFolderPath = Path.GetDirectoryName(projApiFullPath);
						if (!string.IsNullOrEmpty(projApiFolderPath))
						{
							var directoryInfo = new DirectoryInfo(projApiFolderPath);
							var details = new LocationDetails
							{
								OriginalFilePath = projApiFolderPath,
								BackupFilePath = Path.Combine(_backupFolderPath, studioVersion.DisplayName, "ProjectApi", directoryInfo.Name),
								Alias = selectedLocation.Alias,
								Version = studioVersion.DisplayName
							};
							studioDetails.Add(details);
						}
					}
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
