using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Readers;
using Sdl.Community.XLIFF.Manager.Interfaces;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Import
{
	public class WizardPageImportFilesViewModel : WizardPageViewModelBase, IDisposable
	{
		private readonly IDialogService _dialogService;
		private ICommand _addFolderCommand;
		private ICommand _addFilesCommand;
		private ICommand _checkAllCommand;
		private ICommand _dragDropCommand;
		private IList _selectedProjectFiles;
		private List<ProjectFile> _projectFiles;
		private bool _checkedAll;
		private bool _checkingAllAction;

		public WizardPageImportFilesViewModel(Window owner, object view, WizardContext wizardContext,
			IDialogService dialogService) : base(owner, view, wizardContext)
		{
			_dialogService = dialogService;
			ProjectFiles = wizardContext.ProjectFiles;
			VerifyProjectFiles();
			VerifyIsValid();

			LoadPage += OnLoadPage;
		}

		public ICommand AddFolderCommand => _addFolderCommand ?? (_addFolderCommand = new RelayCommand(SelectFolder));
		public ICommand AddFilesCommand => _addFilesCommand ?? (_addFilesCommand = new RelayCommand(AddFiles));
		public ICommand CheckAllCommand => _checkAllCommand ?? (_checkAllCommand = new RelayCommand(CheckAll));
		public ICommand DragDropCommand => _dragDropCommand ?? (_dragDropCommand = new CommandHandler(DragAndDrop));

		public IList SelectedProjectFiles
		{
			get => _selectedProjectFiles ?? (_selectedProjectFiles = new ObservableCollection<ProjectFile>());
			set
			{
				_selectedProjectFiles = value;
				OnPropertyChanged(nameof(SelectedProjectFiles));
			}
		}

		public List<ProjectFile> ProjectFiles
		{
			get => _projectFiles ?? (_projectFiles = new List<ProjectFile>());
			set
			{
				if (_projectFiles != null)
				{
					foreach (var projectFile in _projectFiles)
					{
						projectFile.PropertyChanged -= ProjectFile_PropertyChanged;
					}
				}

				_projectFiles = value;

				if (_projectFiles != null)
				{
					foreach (var projectFile in _projectFiles)
					{
						projectFile.PropertyChanged += ProjectFile_PropertyChanged;
					}
				}

				OnPropertyChanged(nameof(StatusLabel));
			}
		}

		public bool CheckedAll
		{
			get => _checkedAll;
			set
			{
				_checkedAll = value;
				OnPropertyChanged(nameof(CheckedAll));
			}
		}
		public string StatusLabel
		{
			get
			{
				var message = string.Format(PluginResources.StatusLabel_Files_0_Selected_1,
					ProjectFiles?.Count,
					GetValidProjectFilesCount());
				return message;
			}
		}
		public override string DisplayName => PluginResources.PageName_Files;

		public override bool IsValid { get; set; }

		public void VerifyIsValid()
		{
			IsValid = GetValidProjectFilesCount() > 0;
		}

		private int? GetValidProjectFilesCount()
		{
			var count = ProjectFiles.Count(a => a.Selected &&
			                        !string.IsNullOrEmpty(a.XliffFilePath) &&
			                        File.Exists(a.XliffFilePath));

			return count;
		}

		private void VerifyProjectFiles()
		{
			foreach (var projectFile in ProjectFiles)
			{
				if (projectFile.Action == Enumerators.Action.Import)
				{
					var activityfile = projectFile.ProjectFileActivities.FirstOrDefault(a => a.Action == Enumerators.Action.Import);

					projectFile.Status = Enumerators.Status.Warning;
					projectFile.ShortMessage = PluginResources.Message_File_already_imported;
					projectFile.Details = string.Format(PluginResources.Message_Imported_on_0, activityfile?.DateToString) + Environment.NewLine;
					projectFile.Details += string.Format(PluginResources.Message_File_Path_0, projectFile.XliffFilePath);
				}
				else
				{
					projectFile.Status = Enumerators.Status.Ready;
					projectFile.ShortMessage = string.Empty;
					projectFile.Details = string.Empty;
				}

				projectFile.XliffFilePath = string.Empty;
			}
		}

		private void AddFiles()
		{
			var selectedFiles = _dialogService.ShowFileDialog("Xliff (*.xliff) |*.xliff", PluginResources.FilesDialog_Title);
			AddFilesToGrid(selectedFiles);
		}

		private void UpdateCheckAll()
		{
			CheckedAll = ProjectFiles.Count == ProjectFiles.Count(a => a.Selected);
			OnPropertyChanged(nameof(StatusLabel));
		}

		private void CheckAll()
		{
			try
			{
				_checkingAllAction = true;

				var value = CheckedAll;
				foreach (var file in ProjectFiles)
				{
					file.Selected = value;
				}

				VerifyIsValid();
				OnPropertyChanged(nameof(CheckedAll));
				OnPropertyChanged(nameof(StatusLabel));
			}
			finally
			{
				_checkingAllAction = false;
			}
		}

		private void ProjectFile_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (!_checkingAllAction && e.PropertyName == nameof(ProjectFile.Selected))
			{
				UpdateCheckAll();
			}

			VerifyIsValid();
		}
		private void OnLoadPage(object sender, EventArgs e)
		{
			UpdateCheckAll();
			VerifyIsValid();
		}

		private void DragAndDrop(object parameter)
		{
			if (parameter == null || !(parameter is DragEventArgs eventArgs))
			{
				return;
			}

			var fileDrop = eventArgs.Data.GetData(DataFormats.FileDrop, false);
			if (fileDrop is string[] filesOrDirectories && filesOrDirectories.Length > 0)
			{
				foreach (var fullPath in filesOrDirectories)
				{
					var fileAttributes = File.GetAttributes(fullPath);
					if (fileAttributes.HasFlag(FileAttributes.Directory))
					{
						var files = GetAllXliffsFromDirectory(fullPath);
						AddFilesToGrid(files);
					}
				}
			}
		}

		private void AddFilesToGrid(IEnumerable<string> filesPath)
		{
			var sniffer = new XliffSniffer();
			var segmentBuilder = new SegmentBuilder();
			var xliffReader = new XliffReder(sniffer, segmentBuilder);

			foreach (var filePath in filesPath)
			{
				var xliff = xliffReader.ReadXliff(filePath);
				var xliffTargetLanguage = xliff.Files.FirstOrDefault()?.TargetLanguage;
				var xliffTargetPath = GetPathLocation(xliff.DocInfo.Source, xliffTargetLanguage);

				foreach (var projectFile in ProjectFiles)
				{
					if (string.Compare(projectFile.Project.Id, xliff.DocInfo?.ProjectId, StringComparison.CurrentCultureIgnoreCase) != 0)
					{
						MessageBox.Show(PluginResources.WizardMessage_ProjectIdMissmatch, 
							PluginResources.XLIFFManager_Name, MessageBoxButton.OK, MessageBoxImage.Warning);
						return;
					}

					var projectFileTargetLanguage = projectFile.TargetLanguage.CultureInfo.Name;
					var projectFileTargetPath = GetPathLocation(projectFile.Location, xliffTargetLanguage);

					if (string.Compare(projectFileTargetLanguage, xliffTargetLanguage, StringComparison.CurrentCultureIgnoreCase) == 0 &&
						string.Compare(projectFileTargetPath, xliffTargetPath, StringComparison.CurrentCultureIgnoreCase) == 0)
					{
						projectFile.XliffFilePath = filePath;
					}
				}
			}

			VerifyIsValid();
		}

		private string GetPathLocation(string path, string targetLanguage)
		{
			var location = string.Empty;
			while (path.Contains("\\"))
			{
				var part = path.Substring(path.LastIndexOf("\\", StringComparison.Ordinal) + 1);
				if (string.Compare(part, targetLanguage, StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					break;
				}

				location = Path.Combine(part, location);
				path = path.Substring(0, path.LastIndexOf("\\", StringComparison.Ordinal));
			}

			return location;
		}

		private IEnumerable<string> GetAllXliffsFromDirectory(string directoryPath)
		{
			return Directory.GetFiles(directoryPath, "*.xliff", SearchOption.AllDirectories).ToList();
		}

		private void SelectFolder()
		{
			var folderPath = _dialogService.ShowFolderDialog(PluginResources.FolderDialog_Title);
			if (string.IsNullOrEmpty(folderPath) && Directory.Exists(folderPath))
			{
				var files = GetAllXliffsFromDirectory(folderPath);
				AddFilesToGrid(files);
			}
		}

		public void Dispose()
		{
			if (ProjectFiles != null)
			{
				foreach (var projectFile in ProjectFiles)
				{
					projectFile.PropertyChanged -= ProjectFile_PropertyChanged;
				}
			}

			LoadPage -= OnLoadPage;
		}
	}
}
