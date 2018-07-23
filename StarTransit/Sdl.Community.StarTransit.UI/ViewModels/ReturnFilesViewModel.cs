using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
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
        private ObservableCollection<CellViewModel> _listView = new ObservableCollection<CellViewModel>();
        private CellViewModel _selectedItem;

		public ReturnFilesViewModel(ReturnPackage returnPackage, ReturnPackageMainWindow window)
		{
			_returnPackage = returnPackage;
			_window = window;
			_title = "Please select files for the return package";

			if (returnPackage.TargetFiles.Count > 0)
			{
				var xliffFiles = returnPackage.TargetFiles.Where(file => file.Name.EndsWith(".sdlxliff")).ToList();
				if (xliffFiles.Count() != 0)
				{
					foreach (var project in xliffFiles)
					{
						var item = new CellViewModel
						{
							Id = project.Id,
							Name = project.Name,
							Checked = false
						};

						_listView.Add(item);
					}
					ProjectFiles = xliffFiles;
				}
				else
				{
					ProjectFiles = new List<ProjectFile>();
				}
				Title = _title;
			}
			else
			{
				MessageBox.Show("Please select target files!", "Informative message", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

        public string Title { get; set; }

        public ObservableCollection<CellViewModel> ProjectListCells
        {
            get { return _listView; }
            set
            {
                if (Equals(value, _listView)){ return;}
                _listView = value;
                OnPropertyChanged();

            }
        }


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

        private void Browse()
        {
            var folderDialog = new FolderSelectDialog();
            if (folderDialog.ShowDialog())
            {

                ReturnPackageLocation = folderDialog.FileName;
                _returnPackage.FolderLocation = folderDialog.FileName;
            }
        }

        public ReturnPackage GetReturnPackage()
        {
          
            var selectedProjectsIds = CellViewModel.ReturnSelectedProjectIds();
            var selectedFiles = new List<ProjectFile>();

            var pathToProjFile = PathToPrjFile(_returnPackage.ProjectLocation);
            _returnPackage.PathToPrjFile = pathToProjFile;
            foreach (var id in selectedProjectsIds)
            {
                var selectedFile = ProjectFiles.FirstOrDefault(file => file.Id == id);
                selectedFiles.Add(selectedFile);
            }
            _returnPackage.TargetFiles = selectedFiles;
           

            return _returnPackage;
        }

        /// <summary>
        /// Gets the path to prj file in order to add to return package archive later
        /// Prj fille is kept in StarTransitMetadata folder
        /// </summary>
        /// <param name="pathToProject"></param>
        /// <returns></returns>
        private string PathToPrjFile(string pathToProject)
        {
            var prjPath = Path.Combine(pathToProject.Substring(0,pathToProject.LastIndexOf(@"\", StringComparison.Ordinal)), "StarTransitMetadata");
            if (Directory.Exists(prjPath))
            {
                var filesPath = Directory.GetFiles(prjPath).ToList();
                var prj= filesPath.FirstOrDefault(p => p.EndsWith("PRJ"));
                return prj;
            }
            return string.Empty;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}