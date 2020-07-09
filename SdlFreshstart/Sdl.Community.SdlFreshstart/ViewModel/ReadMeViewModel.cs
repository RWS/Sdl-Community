using System.Collections.Generic;
using System.ComponentModel;
using Sdl.Community.SdlFreshstart.Helpers;

namespace Sdl.Community.SdlFreshstart.ViewModel
{
	public class ReadMeViewModel
	{
		public ReadMeViewModel(VersionService studioVersionService)
		{
			StudioVersions = studioVersionService.GetListOfStudioVersions();
			StudioVersions.Reverse();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public List<StudioVersion> StudioVersions { get; set; }
	}
}