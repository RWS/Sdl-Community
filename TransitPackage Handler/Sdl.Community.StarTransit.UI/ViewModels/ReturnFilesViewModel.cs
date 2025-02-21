﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Sdl.Community.StarTransit.Shared.Interfaces;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.UI.Commands;
using Sdl.Community.StarTransit.UI.Helpers;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.StarTransit.UI.ViewModels
{
	public class ReturnFilesViewModel : BaseViewModel
    {
        private readonly ReturnPackage _returnPackage;
        private List<ProjectFile> _projectFiles;
        private ICommand _browseCommand;
        private string _returnPackageLocation;
        private ObservableCollection<CellViewModel> _listView = new ObservableCollection<CellViewModel>();

		public ReturnFilesViewModel(ReturnPackage returnPackage, IMessageBoxService messageBoxService)
		{
			_returnPackage = returnPackage;
			var title = "Please select files for the return package";

			if(returnPackage?.TargetFiles != null && returnPackage.TargetFiles.Count > 0)
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
				Title = title;
			}
			else
			{
				messageBoxService.ShowWarningMessage("Please select a StarTransit project!", "Warning");
			}
		}

        public string Title { get; set; }

        public ObservableCollection<CellViewModel> ProjectListCells
		{
			get => _listView;
			set
			{
				_listView = value;
				OnPropertyChanged(nameof(ProjectListCells));
			}
		}

		public List<ProjectFile> ProjectFiles
		{
			get => _projectFiles;
			set
			{
				_projectFiles = value;
				OnPropertyChanged(nameof(ProjectFiles));
			}
		}

		public ICommand BrowseCommand => _browseCommand ?? (_browseCommand = new CommandHandler(Browse, true));

		public string ReturnPackageLocation
		{
			get => _returnPackageLocation;
			set
			{
				if (Equals(value, _returnPackageLocation))
				{
					return;
				}
				_returnPackageLocation = value;
				OnPropertyChanged(nameof(ReturnPackageLocation));
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
        /// Prj file is kept in StarTransitMetadata folder
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
    }
}