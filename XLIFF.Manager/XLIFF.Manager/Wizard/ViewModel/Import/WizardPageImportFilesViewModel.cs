using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Readers;
using Sdl.Community.XLIFF.Manager.Interfaces;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Import
{
	public class WizardPageImportFilesViewModel : WizardPageViewModelBase,IDisposable
	{
		private ICommand _addFolderCommand;
		private ICommand _addFilesCommand;
		private ICommand _checkAllCommand;
		private ICommand _dragDropCommand;
		private IList _selectedProjectFiles;
		private readonly IDialogService _dialogService;
		private List<ProjectFile> _projectFiles;
		private bool _checkedAll;
		private bool _checkingAllAction;

		public WizardPageImportFilesViewModel(Window owner, object view, WizardContext wizardContext,
			IDialogService dialogService) : base(owner, view, wizardContext)
		{
			_dialogService = dialogService;
			ProjectFiles = wizardContext.ProjectFiles;

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
					_projectFiles?.Count,
					_projectFiles?.Count(a => a.Selected));
				return message;
			}
		}
		public override string DisplayName => PluginResources.PageName_Files;
		public override bool IsValid { get; set; }

		public void VerifyIsValid()
		{
			IsValid = ProjectFiles.Count(a => a.Selected) > 0; //TODO: We also need to verify it checked file has an xliff file added for import
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
			if (parameter == null || !(parameter is DragEventArgs eventArgs)) return;

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
					//is file
					AddFilesToGrid(new List<string>{fullPath});
				}
			}
		}

		private void AddFilesToGrid(List<string> filesPath)
		{
			//TODO: Check if the file type is correct (polyglot, xlf)

			//BUG if we add files and click on cancel, and select import the added files column is populated with the files added before
			//TODO: If user close the import wizard/cancel -> remove added files
			var sniffer = new XliffSupportSniffer();
			var segmentBuilder = new SegmentBuilder();
			var xliffReader = new XliffReder(sniffer, segmentBuilder);
			foreach (var filePath in filesPath)
			{
				var reader = xliffReader.ReadXliff(filePath);

				var correspondingFile = ProjectFiles.FirstOrDefault(p =>
					p.Location.Equals(reader.DocInfo.Source) && p.TargetLanguage.CultureInfo.Name.Equals(reader.DocInfo.TargetLanguage));
				if (correspondingFile != null)
				{
					correspondingFile.ImportedFilePath = filePath;
				}
			}
		}

		private List<string> GetAllXliffsFromDirectory(string directoryPath)
		{
			return Directory.GetFiles(directoryPath, "*.xliff",
				SearchOption.AllDirectories).ToList();
		}

		private void SelectFolder()
		{
			var folderPath = _dialogService.ShowFolderDialog(PluginResources.FolderDialog_Title);
			var files = GetAllXliffsFromDirectory(folderPath);
			AddFilesToGrid(files);
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
