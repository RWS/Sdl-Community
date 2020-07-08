using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Sdl.Community.SdlFreshstart.Model;
using Sdl.Community.Toolkit.Core;

namespace Sdl.Community.SdlFreshstart.Helpers
{
	public static class AppDataFolder
	{
		private static readonly string BackupFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL", "StudioCleanup");
		public static readonly Log Log = Log.Instance;

		public static List<LocationDetails> GetRoamingMajorFolderPath(string userName, StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
		{
			var studioDetails = new List<LocationDetails>();
			try
			{
				foreach (var studioVersion in studioVersions)
				{
					var majorFolderPath =
						$@"C:\Users\{userName}\AppData\Roaming\SDL\SDL Trados Studio\{studioVersion.MajorVersionNumber}";

					var directoryInfo = new DirectoryInfo(majorFolderPath);
					if (!directoryInfo.Exists) continue;

					var details = new LocationDetails
					{
						OriginalFilePath = majorFolderPath,
						BackupFilePath = Path.Combine(BackupFolderPath, studioVersion.DisplayName, directoryInfo.Name),
						Alias = selectedLocation.Alias,
						Version = studioVersion.DisplayName
					};
					studioDetails.Add(details);
				}				
			}
			catch(Exception ex)
			{
				Log.Logger.Error($"{Constants.GetRoamingMajorFolderPath} {ex.Message}\n {ex.StackTrace}");
			}
			return studioDetails;
		}

		private static string GetProjectApiFilePath(string studioInstallationPath)
		{
			try
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
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.GetProjectApiFilePath} {ex.Message}\n {ex.StackTrace}");
			}
			return null;
		}

		private static string GetApplicationVersion(string studioInstallationPath)
		{
			try
			{
				var assemblyFile = Path.Combine(studioInstallationPath, "Sdl.ProjectApi.dll");
				return File.Exists(assemblyFile) ? AssemblyName.GetAssemblyName(assemblyFile).Version.ToString() : null;
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.GetApplicationVersion} {ex.Message}\n {ex.StackTrace}");
			}
			return null;
		}

		public static List<LocationDetails> GetRoamingMajorFullFolderPath(string userName, StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
		{
			var studioDetails = new List<LocationDetails>();
			try
			{
				foreach (var studioVersion in studioVersions)
				{
					var majorVersion = GetNumericVersion(studioVersion);
					var majorFolderPath = majorVersion > 15
						? $@"C:\Users\{userName}\AppData\Roaming\SDL\SDL Trados Studio\{studioVersion.FolderName}"
						: $@"C:\Users\{userName}\AppData\Roaming\SDL\SDL Trados Studio\{studioVersion.MajorVersionNumber}.0.0.0";

					var directoryInfo = new DirectoryInfo(majorFolderPath);
					if (!directoryInfo.Exists) continue;

					var details = new LocationDetails
					{
						OriginalFilePath = majorFolderPath,
						BackupFilePath = Path.Combine(BackupFolderPath, studioVersion.DisplayName, directoryInfo.Name),
						Alias = selectedLocation?.Alias,
						Version = studioVersion.DisplayName
					};
					studioDetails.Add(details);
				}
			}
			catch(Exception ex)
			{
				Log.Logger.Error($"{Constants.GetRoamingMajorFullFolderPath} {ex.Message}\n {ex.StackTrace}");
			}
			return studioDetails;
		}

		private static int GetNumericVersion(StudioVersionListItem studioVersion)
		{
			var numericVersion = new string(studioVersion.MajorVersionNumber.TakeWhile(char.IsDigit).ToArray());
			int.TryParse(numericVersion, out var majorVersion);
			return majorVersion;
		}

		public static List<LocationDetails> GetRoamingProjectApiFolderPath(StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
		{			
			var studioDetails = new List<LocationDetails>();
			try
			{
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
								if (!directoryInfo.Exists) continue;

								var details = new LocationDetails
								{
									OriginalFilePath = projApiFolderPath,
									BackupFilePath = Path.Combine(BackupFolderPath, studioVersion.DisplayName, "ProjectApi", directoryInfo.Name),
									Alias = selectedLocation?.Alias,
									Version = studioVersion?.DisplayName
								};
								studioDetails.Add(details);
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				Log.Logger.Error($"{Constants.GetRoamingProjectApiFolderPath} {ex.Message}\n {ex.StackTrace}");
			}
			return studioDetails;
		}

		public static List<LocationDetails> GetLocalMajorFullFolderPath(string userName, StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
		{
			var studioDetails = new List<LocationDetails>();
			try
			{
				foreach (var studioVersion in studioVersions)
				{
					var majorVersion = GetNumericVersion(studioVersion);
					var majorFolderPath = majorVersion > 15 
						? $@"C:\Users\{userName}\AppData\Local\SDL\SDL Trados Studio\{studioVersion.FolderName}"
						: $@"C:\Users\{userName}\AppData\Local\SDL\SDL Trados Studio\{studioVersion.MajorVersionNumber}.0.0.0";

					var directoryInfo = new DirectoryInfo(majorFolderPath);
					if (!directoryInfo.Exists) continue;

					var details = new LocationDetails
					{
						OriginalFilePath = majorFolderPath,
						BackupFilePath = Path.Combine(BackupFolderPath, studioVersion.DisplayName, "SDL Trados Studio", directoryInfo.Name),
						Alias = selectedLocation?.Alias,
						Version = studioVersion?.DisplayName
					};
					studioDetails.Add(details);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.GetLocalMajorFullFolderPath} {ex.Message}\n {ex.StackTrace}");
			}
			return studioDetails;
		}

		public static List<LocationDetails> GetLocalMajorFolderPath(string userName, StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
	    {
		    var studioDetails = new List<LocationDetails>();
			try
			{
				foreach (var studioVersion in studioVersions)
				{
					var majorFolderPath =
						$@"C:\Users\{userName}\AppData\Local\SDL\SDL Trados Studio\{studioVersion.MajorVersionNumber}";

					var directoryInfo = new DirectoryInfo(majorFolderPath);
					if (!directoryInfo.Exists) continue;

					var details = new LocationDetails
					{
						OriginalFilePath = majorFolderPath,
						BackupFilePath = Path.Combine(BackupFolderPath, studioVersion.DisplayName, "SDL Trados Studio", directoryInfo.Name),
						Alias = selectedLocation?.Alias,
						Version = studioVersion?.DisplayName
					};
					studioDetails.Add(details);
				}
			}
			catch(Exception ex)
			{
				Log.Logger.Error($"{Constants.GetLocalMajorFolderPath} {ex.Message}\n {ex.StackTrace}");
			}
			return studioDetails;
		}
    }
}