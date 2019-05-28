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
		private ICommand _refreshProjectsCommand;
		private readonly WizardModel _wizardModel;
		public static readonly Log Log = Log.Instance;

		public ProjectsViewModel(WizardModel wizardModel, object view) : base(view)
		{
			_wizardModel = wizardModel;
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


		private void GsProject_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals("IsSelected"))
			{
				IsValid = _wizardModel.GsProjects.Any(p => p.IsSelected);
			}
		}

		public override string DisplayName => "GroupShare Projects";
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
