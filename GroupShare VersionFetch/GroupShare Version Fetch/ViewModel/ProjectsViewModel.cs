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

		public ObservableCollection<GsProject> GsProjects
		{
			get => _wizardModel.GsProjects;
			set
			{
				_wizardModel.GsProjects = value;
				OnPropertyChanged(nameof(GsProjects));
			}
		}
		public ICommand RefreshProjectsCommand =>
			_refreshProjectsCommand ?? (_refreshProjectsCommand = new AwaitableDelegateCommand(RefreshProjects));
		public ICommand NextPageCommand => _nextPageCommand ?? (_nextPageCommand = new CommandHandler(DisplayNextPage, true));
		public ICommand PreviousPageCommand => _previousPageCommand ?? (_previousPageCommand = new CommandHandler(DisplayPreviousPage, true));

		private void DisplayPreviousPage()
		{
		}

		private void DisplayNextPage()
		{
			
		}

		private async Task RefreshProjects()
		{
			try
			{
				_wizardModel?.GsProjects?.Clear();
				var projectService = new ProjectService();
				var languageFlagsHelper = new LanguageFlags();
				var projectsResponse = await projectService.GetGsProjects();
				if (projectsResponse?.Items != null)
				{
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
					}
					IsValid = false;
				}
			}
			catch (Exception e)
			{
				Log.Logger.Error($"RefreshProjects method: {e.Message}\n {e.StackTrace}");
			}
		}
	}
}
