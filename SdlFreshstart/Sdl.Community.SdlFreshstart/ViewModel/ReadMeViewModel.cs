using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sdl.Community.SdlFreshstart.Annotations;
using Sdl.Community.SdlFreshstart.Helpers;

namespace Sdl.Community.SdlFreshstart.ViewModel
{
	public class ReadMeViewModel : INotifyPropertyChanged
	{
		public ReadMeViewModel(VersionService studioVersionService)
		{
			StudioVersions = studioVersionService.GetListOfStudioVersions();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public List<StudioVersion> StudioVersions { get; set; }

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}