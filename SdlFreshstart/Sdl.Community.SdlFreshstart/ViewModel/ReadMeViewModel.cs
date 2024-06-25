using System.Collections.Generic;
using Sdl.Community.SdlFreshstart.Model;
using Sdl.Community.SdlFreshstart.Services;

namespace Sdl.Community.SdlFreshstart.ViewModel
{
	public class ReadMeViewModel
	{
		public ReadMeViewModel(StudioVersionService versioningService)
		{
			StudioVersions = versioningService.GetInstalledStudioVersions();
			StudioVersions.Reverse();

			MultitermVersions = versioningService.GetInstalledMultitermVersions();
		}

		public List<StudioVersion> StudioVersions { get; set; }
		public List<MultitermVersion> MultitermVersions { get; set; }
	}
}