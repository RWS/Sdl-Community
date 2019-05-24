using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Sdl.Community.GSVersionFetch.Commands;
using Sdl.Community.GSVersionFetch.Helpers;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Community.GSVersionFetch.Service;

namespace Sdl.Community.GSVersionFetch.ViewModel
{
    public class FilesVersionsViewModel : ProjectWizardViewModelBase
    {
	    private bool _isValid;
	    private ICommand _enterCommand;
		private readonly ProjectService _projectService;
		private readonly WizardModel _wizardModel;
	    private SolidColorBrush _textMessageBrush;
	    private string _textMessage;
	    private string _selectedVersion;
		private string _textMessageVisibility;

		public FilesVersionsViewModel(WizardModel wizardModel,object view) : base(view)
		{
			_wizardModel = wizardModel;
			_projectService = new ProjectService();
			PropertyChanged += FilesVersionsViewModel_PropertyChanged;
			_wizardModel.FileVersions.CollectionChanged += FileVersions_CollectionChanged;
		}

		private void FileVersions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (GsFileVersion gsFile in e.OldItems)
				{
					gsFile.PropertyChanged -= GsFile_PropertyChanged;
				}
			}
			if (e.NewItems == null) return;
			foreach (GsFileVersion gsFile in e.NewItems)
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
				    IsComplete = IsValid;
			    }
		    }
	    }

	    private async void Window_Closing(object sender, CancelEventArgs e)
	    {
		    if (IsComplete && IsCurrentPage)
		    {
			    var anySelectedFile = FilesVersions.Any(f => f.IsSelected);
			    if (anySelectedFile)
			    {
					var folderSelect = new FolderSelectDialog
				    {
					    Title = "Please select download location"
				    };
				    if (folderSelect.ShowDialog())
				    {
					    var folderPath = folderSelect.FileName;
					    if (!string.IsNullOrEmpty(folderPath))
					    {
						    var selectedVersions = FilesVersions.Where(v => v.IsSelected);
						    foreach (var selectedVersion in selectedVersions)
						    {
							    //TODO: write files on disk, we need to have a naming convention, because if the project has multiple languages file name is the same for each language
							    var file = await _projectService.DownloadFileVersion(selectedVersion.ProjectId, selectedVersion.LanguageFileId,
								    selectedVersion.Version);
						    }
					    }
				    }
				}
			}
		}

	    private async void FilesVersionsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(CurrentPageChanged))
			{
				if (IsCurrentPage)
				{
					TextMessage = PluginResources.Files_Version_Loading;
					TextMessageVisibility = "Visible";
					TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#00A8EB");
					var selectedFiles = _wizardModel.GsFiles.Where(f => f.IsSelected);
					foreach (var selectedFile in selectedFiles)
					{
						var fileVersions = await _projectService.GetFileVersion(selectedFile.UniqueId.ToString());
						SetFileProperties(selectedFile, fileVersions);
					}
				}
			}
			if (e.PropertyName.Equals("Window"))
			{
				Window.Closing -= Window_Closing;
				Window.Closing += Window_Closing;
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
	    public bool AllFilesChecked
	    {
		    get => AreAllFilesSelected();
		    set
		    {
			    ToggleCheckAllFiles(value);
			    IsComplete = value;
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

	    public string SelectedVersion
	    {
		    get => _selectedVersion;
		    set
		    {
			    if (IsValidVersion(value))
			    {
					_selectedVersion = value;
				    TextMessageVisibility = "Collapsed";
				    OnPropertyChanged(nameof(SelectedVersion));
				}
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
			foreach (var fileVersion in FilesVersions)
			{
				fileVersion.IsSelected = value;
			}
		}

	    private bool AreAllFilesSelected()
	    {
			return FilesVersions?.Count > 0 && FilesVersions.All(f => f.IsSelected);
		}

	    private bool IsValidVersion(string version)
	    {
		    if (string.IsNullOrEmpty(version))
		    {
			    ToggleCheckAllFiles(false);
				return true;
		    }
		    if (int.TryParse(version, out _))
		    {
			    return true;
		    }

		    TextMessage = PluginResources.Version_Validation;
		    TextMessageVisibility = "Visible";
		    TextMessageBrush = new SolidColorBrush(Colors.Red);
		    return false;
	    }
	    public ICommand EnterCommand => _enterCommand ?? (_enterCommand = new CommandHandler(SelectSpecificVersion,true));

	    private void SelectSpecificVersion()
	    {
		    if (!string.IsNullOrEmpty(SelectedVersion))
		    {
			    var filesVersion = FilesVersions.Where(f => f.Version.Equals(int.Parse(SelectedVersion))).ToList();
			    if (filesVersion.Any())
			    {
				    foreach (var file in filesVersion)
				    {
					    file.IsSelected = true;
				    }
			    }
		    }
		    else
			{	//for empty box we'll deselect all boxes
				ToggleCheckAllFiles(false);
		    }
	    }


	    private void SetFileProperties(GsFile selectedFile, List<GsFileVersion> fileVersions)
	    {
		    foreach (var fileVersion in fileVersions)
		    {
			    fileVersion.ProjectName = selectedFile.ProjectName;
			    fileVersion.LanguageFlagImage = selectedFile.LanguageFlagImage;
			    fileVersion.LanguageName = selectedFile.LanguageName;
			    fileVersion.ProjectId = selectedFile.ProjectId;
			    _wizardModel?.FileVersions?.Add(fileVersion);
		    }
		    TextMessageVisibility = "Collapsed";
		}
	}
}
