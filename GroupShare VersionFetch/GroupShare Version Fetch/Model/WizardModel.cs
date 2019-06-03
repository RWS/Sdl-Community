using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sdl.Community.GSVersionFetch.Model
{
	public class WizardModel : BaseModel
	{
		private Credentials _userCredentials;
		private ObservableCollection<GsProject> _gsProjects;
		private ObservableCollection<GsProject> _projectsForCurrentPage;
		private ObservableCollection<GsFile> _gsFiles;
		private ObservableCollection<GsFileVersion> _filesFileVersions;
		private ObservableCollection<OrganizationResponse> _organizations;
		private List<OrganizationHierarchy> _organizationsTreeView;
		private int _totalPages;
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
		public ObservableCollection<GsProject> ProjectsForCurrentPage
		{
			get => _projectsForCurrentPage;
			set
			{
				_projectsForCurrentPage = value;
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

		public ObservableCollection<OrganizationResponse> Organizations
		{
			get => _organizations;
			set
			{
				if (_organizations == value)
				{
					return;
				}
				_organizations = value;
				OnPropertyChanged(nameof(Organizations));
			}
		}

		public List<OrganizationHierarchy> OrganizationsTreeView
		{
			get => _organizationsTreeView;
			set
			{
				if (_organizationsTreeView == value)
				{
					return;
				}
				_organizationsTreeView = value;
				OnPropertyChanged(nameof(OrganizationsTreeView));
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
				if (_projectsNumber == value)
				{
					return;
				}
				_projectsNumber = value;
				OnPropertyChanged(nameof(ProjectsNumber));
			}
		}
		public int TotalPages
		{
			get => _totalPages;
			set
			{
				if (_totalPages == value)
				{
					return;
				}
				_totalPages = value;
				OnPropertyChanged(nameof(TotalPages));
			}
		}
	}
}
