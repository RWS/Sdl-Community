using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Community.GSVersionFetch.Service;

namespace Sdl.Community.GSVersionFetch.ViewModel
{
    public class FilesVersionsViewModel : ProjectWizardViewModelBase
    {
	    private bool _isValid;
	    private readonly ProjectService _projectService;
		private readonly WizardModel _wizardModel;

		public FilesVersionsViewModel(WizardModel wizardModel,object view) : base(view)
		{
			_wizardModel = wizardModel;
			_projectService = new ProjectService();
			PropertyChanged += FilesVersionsViewModel_PropertyChanged;
		}

		private async void FilesVersionsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(CurrentPageChanged))
			{
				if (IsCurrentPage)
				{
					var selectedFiles = _wizardModel.GsFiles.Where(f => f.IsSelected);
					foreach (var selectedFile in selectedFiles)
					{
						var fileVersions = await _projectService.GetFileVersion(selectedFile.UniqueId.ToString());
						SetFileProperties(selectedFile, fileVersions);
					}
				}
			}
		}

	    private void SetFileProperties(GsFile selectedFile, List<GsFileVersion> fileVersions)
	    {
		    foreach (var fileVersion in fileVersions)
		    {
			    fileVersion.ProjectName = selectedFile.ProjectName;
			    fileVersion.LanguageFlagImage = selectedFile.LanguageFlagImage;
			    fileVersion.LanguageName = selectedFile.LanguageName;

				_wizardModel?.FileVersions?.Add(fileVersion);
		    }
	    }

	    public override string DisplayName => "Files versions";
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

	    public ObservableCollection<GsFileVersion> FilesVersions
	    {
		    get => _wizardModel.FileVersions;
		    set
		    {
			    _wizardModel.FileVersions = value;
			    OnPropertyChanged(nameof(FilesVersions));
		    }
	    }
    }
}
