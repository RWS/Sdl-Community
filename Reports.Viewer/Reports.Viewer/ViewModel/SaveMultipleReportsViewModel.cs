using Reports.Viewer.Plus.Commands;
using Reports.Viewer.Plus.Service;
using Sdl.ProjectAutomation.Core;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace Reports.Viewer.Plus.ViewModel
{
    public class SaveMultipleReportsViewModel : INotifyPropertyChanged
    {
        private readonly IProject _project;
        private ICommand _clearPathCommand;
        private ICommand _browseFolderCommand;
        private string _path;
        private string _selectedFormat;
        private bool _createSubFolders;
        private bool _isValid;
        private DialogService _dialogService;

        public event PropertyChangedEventHandler PropertyChanged;

        public SaveMultipleReportsViewModel(
            IProject project, 
            DialogService dialogService)
        {
            _project = project;            
            _dialogService = dialogService;
        }

        public string WindowTitle => PluginResources.WindowTitle_SaveProjectReports;

        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                OnPropertyChanged(nameof(Path));
                Validate();
            }
        }

        public ObservableCollection<string> SupportedFormats => new ObservableCollection<string>() { "xlsx", "html", "mht", "xml" };

        public bool CreateSubFolders
        {
            get => _createSubFolders;
            set
            {
                _createSubFolders = value;
                OnPropertyChanged(nameof(CreateSubFolders));
            }
        }

        public string SelectedFormat
        {
            get => _selectedFormat;
            set
            {
                _selectedFormat = value;
                OnPropertyChanged(nameof(SelectedFormat));
                Validate();
            }
        }

        public bool IsValid 
        {
            get => _isValid;
            set
            {
                _isValid = value;
                OnPropertyChanged(nameof(IsValid));
            }
        }

        public ICommand ClearPathCommand => _clearPathCommand ?? (_clearPathCommand = new CommandHandler(ClearPath));

        public ICommand BrowseFolderCommand => _browseFolderCommand ?? (_browseFolderCommand = new CommandHandler(BrowseFolder));

        private void ClearPath(object parameter)
        {
            if (parameter.ToString() == "path")
            {
                Path = string.Empty;
            }
        }

        private void BrowseFolder(object parameter)
        {
            // Last thing is the folder
            var projectInfo = _project.GetProjectInfo();
            Path = _dialogService.ShowFolderDialog("Select Folder", !string.IsNullOrEmpty(Path) ? GetValidFolderPath(Path) : projectInfo.LocalProjectFolder);
        }

        private string GetValidFolderPath(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                return string.Empty;
            }

            var folder = directory;
            if (Directory.Exists(folder))
            {
                return folder;
            }

            while (folder.Contains("\\"))
            {
                folder = folder.Substring(0, folder.LastIndexOf("\\", StringComparison.Ordinal));
                if (Directory.Exists(folder))
                {
                    return folder;
                }
            }

            return folder;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Validate()
        => IsValid = !string.IsNullOrEmpty(Path) && !string.IsNullOrEmpty(SelectedFormat);
    }
}
