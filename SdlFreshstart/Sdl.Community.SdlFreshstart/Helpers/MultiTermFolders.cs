using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sdl.Community.SdlFreshstart.Model;

namespace Sdl.Community.SdlFreshstart.Helpers
{
	public static class MultiTermFolders
	{
		private static string _backupFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL", "StudioCleanup");
		public static readonly Log Log = Log.Instance;

		public static List<LocationDetails> GetPackageCachePaths(List<MultiTermVersionListItem> multiTermVersions)
		{
			var packagePaths = new List<LocationDetails>();
			try
			{
				foreach (var multiTermVersion in multiTermVersions)
				{
					var versionName = string.Format("SDLMultiTermDesktop{0}", multiTermVersion.ReleaseNumber);
					var packagePathList = GetMultiTermDesktopPaths(versionName);
					if (packagePathList.Any())
					{
						foreach (var packagePath in packagePathList)
						{
							var directoryInfo = new DirectoryInfo(packagePath);
							var details = new LocationDetails
							{
								OriginalFilePath = packagePath,
								BackupFilePath = Path.Combine(_backupFolderPath, multiTermVersion.DisplayName, "PackageCache", directoryInfo.Name),
								Version = multiTermVersion.DisplayName
							};
							packagePaths.Add(details);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.GetPackageCachePaths} {ex.Message}\n {ex.StackTrace}");
			}
			return packagePaths;
		}

		private static List<string> GetMultiTermDesktopPaths(string versionNumber)
		{
			var directoriesPath = new List<string>();
			try
			{
				var packageCachePath = @"C:\ProgramData\Package Cache\SDL";
				if (Directory.Exists(packageCachePath))
				{
					directoriesPath = new DirectoryInfo(@"C:\ProgramData\Package Cache\SDL")
						.GetDirectories()
						.Where(n => n.Name.Contains(versionNumber))
						.Select(n => n.FullName).ToList();

					return directoriesPath;
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.GetMultiTermDesktopPaths} {ex.Message}\n {ex.StackTrace}");
			}
			return directoriesPath;
		}

		public static List<LocationDetails> ProgramFilesPaths(List<MultiTermVersionListItem> multiTermVersions)
		{
			var programFilesPaths = new List<LocationDetails>();
			try
			{
				foreach (var multiTermVersion in multiTermVersions)
				{
					var programFilePath = string.Format(@"C:\Program Files (x86)\SDL\SDL MultiTerm\MultiTerm{0}", multiTermVersion.MajorVersionNumber);
					var directoryInfo = new DirectoryInfo(programFilePath);
					var details = new LocationDetails
					{
						OriginalFilePath = programFilePath,
						BackupFilePath = Path.Combine(_backupFolderPath, multiTermVersion.DisplayName, "ProgramFiles", directoryInfo.Name),
						Version = multiTermVersion.DisplayName
					};
					programFilesPaths.Add(details);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.ProgramFilesPaths} {ex.Message}\n {ex.StackTrace}");
			}
			return programFilesPaths;
		}

		public static List<LocationDetails> AppDataLocalPaths(string userName, List<MultiTermVersionListItem> multiTermVersions)
		{
			var appDataPaths = new List<LocationDetails>();
			try
			{
				foreach (var multiTermVersion in multiTermVersions)
				{
					var appDataFilePath = string.Format(@"C:\Users\{0}\AppData\Local\SDL\SDL MultiTerm\MultiTerm{1}", userName, multiTermVersion.MajorVersionNumber);
					var directoryInfo = new DirectoryInfo(appDataFilePath);
					var details = new LocationDetails
					{
						OriginalFilePath = appDataFilePath,
						BackupFilePath = Path.Combine(_backupFolderPath, multiTermVersion.DisplayName, "Local", directoryInfo.Name),
						Version = multiTermVersion.DisplayName
					};
					appDataPaths.Add(details);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.AppDataLocalPaths} {ex.Message}\n {ex.StackTrace}");
			}
			return appDataPaths;
		}

		public static List<LocationDetails> AppDataRoamingPaths(string userName, List<MultiTermVersionListItem> multiTermVersions)
		{
			var appDataPaths = new List<LocationDetails>();
			try
			{
				foreach (var multiTermVersion in multiTermVersions)
				{
					var appDataFilePath = string.Format(@"C:\Users\{0}\AppData\Roaming\SDL\SDL MultiTerm\MultiTerm{1}", userName, multiTermVersion.MajorVersionNumber);
					var directoryInfo = new DirectoryInfo(appDataFilePath);
					var details = new LocationDetails
					{
						OriginalFilePath = appDataFilePath,
						BackupFilePath = Path.Combine(_backupFolderPath, multiTermVersion.DisplayName, "Roaming", directoryInfo.Name),
						Version = multiTermVersion.DisplayName
					};
					appDataPaths.Add(details);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.AppDataRoamingPaths} {ex.Message}\n {ex.StackTrace}");
			}
			return appDataPaths;
		}
	}
}