using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DocumentFormat.OpenXml.Packaging;
using Sdl.Versioning;
using Trados.Transcreate.Commands;
using Trados.Transcreate.Common;
using Trados.Transcreate.Interfaces;
using Trados.Transcreate.Model;

namespace Trados.Transcreate.Wizard.ViewModel.Import
{
	public class WizardPageImportFilesViewModel : WizardPageViewModelBase, IDisposable
	{
		private readonly IDialogService _dialogService;
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
		private string _productName;

		public WizardPageImportFilesViewModel(Window owner, object view, TaskContext taskContext,
			IDialogService dialogService) : base(owner, view, taskContext)
		{
			_dialogService = dialogService;
			ProjectFiles = taskContext.ProjectFiles;
			VerifyProjectFiles();
			VerifyIsValid();

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
						!string.IsNullOrEmpty(a.ExternalFilePath) && File.Exists(a.ExternalFilePath));

			return count;
		}

		private void VerifyProjectFiles()
		{
			foreach (var projectFile in ProjectFiles)
			{
				if (projectFile.Action == Enumerators.Action.Import || projectFile.Action == Enumerators.Action.ImportBackTranslation)
				{
					var activityfile = projectFile.ProjectFileActivities.OrderByDescending(a => 
						a.Date).FirstOrDefault(a => a.Action == Enumerators.Action.Import ||
						                            a.Action == Enumerators.Action.ImportBackTranslation);

					projectFile.Status = Enumerators.Status.Warning;
					projectFile.ShortMessage = string.Format(PluginResources.Message_Imported_on_0, activityfile?.DateToString);
				}
				else
				{
					projectFile.Status = Enumerators.Status.Ready;
					projectFile.ShortMessage = string.Empty;
					projectFile.Report = string.Empty;
				}

				projectFile.ExternalFilePath = string.Empty;
			}
		}

		private void AddFiles()
		{
			Dispatcher.CurrentDispatcher.Invoke(new Action(delegate
			{
				var selectedFiles = _dialogService.ShowFileDialog(
					"Word Documents (*.docx)|*.docx",
					PluginResources.FilesDialog_Title,
					GetDefaultPath());

				AddFilesToGrid(selectedFiles);
			}));
		}

		private string GetDefaultPath()
		{
			var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

			var productName = GetProductName();
			var studioFolder = Path.Combine(path, productName);
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

		private string GetProductName()
		{
			if (!string.IsNullOrEmpty(_productName))
			{
				return _productName;
			}

			var studioVersionService = new StudioVersionService();
			var studioVersion = studioVersionService.GetStudioVersion();
			if (studioVersion != null)
			{
				_productName = studioVersion.StudioDocumentsFolderName;
			}

			return _productName;
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
				selectedFile.ExternalFilePath = string.Empty;
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

			var isChecked = System.Convert.ToBoolean(parameter);
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

				if (string.IsNullOrEmpty(projectFile.ExternalFilePath) || !File.Exists(projectFile.ExternalFilePath))
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
						var files = GetAllFilesFromDirectory(fullPath);
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
			foreach (var filePath in filesPath)
			{
				if (!string.IsNullOrEmpty(filePath) && !File.Exists(filePath))
				{
					continue;
				}

				string documentLanguage;
				var projectId = string.Empty;
				var documentId = string.Empty;
				var documentFullPath = string.Empty;
				var sourceLanguage = string.Empty;
				var targetLanguage = string.Empty;

				using (var wordDoc = WordprocessingDocument.Open(filePath, false))
				{
					documentLanguage = wordDoc.PackageProperties.Language;
					if (!wordDoc.PackageProperties.Keywords.Contains(";")) continue;
					var keywords = wordDoc.PackageProperties.Keywords.Split(';');
					foreach (var keyword in keywords)
					{
						if (!wordDoc.PackageProperties.Keywords.Contains(":"))
						{
							continue;
						}

						var key = keyword.Substring(0, keyword.IndexOf(":", StringComparison.Ordinal));
						var value = keyword.Substring(keyword.IndexOf(":", StringComparison.Ordinal) + 1);

						switch (key)
						{
							case "ProjectId":
								projectId = value;
								break;
							case "DocumentId":
								documentId = value;
								break;
							case "SourceLanguage":
								sourceLanguage = value;
								break;
							case "TargetLanguage":
								targetLanguage = value;
								break;
							case "DocumentFullPath":
								documentFullPath = value;
								break;
						}
					}
				}

				if (string.IsNullOrEmpty(projectId))
				{
					MessageBox.Show("The project id parameter cannot be null!", PluginResources.TranscreateManager_Name, MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				if (string.IsNullOrEmpty(sourceLanguage))
				{
					sourceLanguage = TaskContext.Project.SourceLanguage.CultureInfo.Name; ;
				}

				if (string.IsNullOrEmpty(targetLanguage))
				{
					if (string.IsNullOrEmpty(documentLanguage))
					{

						MessageBox.Show("The document language parameter cannot be null!", PluginResources.TranscreateManager_Name, MessageBoxButton.OK, MessageBoxImage.Warning);
						return;
					}
					targetLanguage = documentLanguage;
				}

				var projectLanguages = GetProjectLanguages(targetLanguage);
				var documentTargetPath = GetPathLocation(documentFullPath, projectLanguages, out var foundTargetLanguage);
				if (!foundTargetLanguage)
				{
					continue;
				}

				foreach (var projectFile in ProjectFiles)
				{
					if (string.Compare(projectFile.Project.Id, projectId, StringComparison.CurrentCultureIgnoreCase) != 0)
					{
						MessageBox.Show(PluginResources.WizardMessage_ProjectIdMissmatch, PluginResources.TranscreateManager_Name, MessageBoxButton.OK, MessageBoxImage.Warning);
						return;
					}

					var projectFileTargetLanguage = projectFile.TargetLanguage;
					if (string.Compare(projectFileTargetLanguage, targetLanguage, StringComparison.CurrentCultureIgnoreCase) != 0)
					{
						continue;
					}

					var projectFileTargetPath = GetPathLocation(projectFile.Location, targetLanguage, out var foundXliffTargetLanguage);
					if (!foundXliffTargetLanguage)
					{
						continue;
					}

					if (string.Compare(projectFileTargetPath, documentTargetPath, StringComparison.CurrentCultureIgnoreCase) == 0)
					{
						projectFile.ExternalFilePath = filePath;
						projectFile.Selected = true;
					}
				}
			}

			VerifyIsValid();
		}

		private List<string> GetProjectLanguages(string targetLanguage)
		{
			var projectLanguages = TaskContext.Project.TargetLanguages.Select(a => a.CultureInfo.Name).ToList();
			if (!string.IsNullOrEmpty(targetLanguage) && !projectLanguages.Contains(targetLanguage))
			{
				projectLanguages.Insert(0, targetLanguage);
			}

			if (!projectLanguages.Contains(TaskContext.Project.SourceLanguage.CultureInfo.Name))
			{
				projectLanguages.Add(TaskContext.Project.SourceLanguage.CultureInfo.Name);
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

		private IEnumerable<string> GetAllFilesFromDirectory(string directoryPath)
		{
			var files = new List<string>();
			files.AddRange(Directory.GetFiles(directoryPath, "*.docx", SearchOption.AllDirectories).ToList());

			return files;
		}

		private void SelectFolder()
		{
			Dispatcher.CurrentDispatcher.Invoke(new Action(delegate
			{
				var folderPath = _dialogService.ShowFolderDialog(PluginResources.FolderDialog_Title, GetDefaultPath());
				if (!string.IsNullOrEmpty(folderPath) && Directory.Exists(folderPath))
				{
					var files = GetAllFilesFromDirectory(folderPath);
					AddFilesToGrid(files);
				}
			}));
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
