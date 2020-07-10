using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Sdl.Community.SdlFreshstart.Model;

namespace Sdl.Community.SdlFreshstart.Helpers
{
	public static class FoldersPath
	{
		public static readonly Log Log = Log.Instance;
		private static readonly string BackupFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL", "StudioCleanup");

		public static List<LocationDetails> GetLocationsFromVersions(List<string> locationNames, List<StudioVersion> studioVersions)
		{
			var locations = new List<LocationDetails>(studioVersions.Count * locationNames.Count);
			foreach (var locationName in locationNames)
			{
				foreach (var version in studioVersions)
				{
					var location = (string)studioVersions[0]?.GetType().GetProperty(locationName)?.GetValue(version);

					var locationDetails = new LocationDetails
					{
						Alias = locationName,
						BackupFilePath = Path.Combine(BackupFolderPath, version.ShortVersion, locationName),
						OriginalFilePath = location,
						Version = version.ShortVersion
					};

					locations.Add(locationDetails);
				}
			}

			return locations;
		}

		public static async Task<List<LocationDetails>> GetMultiTermFoldersPath(
			string userName,
			List<MultiTermVersionListItem> multiTermVersions,
			List<MultiTermLocationListItem> locations)
		{
			var foldersLocationList = new List<LocationDetails>();
			try
			{
				foreach (var location in locations)
				{
					if (location.Alias != null)
					{
						if (location.Alias.Equals("packageCache"))
						{
							var packageCacheLocations = await Task.FromResult(MultiTermFolders.GetPackageCachePaths(multiTermVersions));
							foldersLocationList.AddRange(packageCacheLocations);
						}
						if (location.Alias.Equals("programFiles"))
						{
							var programFilesLocations = await Task.FromResult(MultiTermFolders.ProgramFilesPaths(multiTermVersions));
							foldersLocationList.AddRange(programFilesLocations);
						}
						if (location.Alias.Equals("appDataLocal"))
						{
							var appDataLocal = await Task.FromResult(MultiTermFolders.AppDataLocalPaths(userName, multiTermVersions));
							foldersLocationList.AddRange(appDataLocal);
						}
						if (location.Alias.Equals("appDataRoming"))
						{
							var appDataRoaming = await Task.FromResult(MultiTermFolders.AppDataRoamingPaths(userName, multiTermVersions));
							foldersLocationList.AddRange(appDataRoaming);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.GetMultiTermFoldersPath} {ex.Message}\n {ex.StackTrace}");
			}
			return foldersLocationList;
		}
	}
}