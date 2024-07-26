using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.StudioViews.Commands;
using Sdl.Community.StudioViews.Model;
using Sdl.Community.StudioViews.Services;
using Sdl.Core.Globalization;
using Rws.MultiSelectComboBox.EventArgs;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;
using MessageBox = System.Windows.MessageBox;
using Task = System.Threading.Tasks.Task;
using System.Windows.Threading;

namespace Sdl.Community.StudioViews.ViewModel
{
	public class StudioViewsFilesImportViewModel : BaseModel
	{
		private readonly Window _owner;
		private readonly SdlxliffImporter _sdlxliffImporter;
		private readonly SdlxliffReader _sdlxliffReader;
		private readonly FilterItemService _filterItemService;
		private readonly ProjectFileService _projectFileService;

		private FilesController _filesController;
		private Language _language;
		private ObservableCollection<SystemFileInfo> _files;
		private IList _selectedFiles;
		private SystemFileInfo _selectedFile;

		private List<FilterItem> _filterItems;
		private ObservableCollection<FilterItem> _selectedExcludeFilterItems;


		private ICommand _clearFiltersCommand;
		private ICommand _selectedItemsChangedCommand;

		private ICommand _okCommand;
		private ICommand _resetCommand;
		private ICommand _addTemplateCommand;
		private ICommand _removeTemplateCommand;
		private ICommand _dragDropCommand;

		private string _processingMessage;
		private string _processingProgressMessage;
		private string _processingFile;
		private double _processingCurrentProgress;
		private bool _processingIsIndeterminate;
		private bool _processingShowPercentage;
		private bool _progressIsVisible;

		public StudioViewsFilesImportViewModel(Window owner, FilesController filesController, Language language, ProjectFileService projectFileService,
			FilterItemService filterItemService, SdlxliffImporter sdlxliffImporter, SdlxliffReader sdlxliffReader)
		{
			_owner = owner;
			_filterItemService = filterItemService;
			_filesController = filesController;
			_language = language;
			_sdlxliffImporter = sdlxliffImporter;
			_sdlxliffReader = sdlxliffReader;
			_projectFileService = projectFileService;

			WindowTitle = PluginResources.WindowTitle_Import;
			DialogResult = DialogResult.None;

			Reset(null);
		}

		public ICommand ClearFiltersCommand => _clearFiltersCommand ?? (_clearFiltersCommand = new CommandHandler(ClearFilters));

		public ICommand SelectedItemsChangedCommand => _selectedItemsChangedCommand ?? (_selectedItemsChangedCommand = new CommandHandler(SelectedItemsChanged));

		public ICommand AddFileCommand => _addTemplateCommand ?? (_addTemplateCommand = new CommandHandler(AddFile));

		public ICommand RemoveFileCommand => _removeTemplateCommand ?? (_removeTemplateCommand = new CommandHandler(RemoveFile));

		public ICommand DragDropCommand => _dragDropCommand ?? (_dragDropCommand = new CommandHandler(DragAndDrop));

		public ICommand OkCommand => _okCommand ?? (_okCommand = new CommandHandler(Ok));

		public ICommand ResetCommand => _resetCommand ?? (_resetCommand = new CommandHandler(Reset));

		public string WindowTitle { get; set; }

		public string FilesStatusLabel
		{
			get
			{
				var message = string.Format(PluginResources.Status_Label_Files_Selected,
					Files?.Count ?? 0,
					SelectedFiles?.Count ?? 0);
				return message;
			}
		}

		public ObservableCollection<SystemFileInfo> Files
		{
			get => _files;
			set
			{
				_files = value;
				OnPropertyChanged(nameof(Files));
				OnPropertyChanged(nameof(IsValid));
				OnPropertyChanged(nameof(FilesStatusLabel));
			}
		}

		public SystemFileInfo SelectedFile
		{
			get => _selectedFile;
			set
			{
				_selectedFile = value;
				OnPropertyChanged(nameof(SelectedFile));
				OnPropertyChanged(nameof(IsFileSelected));
				OnPropertyChanged(nameof(IsFilesSelected));
				OnPropertyChanged(nameof(FilesStatusLabel));
			}
		}

		public IList SelectedFiles
		{
			get => _selectedFiles;
			set
			{
				_selectedFiles = value;
				OnPropertyChanged(nameof(SelectedFiles));

				_selectedFile = _selectedFiles?.Cast<SystemFileInfo>().ToList().FirstOrDefault();
				OnPropertyChanged(nameof(IsFileSelected));
				OnPropertyChanged(nameof(IsFilesSelected));
				OnPropertyChanged(nameof(FilesStatusLabel));
			}
		}

		public List<FilterItem> FilterItems
		{
			get => _filterItems;
			set
			{
				if (_filterItems == value)
				{
					return;
				}

				_filterItems = value;
				OnPropertyChanged(nameof(FilterItems));
			}
		}

		public ObservableCollection<FilterItem> SelectedExcludeFilterItems
		{
			get => _selectedExcludeFilterItems ?? (_selectedExcludeFilterItems = new ObservableCollection<FilterItem>());
			set
			{
				if (_selectedExcludeFilterItems == value)
				{
					return;
				}

				_selectedExcludeFilterItems = value;
				OnPropertyChanged(nameof(SelectedExcludeFilterItems));
			}
		}

		public string ProcessingFile
		{
			get => _processingFile;
			set
			{
				if (Equals(value, _processingFile))
				{
					return;
				}

				_processingFile = value;
				OnPropertyChanged(nameof(ProcessingFile));
			}
		}

		public string ProcessingMessage
		{
			get => _processingMessage;
			set
			{
				if (Equals(value, _processingMessage))
				{
					return;
				}

				_processingMessage = value;
				OnPropertyChanged(nameof(ProcessingMessage));
			}
		}

		public string ProcessingProgressMessage
		{
			get => _processingProgressMessage;
			set
			{
				if (Equals(value, _processingProgressMessage))
				{
					return;
				}

				_processingProgressMessage = value;
				OnPropertyChanged(nameof(ProcessingProgressMessage));
			}
		}

		public double ProcessingCurrentProgress
		{
			get { return _processingCurrentProgress; }
			set
			{
				if (Equals(value, _processingCurrentProgress))
				{
					return;
				}

				_processingCurrentProgress = value;
				OnPropertyChanged("ProcessingCurrentProgress");
			}
		}

		public bool ProcessingIsIndeterminate
		{
			get => _processingIsIndeterminate;
			set
			{
				if (_processingIsIndeterminate == value)
				{
					return;
				}

				_processingIsIndeterminate = value;
				OnPropertyChanged(nameof(ProcessingIsIndeterminate));
				ProcessingShowPercentage = !_processingIsIndeterminate;
			}
		}

		public bool ProcessingShowPercentage
		{
			get => _processingShowPercentage;
			set
			{
				if (_processingShowPercentage == value)
				{
					return;
				}

				_processingShowPercentage = value;
				OnPropertyChanged(nameof(ProcessingShowPercentage));
			}
		}

		public bool ProgressIsVisible
		{
			get => _progressIsVisible;
			set
			{
				if (_progressIsVisible == value)
				{
					return;
				}

				_progressIsVisible = value;
				OnPropertyChanged(nameof(ProgressIsVisible));
				OnPropertyChanged(nameof(IsEnabled));
			}
		}

		public bool IsEnabled
		{
			get
			{
				return !ProgressIsVisible;
			}
		}

		public bool IsFilesSelected => SelectedFiles?.Cast<SystemFileInfo>().ToList().Count > 0;

		public bool IsFileSelected => SelectedFiles?.Cast<SystemFileInfo>().ToList().Count == 1;

		public bool IsValid => Files?.Count > 0;

		public string Message { get; private set; }

		public bool Success { get; private set; }

		public string LogFilePath { get; private set; }

		public string ExportPath { get; private set; }

		public DateTime ProcessingDateTime { get; private set; }

		public DialogResult DialogResult { get; set; }

		public void AddFiles(IReadOnlyCollection<SystemFileInfo> files)
		{
			foreach (var systemFile in files)
			{
				var fileInfo = _files.FirstOrDefault(a => a.Name == systemFile.Name && a.Path == systemFile.Path);
				if (fileInfo == null)
				{
					_files.Add(systemFile);
				}
			}

			OnPropertyChanged(nameof(Files));
			OnPropertyChanged(nameof(IsValid));
			OnPropertyChanged(nameof(FilesStatusLabel));
		}

		public void RemoveFiles(IReadOnlyCollection<SystemFileInfo> files)
		{
			if (files == null || files.Count == 0)
			{
				return;
			}

			foreach (var fileInfo in files)
			{
				var template = _files.FirstOrDefault(a => a.Name == fileInfo.Name &&
														  a.Path == fileInfo.Path);
				if (template != null)
				{
					_files.Remove(template);
				}
			}

			OnPropertyChanged(nameof(Files));
			OnPropertyChanged(nameof(IsValid));
			OnPropertyChanged(nameof(FilesStatusLabel));
		}

		private void Ok(object parameter)
		{
			try
			{
				_owner.Dispatcher.Invoke(DispatcherPriority.ContextIdle,
					new Action(delegate
					{
						ProgressIsVisible = true;
					}));

				ProcessingDateTime = DateTime.Now;
				var task = Task.Run(ImportFiles);
				task.ContinueWith(t =>
				{
					ExportPath = Path.GetDirectoryName(task.Result.FirstOrDefault()?.FilePath);
					
					var logFileName = "StudioViews_" + "Import" + "_" + _projectFileService.GetDateTimeToFilePartString(ProcessingDateTime) + ".log";
					LogFilePath = Path.Combine(ExportPath, logFileName);

					var failCount = task.Result.Count(a => !a.Success);

					WriteLogFile(t.Result, Files.ToList(), LogFilePath);

					if (failCount == 0)
					{
						Success = true;

						var importedCount = 0;
						var excludedCount = 0;
						var updatedFileCount = 0;

						foreach (var result in task.Result)
						{
							updatedFileCount += result.UpdatedSegments > 0 ? 1 : 0;
							importedCount += result.UpdatedSegments;
							excludedCount += result.ExcludedSegments;

							if (updatedFileCount > 0)
							{
								File.Copy(result.FilePath, result.BackupFilePath, true);
								File.Delete(result.FilePath);

								File.Copy(result.UpdatedFilePath, result.FilePath, true);
								File.Delete(result.UpdatedFilePath);
							}
						}

						Message = PluginResources.Message_Successfully_Completed_Import_Operation;
						Message += Environment.NewLine + Environment.NewLine;
						Message += string.Format(PluginResources.Message_Updated_Of_The_Selected_Files, updatedFileCount);
						Message += Environment.NewLine + Environment.NewLine;
						Message += PluginResources.Message_Segments;
						Message += Environment.NewLine;
						Message += string.Format(PluginResources.Message_Tab_Updated, importedCount);
						Message += Environment.NewLine;
						Message += string.Format(PluginResources.Message_Tab_Excluded, excludedCount);
					}
					else
					{
						Success = false;
						Message = PluginResources.Error_Message_The_Import_Operation_Failed;
					}

					DialogResult = DialogResult.OK;
					AttemptToCloseWindow();
				});
			}
			catch (Exception e)
			{
				DialogResult = DialogResult.Abort;

				_owner.Dispatcher.Invoke(DispatcherPriority.ContextIdle,
					new Action(delegate
					{
						ProgressIsVisible = false;
					}));

				MessageBox.Show(e.Message, PluginResources.Plugin_Name, MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void WriteLogFile(IReadOnlyCollection<ImportResult> importResults, List<SystemFileInfo> files, string logFilePath)
		{
			_owner.Dispatcher.Invoke(
				delegate
				{
					using (var sr = new StreamWriter(logFilePath, false, Encoding.UTF8))
					{
						sr.WriteLine(PluginResources.Plugin_Name);
						sr.WriteLine(PluginResources.LogFile_Title_Task_Import_Files);
						sr.WriteLine(PluginResources.LogFile_Label_Start_Processing, _projectFileService.GetDateTimeToString(ProcessingDateTime));

						if (SelectedExcludeFilterItems.Count > 0)
						{
							sr.WriteLine(string.Empty);
							var filterItems = _filterItemService.GetFilterItemsText(SelectedExcludeFilterItems.ToList());
							sr.WriteLine(PluginResources.LogFile_Label_Exclude_Filters + filterItems);
						}

						sr.WriteLine(string.Empty);
						sr.WriteLine(PluginResources.LogFile_Label_Import_Files, files.Count);
						var fileIndex = 0;
						foreach (var fileInfo in files)
						{
							fileIndex++;
							sr.WriteLine(PluginResources.LogFile_Tab_Label_File_Number_Path, fileIndex, fileInfo.FullPath);
						}

						sr.WriteLine(string.Empty);
						sr.WriteLine(PluginResources.LogFile_Label_Updated_Files, importResults.Count(a => a.UpdatedSegments > 0));
						sr.WriteLine(PluginResources.LogFile_Label_Updated_Segments, importResults.Sum(importResult => importResult.UpdatedSegments));
						sr.WriteLine(string.Empty);

						sr.WriteLine(PluginResources.LogFile_Label_Selected_Files, importResults.Count);
						fileIndex = 0;
						foreach (var result in importResults)
						{
							fileIndex++;
							sr.WriteLine(PluginResources.LogFile_Label_File_Number_Path, fileIndex, result.FilePath);
							sr.WriteLine(PluginResources.LogFile_Label_Tab_Success + result.Success);
							sr.WriteLine(PluginResources.LogFile_Label_Tab_Updated + (result.UpdatedSegments > 0
								? PluginResources.Label_True : PluginResources.Label_False));
							if (result.UpdatedSegments > 0)
							{
								sr.WriteLine(PluginResources.LogFile_Label_Tab_Backup + result.BackupFilePath);
							}
							sr.WriteLine(PluginResources.Message_Tab_Segments);
							sr.WriteLine(PluginResources.Message_Tab_Tab_Updated, result.UpdatedSegments);
							sr.WriteLine(PluginResources.Message_Tab_Tab_Excluded, result.ExcludedSegments);

							sr.WriteLine(string.Empty);
						}

						sr.WriteLine(string.Empty);
						sr.WriteLine(PluginResources.LogFile_Label_End_Processing, _projectFileService.GetDateTimeToString(DateTime.Now));

						sr.Flush();
						sr.Close();
					}
				});
		}

		private void Reset(object paramter)
		{
			ProgressIsVisible = false;

			Files = new ObservableCollection<SystemFileInfo>();

			FilterItems = new List<FilterItem>(_filterItemService.GetFilterItems());
			SelectedExcludeFilterItems = new ObservableCollection<FilterItem>(
				_filterItemService.GetFilterItems(FilterItems, new List<string> { "Locked" }));

			OnPropertyChanged(nameof(IsFilesSelected));
			OnPropertyChanged(nameof(Files));
			OnPropertyChanged(nameof(IsValid));
			OnPropertyChanged(nameof(FilesStatusLabel));
		}

		private void ClearFilters(object parameter)
		{
			SelectedExcludeFilterItems.Clear();
			OnPropertyChanged(nameof(SelectedExcludeFilterItems));
		}

		private void SelectedItemsChanged(object parameter)
		{
			if (parameter is SelectedItemsChangedEventArgs)
			{
				OnPropertyChanged(nameof(SelectedExcludeFilterItems));
			}
		}

		private void AddFile(object paramter)
		{
			var importFiles = GetImportFiles(Path.GetDirectoryName(Files.FirstOrDefault()?.FullPath));
			if (importFiles == null)
			{
				return;
			}

			foreach (var fileName in importFiles)
			{
				AddNewFile(new SystemFileInfo(fileName));
			}

			OnPropertyChanged(nameof(Files));
			OnPropertyChanged(nameof(IsValid));
			OnPropertyChanged(nameof(FilesStatusLabel));
		}

		private IEnumerable<string> GetImportFiles(string directory)
		{
			try
			{
				var openFileDialog = new Microsoft.Win32.OpenFileDialog
				{
					Title = PluginResources.OpenFileDialog_Select_Import_File
				};

				var myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

				var initialDirectory = !string.IsNullOrEmpty(directory) ? Path.GetDirectoryName(directory) : string.Empty;

				openFileDialog.InitialDirectory = !string.IsNullOrEmpty(initialDirectory) && Directory.Exists(initialDirectory)
					? initialDirectory
					: myDocuments;
				openFileDialog.Filter = "SDLXLIFF files|*.sdlxliff";
				openFileDialog.FilterIndex = 0;
				openFileDialog.RestoreDirectory = true;
				openFileDialog.Multiselect = true;

				if (openFileDialog.ShowDialog() == true)
				{
					return openFileDialog.FileNames.ToList();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, PluginResources.Plugin_Name, MessageBoxButton.OK, MessageBoxImage.Warning);
			}

			return null;
		}

		private bool AddNewFile(SystemFileInfo systemFile)
		{
			var fileInfo = Files.FirstOrDefault(a => a.Name == systemFile.Name && a.Path == systemFile.Path);
			if (fileInfo == null)
			{
				Files.Add(systemFile);
				return true;
			}

			return false;
		}

		private void RemoveFile(object paramter)
		{
			var fileInfos = _selectedFiles.Cast<SystemFileInfo>().ToList();
			foreach (var fileInfo in fileInfos)
			{
				var template = _files.FirstOrDefault(a => a.Name == fileInfo.Name &&
																	a.Path == fileInfo.Path);
				if (template != null)
				{
					_files.Remove(template);
				}
			}

			OnPropertyChanged(nameof(Files));
			OnPropertyChanged(nameof(IsValid));
			OnPropertyChanged(nameof(FilesStatusLabel));
		}

		private void DragAndDrop(object parameter)
		{
			if (!(parameter is DragEventArgs eventArgs))
			{
				return;
			}

			var fileDrop = eventArgs.Data.GetData(DataFormats.FileDrop, false);
			if (fileDrop is string[] files)
			{
				foreach (var fullPath in files)
				{
					var fileAttributes = File.GetAttributes(fullPath);
					if (!fileAttributes.HasFlag(FileAttributes.Directory) && fullPath.ToLower().EndsWith(".sdlxliff"))
					{
						AddNewFile(new SystemFileInfo(fullPath));
					}
				}

				OnPropertyChanged(nameof(Files));
				OnPropertyChanged(nameof(IsValid));
				OnPropertyChanged(nameof(FilesStatusLabel));
			}
		}

		private async Task<List<ImportResult>> ImportFiles()
		{
			var importResults = new List<ImportResult>();
			var importFiles = Files.Select(a => a.FullPath);
			var updatedSegmentPairs = GetUpdatedSegmentPairs(importFiles, _sdlxliffReader);
			var excludeFilterIds = SelectedExcludeFilterItems.Select(a => a.Id).ToList();

			var projectFiles = GetProjectFiles(updatedSegmentPairs, _sdlxliffReader);

			var missingFiles = projectFiles.Any(file => file.LocalFileState == LocalFileState.Missing);
			if (missingFiles)
			{
				throw new Exception(PluginResources.Message_Missing_Project_Files_Download_From_Server);
			}

			var fileIndex = 0;
			foreach (var selectedFile in projectFiles)
			{
				fileIndex++;

				_owner.Dispatcher.Invoke(DispatcherPriority.ContextIdle,
					new Action(delegate
					{
						ProcessingMessage = string.Format(PluginResources.Progress_Processing_0_of_1_files, fileIndex, projectFiles.Count());
						ProcessingFile = Path.GetFileName(selectedFile.LocalFilePath);
						ProcessingProgressMessage = "Updating segments...";
						ProcessingCurrentProgress = 0;
						ProcessingIsIndeterminate = true;
					}));

				var updatedFilePath = Path.GetTempFileName();
				var importResult = _sdlxliffImporter.UpdateFile(updatedSegmentPairs, excludeFilterIds,
					selectedFile.LocalFilePath, updatedFilePath);
				importResults.Add(importResult);
			}

			_owner.Dispatcher.Invoke(DispatcherPriority.ContextIdle,
				new Action(delegate
				{
					ProgressIsVisible = false;
				}));

			return await Task.FromResult(new List<ImportResult>(importResults));
		}

		private IEnumerable<ProjectFile> GetProjectFiles(IReadOnlyCollection<SegmentPairInfo> updatedSegmentPairs, 
			SdlxliffReader sdlXliffReader)
		{
			var projectFiles = new List<ProjectFile>();
			var project = _filesController.CurrentProject;

			var targetLanguageFiles = project.GetTargetLanguageFiles(_language);
			var fileIndex = 0;
			foreach (var targetLanguageFile in targetLanguageFiles)
			{
				fileIndex++;

				_owner.Dispatcher.Invoke(DispatcherPriority.ContextIdle,
					new Action(delegate
					{
						ProcessingMessage = string.Format(PluginResources.Progress_Processing_0_of_1_files, fileIndex, targetLanguageFiles.Length);
						ProcessingFile = Path.GetFileName(targetLanguageFile.LocalFilePath);
						ProcessingProgressMessage = "Loading project files...";
						ProcessingCurrentProgress = 0;
						ProcessingIsIndeterminate = true;
					}));

				if (targetLanguageFile.Role != FileRole.Translatable)
				{
					continue;
				}

				var segmentPairs = sdlXliffReader.GetSegmentPairs(targetLanguageFile.LocalFilePath);
				if (updatedSegmentPairs.Any(updatedSegmentPair => segmentPairs.Exists(a => a.ParagraphUnitId == updatedSegmentPair.ParagraphUnitId)))
				{
					projectFiles.Add(targetLanguageFile);
				}
			}
			
			return projectFiles;
		}

		private List<SegmentPairInfo> GetUpdatedSegmentPairs(IEnumerable<string> importFiles, SdlxliffReader sdlXliffReader)
		{

			var updatedSegmentPairs = new List<SegmentPairInfo>();
			var fileIndex = 0;
			foreach (var importFile in importFiles)
			{
				fileIndex++;
				_owner.Dispatcher.Invoke(DispatcherPriority.ContextIdle,
					new Action(delegate
					{
						ProcessingMessage = string.Format(PluginResources.Progress_Processing_0_of_1_files, fileIndex, importFiles.Count());
						ProcessingFile = Path.GetFileName(importFile);
						ProcessingProgressMessage = "Reading segments...";
						ProcessingCurrentProgress = 0;
						ProcessingIsIndeterminate = true;
					}));

				var segmentPairs = sdlXliffReader.GetSegmentPairs(importFile);
				foreach (var segmentPair in segmentPairs)
				{
					if (!updatedSegmentPairs.Exists(a => a.ParagraphUnitId == segmentPair.ParagraphUnitId &&
														 a.SegmentId == segmentPair.SegmentId))
					{
						updatedSegmentPairs.Add(segmentPair);
					}
				}
			}

			return updatedSegmentPairs;
		}

		private void AttemptToCloseWindow()
		{
			var task = Task.Run(
				delegate
				{
					System.Threading.Thread.Sleep(500);
				});

			task.ContinueWith(
				delegate
				{
					ProgressIsVisible = false;
					_owner.Dispatcher.BeginInvoke(
						new Action(delegate
						{
							_owner?.Close();
						}));

				}
			);
		}
	}
}
