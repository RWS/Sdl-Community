using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using Rws.MultiSelectComboBox.EventArgs;
using Sdl.Community.StudioViews.Commands;
using Sdl.Community.StudioViews.Controls.Folder;
using Sdl.Community.StudioViews.Model;
using Sdl.Community.StudioViews.Services;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.FileBased.Reports.Operations;
using MessageBox = System.Windows.MessageBox;
using Task = System.Threading.Tasks.Task;

namespace Sdl.Community.StudioViews.ViewModel
{
	public class StudioViewsFilesSplitViewModel : BaseModel
	{
		private readonly Window _owner;
		private readonly SdlxliffMerger _sdlxliffMerger;
		private readonly SdlxliffExporter _sdlxliffExporter;
		private readonly SdlxliffReader _sdlxliffReader;
		private readonly List<ProjectFile> _selectedFiles;
		private readonly FilterItemService _filterItemService;
		private readonly ProjectFileService _projectFileService;
		private readonly FileBasedProject _project;

		private int _maxNumberOfWords;
		private int _numberOfEqualParts;
		private string _segmentIdsString;
		private bool _splitByWordCount;
		private bool _splitByEqualParts;
		private bool _splitBySegmentIds;
		private string _exportPath;
		private string _fileName;
		private bool _exportPathIsValid;
		private bool _fileNameIsValid;

		private List<FilterItem> _filterItems;
		private ObservableCollection<FilterItem> _selectedExcludeFilterItems;

		private ICommand _clearFiltersCommand;
		private ICommand _selectedItemsChangedCommand;

		private ICommand _okCommand;
		private ICommand _resetCommand;
		private ICommand _exportPathBrowseCommand;
		private ICommand _openFolderInExplorerCommand;

		private string _processingMessage;
		private string _processingProgressMessage;
		private string _processingFile;
		private double _processingCurrentProgress;
		private bool _processingIsIndeterminate;
		private bool _processingShowPercentage;
		private bool _progressIsVisible;
		private bool _cancelIsVisible;

		public StudioViewsFilesSplitViewModel(Window owner, FileBasedProject project, List<ProjectFile> selectedFiles, ProjectFileService projectFileService,
			FilterItemService filterItemService, SdlxliffMerger sdlxliffMerger, SdlxliffExporter sdlxliffExporter, SdlxliffReader sdlxliffReader)
		{
			_owner = owner;
			_project = project;
			_selectedFiles = selectedFiles;
			_projectFileService = projectFileService;
			_sdlxliffMerger = sdlxliffMerger;
			_sdlxliffExporter = sdlxliffExporter;
			_sdlxliffReader = sdlxliffReader;
			_filterItemService = filterItemService;

			WindowTitle = PluginResources.StudioViews_SplitSelectedFiles_Name;

			DialogResult = DialogResult.None;
			Reset(null);
		}

		public ICommand ClearFiltersCommand => _clearFiltersCommand ?? (_clearFiltersCommand = new CommandHandler(ClearFilters));

		public ICommand SelectedItemsChangedCommand => _selectedItemsChangedCommand ?? (_selectedItemsChangedCommand = new CommandHandler(SelectedItemsChanged));

		public ICommand OpenFolderInExplorerCommand => _openFolderInExplorerCommand ?? (_openFolderInExplorerCommand = new CommandHandler(OpenFolderInExplorer));

		public ICommand ExportPathBrowseCommand => _exportPathBrowseCommand ?? (_exportPathBrowseCommand = new CommandHandler(ExportPathBrowse));

		public ICommand OkCommand => _okCommand ?? (_okCommand = new CommandHandler(Ok));

		public ICommand ResetCommand => _resetCommand ?? (_resetCommand = new CommandHandler(Reset));

		public string WindowTitle { get; set; }

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

		public bool CancelIsVisible
		{
			get => _cancelIsVisible;
			set
			{
				if (_cancelIsVisible == value)
				{
					return;
				}

				_cancelIsVisible = value;
				OnPropertyChanged(nameof(CancelIsVisible));
			}
		}

		public int MaxNumberOfWords
		{
			get => _maxNumberOfWords;
			set
			{
				if (_maxNumberOfWords == value)
				{
					return;
				}

				_maxNumberOfWords = value;
				OnPropertyChanged(nameof(MaxNumberOfWords));
			}
		}

		public int NumberOfEqualParts
		{
			get => _numberOfEqualParts;
			set
			{
				if (_numberOfEqualParts == value)
				{
					return;
				}

				_numberOfEqualParts = value;
				OnPropertyChanged(nameof(NumberOfEqualParts));
			}
		}

		public string SegmentIdsString
		{
			get => _segmentIdsString;
			set
			{
				if (_segmentIdsString == value)
				{
					return;
				}

				_segmentIdsString = value;
				OnPropertyChanged(nameof(SegmentIdsString));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public bool SplitByWordCount
		{
			get => _splitByWordCount;
			set
			{
				if (_splitByWordCount == value)
				{
					return;
				}

				SplitByEqualParts = false;
				SplitBySegmentIds = false;

				_splitByWordCount = value;
				OnPropertyChanged(nameof(SplitByWordCount));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public bool SplitByEqualParts
		{
			get => _splitByEqualParts;
			set
			{
				if (_splitByEqualParts == value)
				{
					return;
				}

				SplitByWordCount = false;
				SplitBySegmentIds = false;

				_splitByEqualParts = value;
				OnPropertyChanged(nameof(SplitByEqualParts));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public bool SplitBySegmentIds
		{
			get => _splitBySegmentIds;
			set
			{
				if (_splitBySegmentIds == value)
				{
					return;
				}

				SplitByWordCount = false;
				SplitByEqualParts = false;

				_splitBySegmentIds = value;
				OnPropertyChanged(nameof(SplitBySegmentIds));
				OnPropertyChanged(nameof(IsValid));
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

		public bool IsEnabled
		{
			get
			{
				return !ProgressIsVisible;
			}
		}

		public string FileName
		{
			get => _fileName;
			set
			{
				if (_fileName == value)
				{
					return;
				}

				_fileName = value;
				OnPropertyChanged(nameof(FileName));

				var regexNum = new Regex(@"(?<digits>\[[#]+\])", RegexOptions.None);
				FileNameIsValid = !string.IsNullOrEmpty(FileName.Trim()) &&
								  FileName.TrimEnd().EndsWith(".sdlxliff", StringComparison.InvariantCultureIgnoreCase) &&
								  regexNum.Match(FileName).Success;
			}
		}

		public string ExportPath
		{
			get => _exportPath;
			set
			{
				if (_exportPath == value)
				{
					return;
				}

				_exportPath = value;
				OnPropertyChanged(nameof(ExportPath));

				ExportPathIsValid = Directory.Exists(ExportPath);
			}
		}

		public bool FileNameIsValid
		{
			get => _fileNameIsValid;
			set
			{
				if (_fileNameIsValid == value)
				{
					return;
				}

				_fileNameIsValid = value;
				OnPropertyChanged(nameof(FileNameIsValid));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public bool ExportPathIsValid
		{
			get => _exportPathIsValid;
			set
			{
				if (_exportPathIsValid == value)
				{
					return;
				}

				_exportPathIsValid = value;
				OnPropertyChanged(nameof(ExportPathIsValid));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public bool IsValid
		{
			get
			{
				var segmentIdsIsValid = true;
				if (SplitBySegmentIds)
				{
					segmentIdsIsValid = !string.IsNullOrEmpty(SegmentIdsString?.Trim());
				}

				return ExportPathIsValid && FileNameIsValid && segmentIdsIsValid;
			}
		}

		public DialogResult DialogResult { get; set; }

		public string Message { get; private set; }

		public bool Success { get; private set; }

		public string LogFilePath { get; private set; }

		public DateTime ProcessingDateTime { get; private set; }

		private void ExportPathBrowse(object parameter)
		{
			try
			{
				var initialDirectory = GetValidFolderPath(ExportPath);
				if (string.IsNullOrEmpty(initialDirectory) || !Directory.Exists(initialDirectory))
				{
					initialDirectory = null;
				}

				var folderDialog = new FolderSelectDialog
				{
					Title = PluginResources.FolderSelectDialog_Select_Export_Folder,
					InitialDirectory = initialDirectory,
				};

				var result = folderDialog.ShowDialog();
				if (result)
				{
					ExportPath = folderDialog.FileName;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, PluginResources.Plugin_Name, MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void OpenFolderInExplorer(object parameter)
		{
			if (Directory.Exists(ExportPath))
			{
				Process.Start(ExportPath);
			}
		}

		private void Ok(object parameter)
		{
			if (_selectedFiles == null || _selectedFiles.Count <= 0)
			{
				MessageBox.Show(PluginResources.Message_No_files_selected, PluginResources.Plugin_Name, MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			try
			{
				_owner.Dispatcher.Invoke(DispatcherPriority.ContextIdle,
					new Action(delegate
					{
						ProgressIsVisible = true;
					}));


				ProcessingDateTime = DateTime.Now;
				var logFileName = "StudioViews_" + "Split" + "_" + _projectFileService.GetDateTimeToFilePartString(ProcessingDateTime) + ".log";
				LogFilePath = Path.Combine(ExportPath, logFileName);

				_owner.Dispatcher.Invoke(DispatcherPriority.ContextIdle,
					new Action(delegate
					{
						ProcessingMessage = "Identify segmentation markers...";
						ProcessingFile = "...";
						ProcessingProgressMessage = "...";
						ProcessingCurrentProgress = 0;
						ProcessingIsIndeterminate = true;
					}));

				if (!HasSegmentationMarkers())
				{
					_owner.Dispatcher.Invoke(DispatcherPriority.ContextIdle,
						new Action(delegate
						{
							ProcessingMessage = "Applying segmentation markers...";
							ProcessingFile = "...";
							ProcessingProgressMessage = "...";
							ProcessingCurrentProgress = 0;
							ProcessingIsIndeterminate = true;
						}));

					// needed to add segmentation markers to the files
					RunPretranslateWithEmptyTm(_project, _selectedFiles.FirstOrDefault()?.Language);
				}

				var task = Task.Run(ExportFiles);
				task.ContinueWith(t =>
				{
					Success = t.Result.Success;
					Message = t.Result.Message;

					WriteLogFile(t.Result, LogFilePath);

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

		private void RunPretranslateWithEmptyTm(FileBasedProject project, Language langauge)
		{
			var operations = new ProjectReportsOperations(project);

			var tmProviderConfigOrig = project.GetTranslationProviderConfiguration();
			var tmProviderConfig = project.GetTranslationProviderConfiguration();

			foreach (var cascadeEntry in tmProviderConfig.Entries)
			{
				cascadeEntry.MainTranslationProvider.Enabled = false;
				foreach (var providerReference in cascadeEntry.ProjectTranslationMemories)
				{
					providerReference.Enabled = false;
				}
			}

			project.UpdateTranslationProviderConfiguration(tmProviderConfig);
			project.Save();

			try
			{
				var targetLanguageFileIds = project.GetTargetLanguageFiles(langauge)?.GetIds();
				var task1 = project.RunAutomaticTask(targetLanguageFileIds,
					AutomaticTaskTemplateIds.PreTranslateFiles);

				operations.RemoveReports(task1.Reports.Select(a => a.Id).ToList());
			}
			catch
			{
				// ignore/ catch all
			}
			finally
			{
				project.UpdateTranslationProviderConfiguration(tmProviderConfigOrig);
				project.Save();
			}
		}

		private bool HasSegmentationMarkers()
		{
			foreach (var selectedFile in _selectedFiles)
			{
				var segmentPairs = _sdlxliffReader.GetSegmentPairs(selectedFile?.LocalFilePath);
				if (segmentPairs.Count > 0)
				{
					return true;
				}
			}

			return false;
		}

		private void WriteLogFile(ExportResult exportResult, string logFilePath)
		{
			_owner.Dispatcher.Invoke(
				delegate
				{
					using (var sr = new StreamWriter(logFilePath, false, Encoding.UTF8))
					{
						sr.WriteLine(PluginResources.Plugin_Name);
						sr.WriteLine(PluginResources.LogFile_Title_Task_Split_Files);
						sr.WriteLine(PluginResources.LogFile_Label_Start_Processing, _projectFileService.GetDateTimeToString(ProcessingDateTime));

						sr.WriteLine(string.Empty);
						sr.WriteLine(PluginResources.LogFile_Tab_Label_Input_Files_Number, exportResult.InputFiles.Count);
						var fileIndex = 0;
						foreach (var filePath in exportResult.InputFiles)
						{
							fileIndex++;
							sr.WriteLine(PluginResources.LogFile_Tab_Label_File_Number_Path, fileIndex, filePath);
						}

						sr.WriteLine(string.Empty);
						sr.WriteLine(PluginResources.LogFile_Label_Output_Files_Number, exportResult.OutputFiles.Count);
						fileIndex = 0;
						foreach (var file in exportResult.OutputFiles)
						{
							fileIndex++;
							sr.WriteLine(PluginResources.LogFile_Tab_Label_File_Number_Path, fileIndex, file.FilePath);
							sr.WriteLine(PluginResources.Message_Tab_Tab_Segments_Number, file.SegmentCount);
							sr.WriteLine(PluginResources.Message_Tab_Tab_Words_Number, file.WordCount);
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
			FilterItems = new List<FilterItem>(_filterItemService.GetFilterItems());
			SelectedExcludeFilterItems = new ObservableCollection<FilterItem>(
				_filterItemService.GetFilterItems(FilterItems, new List<string> { "Locked" }));

			ProgressIsVisible = false;
			SplitByWordCount = true;
			MaxNumberOfWords = 1000;
			NumberOfEqualParts = 2;

			SetDefaultOutputValues();
			OnPropertyChanged(nameof(IsValid));
		}

		private async Task<ExportResult> ExportFiles()
		{
			var filePathInput = _selectedFiles[0].LocalFilePath;

			var exportResult = new ExportResult
			{
				InputFiles = _selectedFiles.Select(a => a.LocalFilePath).ToList(),
				OutputFiles = new List<OutputFile>()
			};

			if (_selectedFiles.Count > 1)
			{
				var files = _selectedFiles.Select(a => a.LocalFilePath).ToList();
				var fileDirectory = Path.GetDirectoryName(files[0]);
				var filePathOutput =
					_projectFileService.GetUniqueFileName(Path.Combine(fileDirectory, "StudioViewsFile.sdlxliff"), string.Empty);

				_owner.Dispatcher.Invoke(DispatcherPriority.ContextIdle,
					new Action(delegate
					{
						ProcessingMessage = "Merging selected files...";
						ProcessingFile = Path.GetFileName(filePathOutput);
						ProcessingProgressMessage = "...";
						ProcessingCurrentProgress = 0;
						ProcessingIsIndeterminate = false;
					}));

				var mergedFile = _sdlxliffMerger.MergeFiles(files, filePathOutput, false, ProgressLogger);
				if (mergedFile)
				{
					filePathInput = filePathOutput;
				}
				else
				{
					exportResult.Success = false;
					exportResult.Message = PluginResources.Error_Message_Unexpected_Error_Merging_Files;
					return await Task.FromResult(exportResult);
				}
			}


			_owner.Dispatcher.Invoke(DispatcherPriority.ContextIdle,
				new Action(delegate
				{
					ProcessingMessage = string.Format(PluginResources.Progress_Processing_0_of_1_files, 1, 1);
					ProcessingFile = Path.GetFileName(filePathInput);
					ProcessingProgressMessage = "Reading segments";
					ProcessingCurrentProgress = 0;
					ProcessingIsIndeterminate = true;
				}));


			var segmentPairs = _sdlxliffReader.GetSegmentPairs(filePathInput);

			_owner.Dispatcher.Invoke(DispatcherPriority.ContextIdle,
				new Action(delegate
				{
					ProcessingMessage = string.Format(PluginResources.Progress_Processing_0_of_1_files, 1, 1);
					ProcessingFile = Path.GetFileName(filePathInput);
					ProcessingProgressMessage = "Generating segment word counts";
					ProcessingCurrentProgress = 0;
					ProcessingIsIndeterminate = false;
				}));


			var sourceLanguage = _selectedFiles.FirstOrDefault()?.SourceFile.Language.CultureInfo;
			var targetLanguage = _selectedFiles.FirstOrDefault()?.Language.CultureInfo;
			
			var segmentWordCountService = new SegmentWordCounts(sourceLanguage, targetLanguage);

			//var counter = 0;
			//Parallel.For(0, segmentPairs.Count, 
			//	new ParallelOptions { MaxDegreeOfParallelism = 5 }, index =>
			//{

			//	_owner.Dispatcher.Invoke(DispatcherPriority.ContextIdle,
			//		new Action(delegate
			//		{
			//			var segmentPairInfo = segmentPairs[index];
			//			segmentPairInfo.SourceWordCounts = segmentWordCountService.GetWordCounts(segmentPairInfo.SegmentPair);

			//			counter++;
			//			ProcessingProgressMessage = "Generating segment word counts";
			//			ProcessingCurrentProgress = GetPercentageValue(counter, segmentPairs.Count);
			//		}));
			//});

			for (var index = 0; index < segmentPairs.Count; index++)
			{
				var segmentPairInfo = segmentPairs[index];
				segmentPairInfo.SourceWordCounts = segmentWordCountService.GetWordCounts(segmentPairInfo.SegmentPair);
				_owner.Dispatcher.Invoke(DispatcherPriority.ContextIdle,
					new Action(delegate
					{
						ProcessingProgressMessage = "Generating segment word counts";
						ProcessingCurrentProgress = GetPercentageValue(index + 1, segmentPairs.Count);
					}));
			}

			var segmentPairSplits = GetSegmentPairSplits(segmentPairs);
			if (segmentPairSplits == null)
			{
				exportResult.Success = false;
				exportResult.Message = PluginResources.Error_Message_No_Segments_Selected;
				return await Task.FromResult(exportResult);
			}

			var fileIndex = 0;
			foreach (var segmentPairSplit in segmentPairSplits)
			{
				fileIndex++;
				var filePathOutput = GetFilePathOutput(FileName, ExportPath, fileIndex);

				_owner.Dispatcher.Invoke(DispatcherPriority.ContextIdle,
				new Action(delegate
				{
					ProcessingMessage = string.Format(PluginResources.Progress_Processing_0_of_1_files, fileIndex, segmentPairSplits.Count());
					ProcessingFile = Path.GetFileName(filePathOutput);
					ProcessingProgressMessage = "Exporting segments...";
					ProcessingCurrentProgress = 0;
					ProcessingIsIndeterminate = false;
				}));

				var outputFile = _sdlxliffExporter.ExportFile(segmentPairSplit, filePathInput, filePathOutput, 
					segmentWordCountService, ProgressLogger);

				exportResult.OutputFiles.Add(outputFile);
			}

			if (_selectedFiles.Count > 1)
			{
				if (File.Exists(filePathInput))
				{
					File.Delete(filePathInput);
				}
			}

			exportResult.Success = true;
			exportResult.Message = Message = PluginResources.Message_Successfully_Completed_Split_Operation;
			exportResult.Message += Environment.NewLine + Environment.NewLine;
			exportResult.Message += string.Format(PluginResources.Message_Exported_Segments_into_Files,
				exportResult.OutputFiles.Sum(a => a.SegmentCount), fileIndex);


			_owner.Dispatcher.Invoke(DispatcherPriority.ContextIdle,
				new Action(delegate
				{
					ProgressIsVisible = false;
				}));

			return await Task.FromResult(exportResult);
		}

		private IEnumerable<List<SegmentPairInfo>> GetSegmentPairSplits(List<SegmentPairInfo> segmentPairs)
		{
			IEnumerable<List<SegmentPairInfo>> segmentPairSplits = null;
			if (SplitByWordCount)
			{
				segmentPairSplits = GetSegmentPairsSplitByMaxWordCount(segmentPairs, MaxNumberOfWords);
			}
			else if (SplitByEqualParts)
			{
				var splitWordCount = GetTotalWordCount(segmentPairs);
				var maxNumberOfWords =
					(int)Math.Round(Convert.ToDecimal(splitWordCount) / Convert.ToDecimal(NumberOfEqualParts), 0,
						MidpointRounding.AwayFromZero);
				segmentPairSplits = GetSegmentPairsSplitByMaxWordCount(segmentPairs, maxNumberOfWords);
			}
			else if (SplitBySegmentIds)
			{
				var segmentIds = GetSegmentIds(SegmentIdsString);
				segmentPairSplits = GetSegmentPairsSplitBySegmentId(segmentPairs, segmentIds);
			}

			return segmentPairSplits;
		}

		private IEnumerable<List<SegmentPairInfo>> GetSegmentPairsSplitBySegmentId(IEnumerable<SegmentPairInfo> segmentPairs, List<string> segmentIds)
		{
			var segmentPairsSplits = new List<List<SegmentPairInfo>>();

			var segmentPairSplits = new List<SegmentPairInfo>();
			foreach (var segmentPair in segmentPairs)
			{
				segmentPairSplits.Add(segmentPair);

				if (segmentIds.Exists(a => string.Compare(a.Trim(),
					segmentPair.SegmentId, StringComparison.InvariantCultureIgnoreCase) == 0))
				{
					segmentPairsSplits.Add(segmentPairSplits);
					segmentPairSplits = new List<SegmentPairInfo>();
				}
			}

			if (segmentPairSplits.Count > 0)
			{
				segmentPairsSplits.Add(segmentPairSplits);
			}

			return segmentPairsSplits;
		}

		private IEnumerable<List<SegmentPairInfo>> GetSegmentPairsSplitByMaxWordCount(IEnumerable<SegmentPairInfo> segmentPairInfos, int maxWordCount)
		{
			var segmentPairsSplits = new List<List<SegmentPairInfo>>();
			var excludeFilterIds = SelectedExcludeFilterItems.Select(a => a.Id).ToList();
			var splitWordCount = 0;
			var segmentPairSplits = new List<SegmentPairInfo>();
			foreach (var segmentPairInfo in segmentPairInfos)
			{
				segmentPairSplits.Add(segmentPairInfo);

				// include the word count based on the segment status properties selected by the user
				var status = segmentPairInfo.SegmentPair.Properties.ConfirmationLevel.ToString();
				var match = _filterItemService.GetTranslationOriginType(segmentPairInfo.SegmentPair.Target.Properties.TranslationOrigin);
				if ((!segmentPairInfo.SegmentPair.Properties.IsLocked || !excludeFilterIds.Exists(a => a == "Locked"))
					&& !excludeFilterIds.Exists(a => a == status)
					&& !excludeFilterIds.Exists(a => a == match))
				{
					splitWordCount += segmentPairInfo.SourceWordCounts?.Words ?? 0;
				}

				if (splitWordCount >= maxWordCount)
				{
					segmentPairsSplits.Add(segmentPairSplits);

					splitWordCount = 0;
					segmentPairSplits = new List<SegmentPairInfo>();
				}
			}

			if (segmentPairSplits.Count > 0)
			{
				segmentPairsSplits.Add(segmentPairSplits);
			}

			return segmentPairsSplits;
		}

		private void ProgressLogger(string message, int min, int max)
		{
			if (ProcessingIsIndeterminate)
			{
				_owner.Dispatcher.Invoke(DispatcherPriority.ContextIdle,
					new Action(delegate
					{
						ProcessingIsIndeterminate = false;
					}));
			}

			_owner.Dispatcher.Invoke(DispatcherPriority.ContextIdle,
				new Action(delegate
				{
					ProcessingProgressMessage = message;
					ProcessingCurrentProgress = GetPercentageValue(min, max);
				}));
		}

		private int GetPercentageValue(int index, int total)
		{
			var currentIndex = Convert.ToDouble(index);
			var totalItems = Convert.ToDouble(total);
			var percentage = currentIndex / totalItems * 100;

			var percentageValue = int.Parse(Math.Truncate(percentage).ToString(CultureInfo.InvariantCulture));
			return percentageValue;
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

		private List<string> GetSegmentIds(string segmentIdsString)
		{
			var segmentIds = segmentIdsString.Replace(" ", "").Trim().Split(',').ToList();
			return segmentIds;
		}

		private int GetTotalWordCount(IEnumerable<SegmentPairInfo> segmentPairInfos)
		{
			var excludeFilterIds = SelectedExcludeFilterItems.Select(a => a.Id).ToList();
			var splitWordCount = 0;
			foreach (var segmentPairInfo in segmentPairInfos)
			{
				// include the word count based on the segment status properties selected by the user
				var status = segmentPairInfo.SegmentPair.Properties.ConfirmationLevel.ToString();
				var match = _filterItemService.GetTranslationOriginType(segmentPairInfo.SegmentPair.Target.Properties
					.TranslationOrigin);
				if ((!segmentPairInfo.SegmentPair.Properties.IsLocked || !excludeFilterIds.Exists(a => a == "Locked"))
					&& !excludeFilterIds.Exists(a => a == status)
					&& !excludeFilterIds.Exists(a => a == match))
				{
					splitWordCount += segmentPairInfo.SourceWordCounts?.Words ?? 0;
				}
			}

			return splitWordCount;
		}

		private string GetFilePathOutput(string fileName, string fileFolder, int fileIndex)
		{
			var regexNum = new Regex(@"(?<digits>\[[#]+\])", RegexOptions.None);
			var fileNameOutput = fileName;
			var match = regexNum.Match(fileNameOutput);
			if (match.Success)
			{
				var digits = match.Groups["digits"].Value.Length - 2;
				var fileNamePrefix = fileNameOutput.Substring(0, match.Index);
				var digitsString = fileIndex.ToString().PadLeft(digits, '0');
				var fileNameSuffix = fileNameOutput.Substring(match.Index + match.Length);

				fileNameOutput = fileNamePrefix + digitsString + fileNameSuffix;
			}

			var filePathOutput = Path.Combine(fileFolder, fileNameOutput);
			return filePathOutput;
		}

		private void SetDefaultOutputValues()
		{
			if (_selectedFiles?.Count > 0)
			{
				ExportPath = Path.GetDirectoryName(_selectedFiles[0].LocalFilePath);
				if (_selectedFiles.Count == 1)
				{
					var name = Path.GetFileName(_selectedFiles[0].LocalFilePath);
					FileName = name.Substring(0, name.LastIndexOf(".sdlxliff", StringComparison.CurrentCultureIgnoreCase));
					FileName += ".Split_[####].sdlxliff";
				}
				else
				{
					FileName = "StudioViewsFile.Split_[####].sdlxliff";
				}
			}
		}

		private string GetValidFolderPath(string initialPath)
		{
			if (string.IsNullOrWhiteSpace(initialPath))
			{
				return string.Empty;
			}

			var outputFolder = initialPath;
			if (Directory.Exists(outputFolder))
			{
				return outputFolder;
			}

			while (outputFolder.Contains("\\"))
			{
				outputFolder = outputFolder.Substring(0, outputFolder.LastIndexOf("\\", StringComparison.Ordinal));
				if (Directory.Exists(outputFolder))
				{
					return outputFolder;
				}
			}

			return outputFolder;
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
	}
}
