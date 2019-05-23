using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
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
		private SolidColorBrush _textMessageBrush;
		private string _textMessage;
		private string _textMessageVisibility;

		public FilesViewModel(WizardModel wizardModel,object view) : base(view)
		{
			_projectService = new ProjectService();
			_wizardModel = wizardModel;
			PropertyChanged += FilesViewModel_PropertyChanged;
			wizardModel.GsFiles.CollectionChanged += GsFiles_CollectionChanged;
		}

		private void GsFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (GsFile gsFile in e.OldItems)
				{
					gsFile.PropertyChanged -= GsFile_PropertyChanged;
				}
			}

			if (e.NewItems == null) return;
			foreach (GsFile gsFile in e.NewItems)
			{
				gsFile.PropertyChanged += GsFile_PropertyChanged;
			}
		}

		private void GsFile_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals("IsSelected"))
			{
				OnPropertyChanged(nameof(AllFilesChecked));
				if (_wizardModel?.GsFiles != null)
				{
					IsValid = _wizardModel.GsFiles.Any(f => f.IsSelected);
				}
			}
		}

		private async void FilesViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(CurrentPageChanged))
			{
				if (IsCurrentPage)
				{
					TextMessage = "Please wait, we are loading GroupShare files";
					TextMessageVisibility = "Visible";
					TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#00A8EB");
					var selectedProjects = _wizardModel.GsProjects.Where(p => p.IsSelected);
					//_wizardModel?.GsFiles?.Clear();
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

		public bool AllFilesChecked
		{
			get => AreAlFilesSelected();
			set
			{
				ToggleCheckAllFiles(value);
				OnPropertyChanged(nameof(AllFilesChecked));
			}
		}

		public string TextMessage
		{
			get => _textMessage;
			set
			{
				_textMessage = value;
				OnPropertyChanged(nameof(TextMessage));
			}
		}
		public string TextMessageVisibility
		{
			get => _textMessageVisibility;
			set
			{
				_textMessageVisibility = value;
				OnPropertyChanged(nameof(TextMessageVisibility));
			}
		}
		public SolidColorBrush TextMessageBrush
		{
			get => _textMessageBrush;
			set
			{
				_textMessageBrush = value;
				OnPropertyChanged(nameof(TextMessageBrush));
			}
		}

		private void ToggleCheckAllFiles(bool value)
		{
			foreach (var gsFile in GsFiles)
			{
				gsFile.IsSelected = value;
			}
		}

		private bool AreAlFilesSelected()
		{
			return GsFiles?.Count > 0 && GsFiles.All(f => f.IsSelected);
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
			TextMessageVisibility = "Collapsed";
		}
	}
}
