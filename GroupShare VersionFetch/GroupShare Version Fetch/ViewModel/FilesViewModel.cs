using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Community.GSVersionFetch.Service;

namespace Sdl.Community.GSVersionFetch.ViewModel
{
	public class FilesViewModel : ProjectWizardViewModelBase
	{
		private bool _isValid;
		private readonly ProjectService _projectService;
		private readonly WizardModel _wizardModel;

		public FilesViewModel(WizardModel wizardModel,object view) : base(view)
		{
			_projectService = new ProjectService();
			_wizardModel = wizardModel;
			PropertyChanged += FilesViewModel_PropertyChanged;
		}

		private async void FilesViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(CurrentPageChanged))
			{
				if (IsCurrentPage)
				{
					var selectedProjects = _wizardModel.GsProjects.Where(p => p.IsSelected);
					foreach (var selectedProject in selectedProjects)
					{
						var files = await _projectService.GetProjectFiles(selectedProject.ProjectId);
						SetProjectIdOnGsFiles(selectedProject.ProjectId, files);
					}
				}
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

		public override string DisplayName => " Projects files";

		private void SetProjectIdOnGsFiles(string projectId, IEnumerable<GsFile> files)
		{
			foreach (var gsFile in files)
			{
				gsFile.ProjectId = projectId;
				_wizardModel?.GsFiles?.Add(gsFile);
			}
		}
	}
}
