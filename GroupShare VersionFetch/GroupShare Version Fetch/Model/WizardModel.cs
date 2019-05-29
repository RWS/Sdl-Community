using System.Collections.ObjectModel;

namespace Sdl.Community.GSVersionFetch.Model
{
	public class WizardModel : BaseModel
	{
		private Credentials _userCredentials;
		private ObservableCollection<GsProject> _gsProjects;
		private ObservableCollection<GsFile> _gsFiles;
		private ObservableCollection<GsFileVersion> _filesFileVersions;
		private string _version;
		private int _projectsNumber;
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
		public ObservableCollection<GsFile> GsFiles
		{
			get => _gsFiles;
			set
			{
				_gsFiles = value;
				OnPropertyChanged(nameof(GsFiles));
			}
		}
		public ObservableCollection<GsFileVersion> FileVersions
		{
			get => _filesFileVersions;
			set
			{
				_filesFileVersions = value;
				OnPropertyChanged(nameof(FileVersions));
			}
		}
		public string Version
		{
			get => _version;
			set
			{
				_version = value;
				OnPropertyChanged(nameof(Version));
			}
		}
		public int ProjectsNumber
		{
			get => _projectsNumber;
			set
			{
				_projectsNumber = value;
				OnPropertyChanged(nameof(ProjectsNumber));
			}
		}
	}
}
