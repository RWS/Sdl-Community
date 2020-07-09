using System.Collections.Generic;
using System.Linq;
using Sdl.Versioning;

namespace Sdl.Community.SdlFreshstart.Helpers
{
	public class VersionService
	{
		public List<StudioVersion> GetListOfStudioVersions()
		{
			var versionsDictionary = Versions.KnownStudioVersions.Skip(1).TakeWhile(s => !s.Value.Contains("Next"));
			return versionsDictionary.Select(item => new StudioVersion(item.Key, item.Value)).ToList();
		}

		public List<StudioVersion> GetInstalledStudioVersions()
		{
			var studioVersionService = new StudioVersionService();
			var installedStudioVersions = studioVersionService.GetInstalledStudioVersions();

			return installedStudioVersions.Select(v =>
				v.Edition.Equals("Beta")
					? new StudioVersion(v.Version, v.PublicVersion, v.Edition)
					: new StudioVersion(v.Version, v.PublicVersion)).ToList();
		}
	}
}