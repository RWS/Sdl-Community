using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.GSVersionFetch.Commands;
using Sdl.Community.GSVersionFetch.Helpers;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Community.GSVersionFetch.Service;
using Sdl.Core.Globalization;

namespace Sdl.Community.GSVersionFetch.ViewModel
{
	public class ProjectsViewModel: ProjectWizardViewModelBase
	{
		private bool _isValid;
		private bool _isPreviousEnabled;
		private bool _isNextEnabled;
		private string _displayName;
		private ICommand _refreshProjectsCommand;
		private ICommand _nextPageCommand;
		private ICommand _previousPageCommand;
		private int _currentPageNumber;
		private readonly WizardModel _wizardModel;
		public static readonly Log Log = Log.Instance;

		public ProjectsViewModel(WizardModel wizardModel, object view) : base(view)
		{
			_currentPageNumber = 1;
			_wizardModel = wizardModel;
			_displayName = "GroupShare Projects";
			_isPreviousEnabled = false;
			_isNextEnabled = true;
			_wizardModel.GsProjects.CollectionChanged += GsProjects_CollectionChanged;
		}

		private void GsProjects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (GsProject gsProject in e.OldItems)
				{
					gsProject.PropertyChanged -= GsProject_PropertyChanged;
				}
			}

			if (e.NewItems == null) return;
			foreach (GsProject project in e.NewItems)
			{
				project.PropertyChanged += GsProject_PropertyChanged;
			}
		}
		public override  bool OnChangePage(int position, out string message)
		{
			message = string.Empty;

			var pagePosition = PageIndex - 1;
			if (position == pagePosition)
			{
				return false;
			}

			if (!IsValid && position > pagePosition)
			{
				message = "Please select at least one project before moving to next page";
				return false;
			}
			return true;
		}

		private void GsProject_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals("IsSelected"))
			{
				IsValid = _wizardModel.GsProjects.Any(p => p.IsSelected);
			}
		}
		public override string DisplayName
		{
			get => _displayName;
			set
			{
				if (_displayName == value)
				{
					return;
				}

				_displayName = value;
				OnPropertyChanged(nameof(DisplayName));
			}
		}
		public override bool IsValid
		{
			get => _isValid;
			set
			{
				if (_isValid == value)
					return;

				_isValid = value;
				OnPropertyChanged(nameof(IsValid));
			}
		}
		public  bool IsPreviousEnabled
		{
			get => _isPreviousEnabled;
			set
			{
				if (_isPreviousEnabled == value)
					return;

				_isPreviousEnabled = value;
				OnPropertyChanged(nameof(IsPreviousEnabled));
			}
		}
		public bool IsNextEnabled
		{
			get => _isNextEnabled;
			set
			{
				if (_isNextEnabled == value)
					return;

				_isNextEnabled = value;
				OnPropertyChanged(nameof(IsNextEnabled));
			}
		}
		public int ProjectsNumber
		{
			get => _wizardModel.ProjectsNumber;
			set
			{
				if (_wizardModel?.ProjectsNumber == value)
					return;
				if (_wizardModel != null)
				{
					_wizardModel.ProjectsNumber = value;
				}				
				OnPropertyChanged(nameof(ProjectsNumber));
			}
		}

		public int CurrentPageNumber
		{
			get => _currentPageNumber;
			set
			{
				_currentPageNumber = value;
				OnPropertyChanged(nameof(CurrentPageNumber));
			}
		}
		public int PagesNumber
		{
			get => _wizardModel.TotalPages;
			set
			{
				_wizardModel.TotalPages = value;
				OnPropertyChanged(nameof(PagesNumber));
			}
		}

		public ObservableCollection<GsProject> GsProjects
		{
			get => _wizardModel?.GsProjects;
			set
			{
				_wizardModel.GsProjects = value;
				OnPropertyChanged(nameof(GsProjects));
			}
		}
		public ObservableCollection<GsProject> ProjectsForCurrentPage
		{
			get => _wizardModel?.ProjectsForCurrentPage;
			set
			{
				_wizardModel.ProjectsForCurrentPage = value;
				OnPropertyChanged(nameof(ProjectsForCurrentPage));
			}
		}
		public ICommand RefreshProjectsCommand =>
			_refreshProjectsCommand ?? (_refreshProjectsCommand = new AwaitableDelegateCommand(RefreshProjects));
		public ICommand NextPageCommand => _nextPageCommand ?? (_nextPageCommand = new AwaitableDelegateCommand(DisplayNextPage));
		public ICommand PreviousPageCommand => _previousPageCommand ?? (_previousPageCommand = new AwaitableDelegateCommand(DisplayPreviousPage));

		private async Task DisplayPreviousPage()
		{
			CurrentPageNumber--;
			await GetProjects();
		}

		private async Task DisplayNextPage()
		{
			CurrentPageNumber++;
			await GetProjects();
		}

		private async Task GetProjects()
		{
			_wizardModel?.ProjectsForCurrentPage.Clear();

			if (!ExistsProjectsForCurrentPage())
			{
				await LoadProjectsForCurrentPage();
			}
			else
			{
				UpdateNavigationButtons();
			}
		}

		private bool ExistsProjectsForCurrentPage()
		{
			var page = CurrentPageNumber - 1;
			var projectsList = _wizardModel?.GsProjects.Skip(page * 50).Take(50).ToList();

			if (projectsList?.Count > 0 && _wizardModel!=null)
			{
				foreach (var project in projectsList)
				{
					_wizardModel.ProjectsForCurrentPage.Add(project);
				}
				return true;
			}
			return false;
		}

		private async Task RefreshProjects()
		{
			_wizardModel?.GsProjects?.Clear();
			_wizardModel?.ProjectsForCurrentPage?.Clear();

			await LoadProjectsForCurrentPage();
		}

		private async Task LoadProjectsForCurrentPage()
		{
			try
			{
				var projectService = new ProjectService();
				var languageFlagsHelper = new LanguageFlags();
				var projectFilter = new ProjectFilter
				{
					Page = CurrentPageNumber,
					PageSize = 50
				};
				var projectsResponse = await projectService.GetGsProjects(projectFilter);
				if (projectsResponse?.Items != null)
				{
					_wizardModel.ProjectsNumber = projectsResponse.Count;
					_wizardModel.TotalPages = (projectsResponse.Count + projectFilter.PageSize - 1) / projectFilter.PageSize;

					foreach (var project in projectsResponse.Items)
					{
						var gsProject = new GsProject
						{
							Name = project.Name,
							DueDate = project.DueDate?.ToString(),
							Image = new Language(project.SourceLanguage).GetFlagImage(),
							TargetLanguageFlags = languageFlagsHelper.GetTargetLanguageFlags(project.TargetLanguage),
							ProjectId = project.ProjectId,
							SourceLanguage = project.SourceLanguage
						};

						if (Enum.TryParse<ProjectStatus.Status>(project.Status.ToString(), out _))
						{
							gsProject.Status = Enum.Parse(typeof(ProjectStatus.Status), project.Status.ToString()).ToString();
						}
						_wizardModel?.GsProjects?.Add(gsProject);
						_wizardModel?.ProjectsForCurrentPage?.Add(gsProject);
					}
					IsValid = false;
					UpdateNavigationButtons();
				}
			}
			catch (Exception e)
			{
				Log.Logger.Error($"RefreshProjects method: {e.Message}\n {e.StackTrace}");
			}
		}
		private void UpdateNavigationButtons()
		{
			IsPreviousEnabled = !CurrentPageNumber.Equals(1);

			if (PagesNumber > 0)
			{
				IsNextEnabled = !CurrentPageNumber.Equals(PagesNumber);
			}
		}
	}
}
