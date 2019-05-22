using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Community.GSVersionFetch.Service;
using Sdl.Core.Globalization;

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
						SetFileProperties(selectedProject.ProjectId,selectedProject.Name, files);
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

		public ObservableCollection<GsFile> GsFiles
		{
			get => _wizardModel?.GsFiles;
			set
			{
				_wizardModel.GsFiles = value;
				OnPropertyChanged(nameof(GsFiles));
			}
		}

		public override string DisplayName => " Projects files";

		private void SetFileProperties(string projectId, string projectName,IEnumerable<GsFile> files)
		{
			foreach (var gsFile in files)
			{
				gsFile.ProjectId = projectId;
				gsFile.ProjectName = projectName;
				gsFile.LanguageFlagImage = new Language(gsFile.LanguageCode).GetFlagImage();
				gsFile.LanguageName = new Language(gsFile.LanguageCode).DisplayName;
				_wizardModel?.GsFiles?.Add(gsFile);
			}
		}
	}
}
