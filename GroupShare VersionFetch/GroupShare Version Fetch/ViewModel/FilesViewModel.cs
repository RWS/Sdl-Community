using NLog;
using Sdl.Community.GSVersionFetch.Commands;
using Sdl.Community.GSVersionFetch.Events;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Community.GSVersionFetch.Service;
using Sdl.Core.Globalization;
using Sdl.Core.Globalization.LanguageRegistry;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Sdl.Community.GSVersionFetch.ViewModel
{
    public class FilesViewModel : ProjectWizardViewModelBase
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ObservableCollection<GsProject> _oldSelectedProjects;
        private readonly ProjectService _projectService;
        private readonly WizardModel _wizardModel;
        private ICommand _clearCommand;
        private string _displayName;
        private bool _isValid;
        private string _searchByFileNameText = string.Empty;
        private string _searchByProjectNameText = string.Empty;
        private string _searchByStatusText = string.Empty;

        public FilesViewModel(WizardModel wizardModel)
        {
            _projectService = new ProjectService();
            _oldSelectedProjects = new ObservableCollection<GsProject>();
            _displayName = "Projects files";
            _wizardModel = wizardModel;
            PropertyChanged += FilesViewModel_PropertyChanged;
            _wizardModel.GsFiles.CollectionChanged += GsFiles_CollectionChanged;
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

        public ObservableCollection<GsFile> GsFiles
        {
            get => _wizardModel?.GsFiles;
            set
            {
                _wizardModel.GsFiles = value;
                OnPropertyChanged(nameof(GsFiles));
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

        public string SearchByFileNameText
        {
            get => _searchByFileNameText;
            set
            {
                _searchByFileNameText = value;
                OnPropertyChanged(nameof(SearchByFileNameText));
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

        public string SearchByStatusText
        {
            get => _searchByStatusText;
            set
            {
                _searchByStatusText = value;
                OnPropertyChanged(nameof(SearchByStatusText));
            }
        }

        public void Filter()
        {
            var filesVersionsView = CollectionViewSource.GetDefaultView(GsFiles);
            filesVersionsView.Filter = fv =>
            {
                if (!(fv is GsFile fileVersion)) return false;

                var isReturnable = fileVersion.ProjectName.ToLower().Contains(SearchByProjectNameText.ToLower());

                isReturnable &= fileVersion.FileName.ToLower().Contains(
                    SearchByFileNameText.ToLower());

                isReturnable &= fileVersion.Status.ToLower().Contains(
                    SearchByStatusText.ToLower());

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

        private async Task AddFilesToGrid(List<GsProject> projects)
        {
            foreach (var project in projects)
            {
                _oldSelectedProjects.Add(project);
                var files = await _projectService.GetProjectFiles(project.ProjectId);
                SetFileProperties(project, files);
            }
        }

        private bool AreAllFilesSelected()
        {
            return GsFiles?.Count > 0 && GsFiles.All(f => f.IsSelected);
        }

        private void Clear(object obj)
        {
            if (!(obj is string objectName)) return;

            switch (objectName)
            {
                case "SearchByProjectName":
                    SearchByProjectNameText = string.Empty;
                    break;

                case "SearchByFileName":
                    SearchByFileNameText = string.Empty;
                    break;

                case "SearchByStatus":
                    SearchByStatusText = string.Empty;
                    break;
            }
        }

        private async void FilesViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            await ResetGrid(e);

            if (e.PropertyName == nameof(SearchByProjectNameText) ||
                e.PropertyName == nameof(SearchByFileNameText) ||
                e.PropertyName == nameof(SearchByStatusText))
            {
                Filter();
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

        private void RemoveFilesFromGrid(List<GsProject> removedProjects)
        {
            foreach (var removedProject in removedProjects)
            {
                var projectToBeRemoved = _oldSelectedProjects.FirstOrDefault(p => p.ProjectId.Equals(removedProject.ProjectId));
                if (projectToBeRemoved != null)
                {
                    _oldSelectedProjects.Remove(projectToBeRemoved);

                    //remove coresponding files for removed project from grid
                    var filesToBeRemoved = _wizardModel?.GsFiles.Where(p => p.ProjectId.Equals(projectToBeRemoved.ProjectId)).ToList();
                    if (filesToBeRemoved != null)
                    {
                        foreach (var file in filesToBeRemoved)
                        {
                            _wizardModel?.GsFiles.Remove(file);
                        }
                    }
                    OnPropertyChanged(nameof(GsFiles));
                }
            }
        }

        private async Task ResetGrid(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentPageChanged))
            {
                if (IsCurrentPage)
                {
                    try
                    {
                        ShowProgress(PluginResources.WaitMessage_FilesView, true);
                        var selectedProjects = _wizardModel.GsProjects.Where(p => p.IsSelected).ToList();
                        if (selectedProjects.Count > 0)
                        {
                            var selectedFilesCount = _wizardModel.GsFiles.Count(f => f.IsSelected);
                            IsValid = selectedFilesCount > 0;

                            var addedProjects = selectedProjects.Except(_oldSelectedProjects).ToList();
                            if (addedProjects.Count > 0)
                            {
                                await AddFilesToGrid(addedProjects);
                            }

                            // get the removed projects
                            var removedProjects = _oldSelectedProjects.Except(selectedProjects).ToList();
                            if (removedProjects.Count > 0)
                            {
                                RemoveFilesFromGrid(removedProjects);
                            }
                        }

                        if (GsFiles.Count == 0)
                            ShowProgress(PluginResources.ProgressBar_FilesVM_NoProjectsMessage);
                        else
                            ShowProgress();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"FilesViewModel_PropertyChanged method: {ex.Message}\n {ex.StackTrace}");
                    }
                }
            }
        }

        private void SetFileProperties(GsProject project, IEnumerable<GsFile> files)
        {
            foreach (var gsFile in files)
            {
                if (gsFile.LanguageCode != project.SourceLanguage)
                {
                    gsFile.ProjectId = project.ProjectId;
                    gsFile.ProjectName = project.Name;
                    gsFile.LanguageFlagImage = LanguageRegistryApi.Instance.GetLanguage(gsFile.LanguageCode).GetFlagImage();
                    gsFile.LanguageName = new Language(gsFile.LanguageCode).DisplayName;

                    var file = _wizardModel.GsFiles.FirstOrDefault(f => f.UniqueId.ToString().Equals(gsFile.UniqueId.ToString()));
                    if (file == null)
                    {
                        _wizardModel?.GsFiles?.Add(gsFile);
                    }
                }
            }
        }

        private void ToggleCheckAllFiles(bool value)
        {
            foreach (var gsFile in GsFiles)
            {
                gsFile.IsSelected = value;
            }
        }
    }
}