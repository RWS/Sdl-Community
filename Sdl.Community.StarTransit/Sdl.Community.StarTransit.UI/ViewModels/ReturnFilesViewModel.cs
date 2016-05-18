using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.UI.Annotations;
using Sdl.Community.StarTransit.UI.Controls;
using Sdl.Community.StarTransit.UI.Helpers;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.StarTransit.UI.ViewModels
{
    public class ReturnFilesViewModel :INotifyPropertyChanged
    {
        private ReturnPackage _returnPackage;
        private string _title;
        private List<ProjectFile> _projectFiles;
        private ICommand _browseCommand;
        private string _returnPackageLocation;
        private ReturnPackageMainWindow _window;

        public ReturnFilesViewModel(ReturnPackage returnPackage, ReturnPackageMainWindow window)
        {
            _returnPackage = returnPackage;
            _window = window;
            _title = "Please select files for the return package";
            Title = _title;
            ProjectFiles = returnPackage.TargetFiles;
        }

        public string Title { get; set; }

        public List<ProjectFile> ProjectFiles
        {
            get { return _projectFiles; }
            set
            {
                if (Equals(value, _projectFiles))
                {
                    return;
                }
                _projectFiles = value;
                OnPropertyChanged();
            }
        }

        public ICommand BrowseCommand
        {
            get { return _browseCommand ?? (_browseCommand = new CommandHandler(Browse, true)); }
        }

        public string ReturnPackageLocation
        {
            get { return _returnPackageLocation; }
            set {
                if (Equals(value, _returnPackageLocation))
                {
                    return;
                }
                _returnPackageLocation = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// TO DO: Do we need this button?Or set the return folder path in the studio project folder?
        /// 
        /// </summary>
        private async void Browse()
        {
            var folderDialog = new FolderSelectDialog();
            if (folderDialog.ShowDialog())
            {

                bool isEmpty = !Directory.EnumerateFiles(folderDialog.FileName).Any();
                var hasSubdirectories = Directory.GetDirectories(folderDialog.FileName);
                if (hasSubdirectories.Count() != 0 || !isEmpty)
                {
                    var dialog = new MetroDialogSettings
                    {
                        AffirmativeButtonText = "OK"

                    };
                    MessageDialogResult result =
                        await _window.ShowMessageAsync("Folder not empty!", "Please select an empty folder",
                            MessageDialogStyle.Affirmative, dialog);
                }
                else
                {
                    ReturnPackageLocation = folderDialog.FileName;
                    //_returnPackage.Location = ReturnPackageLocation;
                }
            }
        }

        public ReturnPackage GetReturnPackage()
        {
            return _returnPackage;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
