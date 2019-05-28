using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Sdl.Community.GSVersionFetch.Commands;
using Sdl.Community.GSVersionFetch.Helpers;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Community.GSVersionFetch.Service;
using Sdl.Community.GSVersionFetch.UiHelpers;

namespace Sdl.Community.GSVersionFetch.ViewModel
{
    public class FilesVersionsViewModel : ProjectWizardViewModelBase
    {
	    private bool _isValid;
	    private ICommand _enterCommand;
	    private ICommand _downloadFilesCommand;
		private readonly ProjectService _projectService;
		private readonly WizardModel _wizardModel;
	    private SolidColorBrush _textMessageBrush;
	    private string _textMessage;
	    private string _selectedVersion;
		private string _textMessageVisibility;
	    private readonly ObservableCollection<GsFile> _oldSelectedFiles;

		public FilesVersionsViewModel(WizardModel wizardModel,object view) : base(view)
		{
			_wizardModel = wizardModel;
			_projectService = new ProjectService();
			_oldSelectedFiles = new ObservableCollection<GsFile>();
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
			    }
		    }
	    }

	    private void GroupFilesByFolderStructure(string selectedFolderPath)
	    {
		    var selectedVersionsGroups =
			    FilesVersions.Where(v => v.IsSelected).GroupBy(p => p.ProjectName); //Group by project name
		    foreach (var group in selectedVersionsGroups)
		    {
			    var projectName = group.Key;
			    var fileVersionsGroup = group.ToList().GroupBy(l => l.LanguageCode); //Group by language code
			    foreach (var languageGroup in fileVersionsGroup)
			    {
				    var languageCode = languageGroup.Key;
				    var languageFolderPath = Path.Combine(selectedFolderPath, projectName, languageCode);

				    var versionGroups = languageGroup.ToList().GroupBy(f => f.Version);
				    foreach (var versionGroup in versionGroups)
				    {
					    var versionFolderPath = Path.Combine(languageFolderPath, versionGroup.Key.ToString()); //Group by file version
					    var files = versionGroup.ToList();

					    SaveFiles(versionFolderPath, files);
				    }
			    }
		    }
	    }

	    private async void SaveFiles(string folderPath,List<GsFileVersion> files)
	    {
			if (!Directory.Exists(folderPath))
		    {
			    Directory.CreateDirectory(folderPath);
		    }
		    foreach (var file in files)
		    {
			    var rawFile = await _projectService.DownloadFileVersion(file.ProjectId, file.LanguageFileId,
				    file.Version);

			    var filePath = Path.Combine(folderPath, file.FileName);
			    if (File.Exists(filePath))
			    {
				    File.Delete(filePath);
			    }
			    File.WriteAllBytes(filePath, rawFile);
		    }
		}

	    private void FilesVersionsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(CurrentPageChanged))
			{
				if (IsCurrentPage)
				{
					TextMessage = PluginResources.Files_Version_Loading;
					TextMessageVisibility = "Visible";
					TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#00A8EB");
					var selectedFiles = _wizardModel.GsFiles.Where(f => f.IsSelected).ToList();
					if (_oldSelectedFiles.Count == 0)
					{
						AddVersionsToGrid(selectedFiles);
					}
					else
					{
						//get the files which are selected in wizard and they are not in the old list => a new file was selected and we need to download the files versions only for it
						var addedFiles = selectedFiles.Except(_oldSelectedFiles).ToList();
						AddVersionsToGrid(addedFiles);

						//get the removed files
						var removedFiles = _oldSelectedFiles.Except(selectedFiles).ToList();
						RemoveFilesFromGrid(removedFiles);
					}
				}
			}
		}

	    private void RemoveFilesFromGrid(List<GsFile> removedFiles)
	    {
		    foreach (var removedFile in removedFiles)
		    {
			    var fileToBeRemoved =
				    _oldSelectedFiles.FirstOrDefault(f => f.UniqueId.ToString().Equals(removedFile.UniqueId.ToString()));
			    if (fileToBeRemoved != null)
			    {
				    _oldSelectedFiles.Remove(fileToBeRemoved);

				    //remove coresponding files versions for removed files from grid
				    var versionsToRemove = _wizardModel?.FileVersions?.Where(f => f.OriginalFileId.ToString().Equals(removedFile.UniqueId.ToString()))
					    .ToList();
				    {
					    if (versionsToRemove != null)
					    {
						    foreach (var version in versionsToRemove)
						    {
							    _wizardModel?.FileVersions?.Remove(version);
								OnPropertyChanged(nameof(FilesVersions));
						    }
					    }
				    }
				}
			}
	    }

	    private async void AddVersionsToGrid(List<GsFile> selectedFiles)
	    {
			foreach (var selectedFile in selectedFiles)
			{
				_oldSelectedFiles.Add(selectedFile);
				var fileVersions = await _projectService.GetFileVersion(selectedFile.UniqueId.ToString());
				SetFileProperties(selectedFile, fileVersions);
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

	    public ICommand DownloadFilesCommand =>
		    _downloadFilesCommand ?? (_downloadFilesCommand = new CommandHandler(ShowSelectFolderDialog, true));

	    private void ShowSelectFolderDialog()
	    {
		    try
		    {
			    var anySelectedFile = FilesVersions.Any(f => f.IsSelected);
			    if (anySelectedFile)
			    {
				    var folderSelect = new FolderSelectDialog
				    {
					    Title = PluginResources.SelectFolderTitle
				    };
				    if (folderSelect.ShowDialog())
				    {
					    var selectedFolderPath = folderSelect.FileName;
					    if (!string.IsNullOrEmpty(selectedFolderPath))
					    {
						    TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#00A8EB");
							TextMessageVisibility = "Visible";
						    TextMessage = PluginResources.Please_wait_message;

							GroupFilesByFolderStructure(selectedFolderPath);

						    TextMessage = PluginResources.Download_Message;

							ToggleCheckAllFiles(false);
						    Process.Start(selectedFolderPath);
						}
				    }
			    }
		    }
		    catch (Exception ex)
		    {
			    //Here we'll log issue
		    }
	    }

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
			    fileVersion.LanguageCode = selectedFile.LanguageCode;
			    fileVersion.ProjectId = selectedFile.ProjectId;
			    fileVersion.OriginalFileId = selectedFile.UniqueId;
			    _wizardModel?.FileVersions?.Add(fileVersion);
		    }
		    TextMessageVisibility = "Collapsed";
		}
	}
}
