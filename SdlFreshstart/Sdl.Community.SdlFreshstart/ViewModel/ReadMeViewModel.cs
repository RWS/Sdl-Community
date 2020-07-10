using System.Collections.Generic;
using System.ComponentModel;
using Sdl.Community.SdlFreshstart.Helpers;
using Sdl.Community.SdlFreshstart.Model;
using Sdl.Community.SdlFreshstart.Services;

namespace Sdl.Community.SdlFreshstart.ViewModel
{
	public class ReadMeViewModel
	{
		public ReadMeViewModel(VersionService studioVersionService)
		{
			StudioVersions = studioVersionService.GetListOfStudioVersions();
			StudioVersions.Reverse();
		}

		public List<StudioVersion> StudioVersions { get; set; }
	}
}