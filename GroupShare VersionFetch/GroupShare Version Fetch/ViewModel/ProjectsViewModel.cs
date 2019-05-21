using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Community.GSVersionFetch.Service;

namespace Sdl.Community.GSVersionFetch.ViewModel
{
	public class ProjectsViewModel: ProjectWizardViewModelBase
	{
		private bool _isValid;
		private readonly WizardModel _wizardModel;
		private readonly ProjectService _projectService;
		public ProjectsViewModel(WizardModel wizardModel, object view) : base(view)
		{
			_wizardModel = wizardModel;
			_projectService = new ProjectService();

			wizardModel.GsProjects.CollectionChanged += GsProjects_CollectionChanged;
		}

		private void GsProjects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var projects = (ObservableCollection<GsProject>) sender;

			if (projects?.Any() == null) return;
			foreach (var project in projects)
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
	}
}
