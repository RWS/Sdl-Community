using System.Collections.ObjectModel;

namespace Sdl.Community.GSVersionFetch.Model
{
	public class WizardModel : BaseModel
	{
		private Credentials _userCredentials;
		private ObservableCollection<GsProject> _gsProjects;
		public Credentials UserCredentials
		{
			get => _userCredentials;
			set
			{
				_userCredentials = value;
				OnPropertyChanged(nameof(UserCredentials));
			}
		}

		public ObservableCollection<GsProject> GsProjects
		{
			get => _gsProjects;
			set
			{
				_gsProjects = value;
				OnPropertyChanged(nameof(GsProjects));
			}
		}


	}
}
