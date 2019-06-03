using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Sdl.Community.GSVersionFetch.Commands;
using Sdl.Community.GSVersionFetch.Helpers;
using Sdl.Community.GSVersionFetch.Model;
using UserControl = System.Windows.Controls.UserControl;

namespace Sdl.Community.GSVersionFetch.ViewModel
{
	public class ProjectsViewModel: ProjectWizardViewModelBase
	{
		private bool _isValid;
		private bool _isPreviousEnabled;
		private bool _isNextEnabled;
		private string _displayName;
		private string _searchText;
		private ICommand _refreshProjectsCommand;
		private ICommand _nextPageCommand;
		private ICommand _previousPageCommand;
		private int _currentPageNumber;
		private readonly WizardModel _wizardModel;
		private readonly UserControl _view;
		public static readonly Log Log = Log.Instance;

		public ProjectsViewModel(WizardModel wizardModel, object view) : base(view)
		{
			_currentPageNumber = 1;
			_wizardModel = wizardModel;
			_displayName = "GroupShare Projects";
			_searchText = string.Empty;
			_isPreviousEnabled = false;
			_view = (UserControl)view;
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

		public string SearchText
		{
			get => _searchText;
			set
			{
				_searchText = value;
				_wizardModel?.GsProjects?.Clear();
				_wizardModel?.ProjectsForCurrentPage?.Clear();
				OnPropertyChanged(SearchText);
				_view.Dispatcher.Invoke(async () =>
				{
					await LoadProjectsForCurrentPage();
				},DispatcherPriority.ContextIdle);
	
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
				var utils = new Utils();
				var filter = new ProjectFilter
				{
					Filter = new Filter
					{
						ProjectName = SearchText,
						OrgPath = "/",
						Status = 7,
						IncludeSubOrgs = true
					},
					PageSize = 50,
					Page = CurrentPageNumber
				};
				await utils.SetGsProjectsToWizard(_wizardModel, filter);

				IsValid = false;
				UpdateNavigationButtons();
				_view.Dispatcher?.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate
				{
					OnPropertyChanged(nameof(ProjectsNumber));
					OnPropertyChanged(nameof(PagesNumber));

				}));

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
