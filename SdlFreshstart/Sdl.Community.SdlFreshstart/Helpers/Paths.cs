using System;
using System.Collections.Generic;
using System.IO;
using Sdl.Community.SdlFreshstart.Model;

namespace Sdl.Community.SdlFreshstart.Helpers
{
	public static class Paths
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

					var fileName = File.Exists(location) ? Path.GetFileName(location) : string.Empty;
					fileName = locationName == nameof(StudioVersion.SdlRegistryKey) ? "sdlregkeys.reg" : fileName;

					locations.Add(new LocationDetails
					{
						Alias = locationName,
						BackupFilePath = Path.Combine(BackupFolderPath, version.VersionWithEdition, locationName, fileName),
						OriginalPath = location,
						Version = version.ShortVersion
					});
				}
			}

			return locations;
		}

		public static List<LocationDetails> GetMultiTermLocationsFromVersions(
			List<string> locationNames,
			List<MultitermVersion> multitermVersions)
		{
			var locations = new List<LocationDetails>();
			foreach (var locationName in locationNames)
			{
				foreach (var version in multitermVersions)
				{
					if (version == null) continue;
					var location = (string)version.GetType().GetProperty(locationName)?.GetValue(version);

					var fileName = File.Exists(location) ? Path.GetFileName(location) : string.Empty;
					fileName = locationName == nameof(MultitermVersion.MultiTermRegistryKey) ? "multitermregkeys.reg" : fileName;

					locations.Add(new LocationDetails
					{
						Alias = locationName,
						BackupFilePath = Path.Combine(BackupFolderPath, version.VersionName, locationName, fileName),
						OriginalPath = location,
						Version = version.VersionName
					});
				}
			}

			return locations;
		}
	}
}