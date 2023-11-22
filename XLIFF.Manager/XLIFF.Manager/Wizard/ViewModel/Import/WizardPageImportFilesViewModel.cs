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
using Sdl.Community.XLIFF.Manager.LanguageMapping.Interfaces;
using Sdl.Community.XLIFF.Manager.LanguageMapping.Model;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Import
{
	public class WizardPageImportFilesViewModel : WizardPageViewModelBase, IDisposable
	{
		private readonly IDialogService _dialogService;
		private readonly ILanguageProvider _languageProvider;
		private List<MappedLanguage> _languageMappings;
		private ICommand _clearSelectedComand;
		private ICommand _checkSelectedComand;
		private ICommand _addFolderCommand;
		private ICommand _addFilesCommand;
		private ICommand _checkAllCommand;
		private ICommand _dragDropCommand;
		private IList _selectedProjectFiles;
		private List<ProjectFile> _projectFiles;
		private bool _checkedAll;
		private bool _checkingAllAction;

		public WizardPageImportFilesViewModel(Window owner, object view, WizardContext wizardContext,
			IDialogService dialogService, ILanguageProvider languageProvider) : base(owner, view, wizardContext)
		{
			_dialogService = dialogService;
			_languageProvider = languageProvider;
			ProjectFiles = wizardContext.ProjectFiles;
			VerifyProjectFiles();
			VerifyIsValid();

			_languageMappings = _languageProvider.GetMappedLanguages(false);

			LoadPage += OnLoadPage;
			LeavePage += OnLeavePage;
		}

		public ICommand ClearSelectedCommand => _clearSelectedComand ?? (_clearSelectedComand = new CommandHandler(ClearSelected));

		public ICommand CheckSelectedCommand => _checkSelectedComand ?? (_checkSelectedComand = new CommandHandler(CheckSelected));

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
			System.Windows.Forms.SendKeys.Send("{TAB}");
		}

		private int? GetValidProjectFilesCount()
		{
			var count = ProjectFiles.Count(a => a.Selected &&
						!string.IsNullOrEmpty(a.XliffFilePath) && File.Exists(a.XliffFilePath));

			return count;
		}

		private void VerifyProjectFiles()
		{
			foreach (var projectFile in ProjectFiles)
			{
				if (projectFile.Action == Enumerators.Action.Import)
				{
					var activityfile = projectFile.ProjectFileActivities.OrderByDescending(a => a.Date).FirstOrDefault(a => a.Action == Enumerators.Action.Import);

					projectFile.Status = Enumerators.Status.Warning;
					projectFile.ShortMessage = string.Format(PluginResources.Message_Imported_on_0, activityfile?.DateToString);
				}
				else
				{
					projectFile.Status = Enumerators.Status.Ready;
					projectFile.ShortMessage = string.Empty;
					projectFile.Report = string.Empty;
				}

				projectFile.XliffFilePath = string.Empty;
			}
		}

		private void AddFiles()
		{
			var selectedFiles = _dialogService.ShowFileDialog(
				"Xliff (*.xliff) |*.xliff;*.xlf",
				PluginResources.FilesDialog_Title,
				GetDefaultPath());
			AddFilesToGrid(selectedFiles);
		}

		private string GetDefaultPath()
		{
			var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

			var studioFolder = Path.Combine(path, "Studio 2019");
			if (Directory.Exists(studioFolder))
			{
				path = studioFolder;

				var projectsFolder = Path.Combine(path, "Projects");
				if (Directory.Exists(projectsFolder))
				{
					path = projectsFolder;
				}
			}

			return path;
		}

		private void UpdateCheckAll()
		{
			CheckedAll = ProjectFiles.Count == ProjectFiles.Count(a => a.Selected);
			OnPropertyChanged(nameof(StatusLabel));
		}

		private void ClearSelected(object parameter)
		{
			if (SelectedProjectFiles == null)
			{
				return;
			}

			foreach (var selectedFile in SelectedProjectFiles.Cast<ProjectFile>())
			{
				selectedFile.XliffFilePath = string.Empty;
			}

			UpdateCheckAll();
			VerifyIsValid();
		}

		private void CheckSelected(object parameter)
		{
			if (SelectedProjectFiles == null)
			{
				return;
			}

			var isChecked = Convert.ToBoolean(parameter);
			foreach (var selectedFile in SelectedProjectFiles.Cast<ProjectFile>())
			{
				selectedFile.Selected = isChecked;
			}

			UpdateCheckAll();
			VerifyIsValid();
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

		private void OnLeavePage(object sender, EventArgs e)
		{
			foreach (var projectFile in ProjectFiles)
			{
				if (!projectFile.Selected)
				{
					continue;
				}

				if (string.IsNullOrEmpty(projectFile.XliffFilePath) || !File.Exists(projectFile.XliffFilePath))
				{
					projectFile.Selected = false;
				}
			}
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
					else
					{
						AddFilesToGrid(new List<string> { fullPath });
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
				if (!string.IsNullOrEmpty(filePath) && !File.Exists(filePath))
				{
					continue;
				}

				var xliff = xliffReader.ReadXliff(filePath);
				var docInfoSourceFile = xliff.DocInfo?.Source;
				var xliffTargetLanguage = xliff.Files?.FirstOrDefault()?.TargetLanguage;
				// take the default target from <doc-info> when not specified in <File>
				if (string.IsNullOrEmpty(xliffTargetLanguage))
				{
					xliffTargetLanguage = xliff.DocInfo?.TargetLanguage;
				}

				// Get the mapped language code
				var mappedLanguage = _languageMappings.FirstOrDefault(a =>
					string.Compare(a.LanguageCode, xliffTargetLanguage, StringComparison.CurrentCultureIgnoreCase) == 0
					|| string.Compare(a.MappedCode, xliffTargetLanguage, StringComparison.CurrentCultureIgnoreCase) == 0);

				if (mappedLanguage == null)
				{
					MessageBox.Show(string.Format(PluginResources.WarningMessage_UnableToMapLanguage, xliffTargetLanguage),
						PluginResources.XLIFFManager_Name, MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				// Assign the Trados Studio language code
				xliffTargetLanguage = mappedLanguage.LanguageCode;
				var projectLanguages = GetProjectLanguages(xliff.DocInfo?.TargetLanguage);

				var xliffTargetPath = GetPathLocation(docInfoSourceFile, projectLanguages, out var foundTargetLanguage);
				if (!foundTargetLanguage)
				{
					continue;
				}

				foreach (var projectFile in ProjectFiles)
				{
					if (string.Compare(projectFile.Project.Id, xliff.DocInfo?.ProjectId, StringComparison.CurrentCultureIgnoreCase) != 0)
					{
						MessageBox.Show(PluginResources.WizardMessage_ProjectIdMissmatch, PluginResources.XLIFFManager_Name, MessageBoxButton.OK, MessageBoxImage.Warning);
						return;
					}

					var projectFileTargetLanguage = projectFile.TargetLanguage;
					if (string.Compare(projectFileTargetLanguage, xliffTargetLanguage, StringComparison.CurrentCultureIgnoreCase) != 0)
					{
						continue;
					}
					
					var projectFileTargetPath = GetPathLocation(projectFile.Location, xliffTargetLanguage, out var foundXliffTargetLanguage);
					if (!foundXliffTargetLanguage)
					{
						continue;
					}

					if (string.Compare(projectFileTargetPath, xliffTargetPath, StringComparison.CurrentCultureIgnoreCase) == 0)
					{
						projectFile.XliffFilePath = filePath;
						projectFile.Selected = true;
					}
				}
			}

			VerifyIsValid();
		}

		private List<string> GetProjectLanguages(string targetLanguage)
		{
			var projectLanguages = WizardContext.Project.TargetLanguages.Select(a => a.CultureInfo.Name).ToList();
			if (!string.IsNullOrEmpty(targetLanguage) && !projectLanguages.Contains(targetLanguage))
			{
				projectLanguages.Insert(0, targetLanguage);
			}

			if (!projectLanguages.Contains(WizardContext.Project.SourceLanguage.CultureInfo.Name))
			{
				projectLanguages.Add(WizardContext.Project.SourceLanguage.CultureInfo.Name);
			}

			return projectLanguages;
		}

		private string GetPathLocation(string path, string targetLanguage, out bool foundLanguage)
		{			
			var location = string.Empty;
			while (path.Contains("\\"))
			{
				var part = path.Substring(path.LastIndexOf("\\", StringComparison.Ordinal) + 1);
				if (string.Compare(part, targetLanguage, StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					foundLanguage = true;
					return location;
				}

				location = Path.Combine(part, location);
				path = path.Substring(0, path.LastIndexOf("\\", StringComparison.Ordinal));
			}

			foundLanguage = false;
			return null;
		}

		private string GetPathLocation(string path, List<string> targetLanguages, out bool foundLanguage)
		{
			foreach (var targetLanguage in targetLanguages)
			{
				var location = GetPathLocation(path, targetLanguage, out var found);
				if (found)
				{
					foundLanguage = true;
					return location;
				}
			}

			foundLanguage = false;
			return null;
		}

		private IEnumerable<string> GetAllXliffsFromDirectory(string directoryPath)
		{
			var files = new List<string>();
			files.AddRange(Directory.GetFiles(directoryPath, "*.xliff", SearchOption.AllDirectories).ToList());
			files.AddRange(Directory.GetFiles(directoryPath, "*.xlf", SearchOption.AllDirectories).ToList());

			return files;
		}

		private void SelectFolder()
		{
			var folderPath = _dialogService.ShowFolderDialog(PluginResources.FolderDialog_Title, GetDefaultPath());
			if (!string.IsNullOrEmpty(folderPath) && Directory.Exists(folderPath))
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
			LeavePage -= OnLeavePage;
		}
	}
}
