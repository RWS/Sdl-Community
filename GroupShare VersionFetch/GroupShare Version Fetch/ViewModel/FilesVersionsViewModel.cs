using NLog;
using Sdl.Community.GSVersionFetch.Commands;
using Sdl.Community.GSVersionFetch.Events;
using Sdl.Community.GSVersionFetch.Helpers;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Community.GSVersionFetch.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Sdl.Community.GSVersionFetch.ViewModel
{
    public class FilesVersionsViewModel : ProjectWizardViewModelBase
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ObservableCollection<GsFile> _oldSelectedFiles;
        private readonly ProjectService _projectService;
        private readonly WizardModel _wizardModel;
        private ICommand _clearCommand;
        private string _displayName;
        private ICommand _downloadFilesCommand;
        private ICommand _enterCommand;
        private bool _isValid;
        private string _searchByAuthorText = string.Empty;
        private string _searchByCommentText = string.Empty;
        private string _searchByProjectNameText = string.Empty;
        private string _selectedVersion;
        private string _textMessage;
        private SolidColorBrush _textMessageBrush;
        private string _textMessageVisibility;

        public FilesVersionsViewModel(WizardModel wizardModel)
        {
            _wizardModel = wizardModel;
            _displayName = "Files versions";
            _projectService = new ProjectService();
            _oldSelectedFiles = new ObservableCollection<GsFile>();
            PropertyChanged += FilesVersionsViewModel_PropertyChanged;
            _wizardModel.FileVersions.CollectionChanged += FileVersions_CollectionChanged;
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

        public ICommand ClearCommand => _clearCommand ?? (_clearCommand = new ParameterCommand(Clear));

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

        public ICommand DownloadFilesCommand =>
            _downloadFilesCommand ?? (_downloadFilesCommand = new CommandHandler(ShowSelectFolderDialog, true));

        public ICommand EnterCommand => _enterCommand ?? (_enterCommand = new CommandHandler(SelectSpecificVersion, true));

        public ObservableCollection<GsFileVersion> FilesVersions
        {
            get => _wizardModel.FileVersions;
            set
            {
                _wizardModel.FileVersions = value;
                OnPropertyChanged(nameof(FilesVersions));
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

        public string SearchByAuthorText
        {
            get => _searchByAuthorText;
            set
            {
                _searchByAuthorText = value;
                OnPropertyChanged(nameof(SearchByAuthorText));
            }
        }

        public string SearchByCommentText
        {
            get => _searchByCommentText;
            set
            {
                _searchByCommentText = value;
                OnPropertyChanged(nameof(SearchByCommentText));
            }
        }

        public string SearchByProjectNameText
        {
            get => _searchByProjectNameText;
            set
            {
                _searchByProjectNameText = value;
                OnPropertyChanged(nameof(SearchByProjectNameText));
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

        public string TextMessage
        {
            get => _textMessage;
            set
            {
                _textMessage = value;
                OnPropertyChanged(nameof(TextMessage));
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

        public string TextMessageVisibility
        {
            get => _textMessageVisibility;
            set
            {
                _textMessageVisibility = value;
                OnPropertyChanged(nameof(TextMessageVisibility));
            }
        }

        public void Filter()
        {
            var filesVersionsView = CollectionViewSource.GetDefaultView(FilesVersions);
            filesVersionsView.Filter = fv =>
            {
                if (!(fv is GsFileVersion fileVersion)) return false;

                var isReturnable = fileVersion.CreatedBy.ToLower().Contains(
                    SearchByAuthorText.ToLower());

                isReturnable &= fileVersion.CheckInComment.ToLower().Contains(
                    SearchByCommentText.ToLower());

                isReturnable &= fileVersion.ProjectName.ToLower().Contains(SearchByProjectNameText.ToLower());

                return isReturnable;
            };
        }

        public override bool OnChangePage(int position, out string message)
        {
            message = string.Empty;

            var pagePosition = PageIndex - 1;
            if (position == pagePosition)
            {
                return false;
            }

            if (!IsValid && position > pagePosition)
            {
                message = PluginResources.UnableToNavigateToSelectedPage + Environment.NewLine + Environment.NewLine +
                          string.Format(PluginResources.The_data_on__0__is_not_valid, _displayName);
                return false;
            }

            return true;
        }

        private static void ShowProgress(string message = null, bool showRing = false)
        {
            AppInitializer.PublishEvent(new ProgressEvent(message, showRing));
        }

        private async Task AddVersionsToGrid(List<GsFile> selectedFiles)
        {
            foreach (var selectedFile in selectedFiles)
            {
                _oldSelectedFiles.Add(selectedFile);
                var fileVersions = await _projectService.GetFileVersion(selectedFile.UniqueId.ToString());
                SetFileProperties(selectedFile, fileVersions);
            }
        }

        private bool AreAllFilesSelected()
        {
            return FilesVersions?.Count > 0 && FilesVersions.All(f => f.IsSelected);
        }

        private void Clear(object obj)
        {
            if (!(obj is string objectName)) return;

            switch (objectName)
            {
                case "SearchByComment":
                    SearchByCommentText = string.Empty;
                    break;

                case "SearchByAuthor":
                    SearchByAuthorText = string.Empty;
                    break;

                case "SearchByProjectName":
                    SearchByProjectNameText = string.Empty;
                    break;
            }
        }

        private async void FilesVersionsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentPageChanged))
            {
                await OnFileVersionsChanged();
            }

            if (e.PropertyName == nameof(SearchByProjectNameText) || e.PropertyName == nameof(SearchByCommentText) || e.PropertyName == nameof(SearchByAuthorText))
            {
                Filter();
            }
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

        private async Task GetProjectFiles()
        {
            var selectedProjects = _wizardModel.GsProjects.Where(p => p.IsSelected).ToList();
            foreach (var project in selectedProjects)
            {
                var files = await _projectService.GetProjectFiles(project.ProjectId).ConfigureAwait(true);
                project.SetFileProperties(_wizardModel, files, true);
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

        private bool IsValidVersion(string version)
        {
            if (string.IsNullOrEmpty(version))
            {
                return true;
            }
            if (int.TryParse(version, out _))
            {
                return true;
            }

            ShowMessage(PluginResources.Version_Validation, "#FF00");
            return false;
        }

        private async Task OnFileVersionsChanged()
        {
            if (IsCurrentPage)
            {
                ShowProgress(PluginResources.Files_Version_Loading, true);

                await GetProjectFiles().ConfigureAwait(true);
                var selectedFiles = _wizardModel?.GsFiles?.Where(f => f.IsSelected).ToList();

                if (selectedFiles?.Count > 0)
                {
                    //get the files which are selected in wizard and they are not in the old list => a new file was selected and we need to download the files versions only for it
                    var addedFiles = selectedFiles.Except(_oldSelectedFiles).ToList();
                    if (addedFiles.Count > 0)
                    {
                        await AddVersionsToGrid(addedFiles);
                    }

                    //get the removed files
                    var removedFiles = _oldSelectedFiles.Except(selectedFiles).ToList();
                    if (removedFiles.Count > 0)
                    {
                        RemoveFilesFromGrid(removedFiles);
                    }
                }

                if (_wizardModel != null && _wizardModel.FileVersions.Count == 0)
                    ShowProgress(PluginResources.ProgressBar_FileVersionsVM_NoProjectsMessage);
                else
                    ShowProgress();
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

        private async void SaveFiles(string folderPath, List<GsFileVersion> files)
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
            {   //for empty box we'll deselect all boxes
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
        }

        private void ShowMessage(string message, string color)
        {
            TextMessage = message;
            TextMessageVisibility = "Visible";
            TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(color);
        }

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
                            ShowProgress(PluginResources.Please_wait_message, true);

                            GroupFilesByFolderStructure(selectedFolderPath);

                            TextMessage = PluginResources.Download_Message;

                            ToggleCheckAllFiles(false);
                            Process.Start(selectedFolderPath);

                            ShowProgress();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"ShowSelectFolderDialog method: {ex.Message}\n {ex.StackTrace}");
            }
        }

        private void ToggleCheckAllFiles(bool value)
        {
            foreach (var fileVersion in FilesVersions)
            {
                fileVersion.IsSelected = value;
            }
        }
    }
}