using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;
using Sdl.Community.StudioViews.Actions;
using Sdl.Community.StudioViews.Commands;
using Sdl.Community.StudioViews.Common;
using Sdl.Community.StudioViews.Controls.Folder;
using Sdl.Community.StudioViews.Model;
using Sdl.Community.StudioViews.Providers;
using Sdl.Community.StudioViews.Services;
using Sdl.Community.StudioViews.View;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Rws.MultiSelectComboBox.EventArgs;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StudioViews.ViewModel
{
	public class StudioViewsEditorViewModel : BaseModel, IDisposable
	{
		private readonly EditorController _editorController;
		private readonly FilterItemService _filterItemService;
		private readonly ProjectFileService _projectFileService;
		private readonly SdlxliffMerger _sdlxliffMerger;
		private readonly SdlxliffExporter _sdlxliffExporter;
		private readonly SdlxliffReader _sdlxliffReader;
		private readonly ParagraphUnitProvider _paragraphUnitProvider;
		private readonly DisplayFilter _displayFilter;
		private readonly WordCountProvider _wordCountProvider;

		private IStudioDocument _activeDocument;

		private ICommand _openFolderInExplorerCommand;
		private ICommand _clearFiltersCommand;
		private ICommand _selectedItemsChangedCommand;
		private ICommand _importCommand;
		private ICommand _exportCommand;
		private ICommand _importPathBrowseCommand;
		private ICommand _exportPathBrowseCommand;
		private ICommand _dragDropCommand;

		private string _importPath;
		private string _exportPath;
		private bool _importPathIsValid;
		private bool _exportPathIsValid;
		private List<FilterItem> _filterItems;
		private ObservableCollection<FilterItem> _selectedExcludeFilterItems;

		private bool _exportSelectedSegments;
		private bool _exportVisibleSegments;

		private int _selectedTabItem;

		public StudioViewsEditorViewModel(EditorController editorController,
			FilterItemService filterItemService, ProjectFileService projectFileService,
			SdlxliffMerger sdlxliffMerger, SdlxliffExporter sdlxliffExporter, SdlxliffReader sdlxliffReader,
			ParagraphUnitProvider paragraphUnitProvider, DisplayFilter displayFilter, WordCountProvider wordCountProvider)
		{
			_filterItemService = filterItemService;
			_projectFileService = projectFileService;
			_sdlxliffMerger = sdlxliffMerger;
			_sdlxliffExporter = sdlxliffExporter;
			_sdlxliffReader = sdlxliffReader;
			_paragraphUnitProvider = paragraphUnitProvider;
			_displayFilter = displayFilter;

			_editorController = editorController;
			_editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;

			ActivateDocument(_editorController.ActiveDocument);

			// Default values
			ExportSelectedSegments = true;
			FilterItems = new List<FilterItem>(_filterItemService.GetFilterItems());
			SelectedExcludeFilterItems = new ObservableCollection<FilterItem>(
				_filterItemService.GetFilterItems(FilterItems, new List<string> { "Locked" }));
			SelectedTabItem = 0;
			_wordCountProvider = wordCountProvider;
		}

		public ICommand OpenFolderInExplorerCommand => _openFolderInExplorerCommand ?? (_openFolderInExplorerCommand = new CommandHandler(OpenFolderInExplorer));

		public ICommand ClearFiltersCommand => _clearFiltersCommand ?? (_clearFiltersCommand = new CommandHandler(ClearFilters));

		public ICommand SelectedItemsChangedCommand => _selectedItemsChangedCommand ?? (_selectedItemsChangedCommand = new CommandHandler(SelectedItemsChanged));

		public ICommand ExportCommand => _exportCommand ?? (_exportCommand = new CommandHandler(Export));

		public ICommand ExportPathBrowseCommand => _exportPathBrowseCommand ?? (_exportPathBrowseCommand = new CommandHandler(ExportPathBrowse));

		public ICommand ImportCommand => _importCommand ?? (_importCommand = new CommandHandler(Import));

		public ICommand ImportPathBrowseCommand => _importPathBrowseCommand ?? (_importPathBrowseCommand = new CommandHandler(ImportPathBrowse));

		public ICommand DragDropCommand => _dragDropCommand ?? (_dragDropCommand = new CommandHandler(DragAndDrop));

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

		public int SelectedTabItem
		{
			get => _selectedTabItem;
			set
			{
				if (_selectedTabItem == value)
				{
					return;
				}

				_selectedTabItem = value;
				OnPropertyChanged(nameof(SelectedTabItem));
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

		public string ImportPath
		{
			get => _importPath;
			set
			{
				if (_importPath == value)
				{
					return;
				}

				_importPath = value;
				OnPropertyChanged(nameof(ImportPath));

				ImportPathIsValid = File.Exists(ImportPath);
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
			}
		}

		public bool ImportPathIsValid
		{
			get => _importPathIsValid;
			set
			{
				if (_importPathIsValid == value)
				{
					return;
				}

				_importPathIsValid = value;
				OnPropertyChanged(nameof(ImportPathIsValid));
			}
		}

		public bool ExportSelectedSegments
		{
			get => _exportSelectedSegments;
			set
			{
				if (_exportSelectedSegments == value)
				{
					return;
				}

				_exportSelectedSegments = value;
				OnPropertyChanged(nameof(ExportSelectedSegments));
			}
		}

		public bool ExportVisibleSegments
		{
			get => _exportVisibleSegments;
			set
			{
				if (_exportVisibleSegments == value)
				{
					return;
				}

				_exportVisibleSegments = value;
				OnPropertyChanged(nameof(ExportVisibleSegments));
			}
		}

		public string LogFilePath { get; private set; }

		public DateTime ProcessingDateTime { get; private set; }

		private List<ProjectFileInfo> ProjectFileInfos { get; set; }

		private void OpenFolderInExplorer(object parameter)
		{
			if (Directory.Exists(ExportPath))
			{
				Process.Start(ExportPath);
			}
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

		private void ExportPathBrowse(object parameter)
		{
			try
			{
				var initialDirectory = _projectFileService.GetValidFolderPath(ExportPath);
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

		private void ImportPathBrowse(object parameter)
		{
			try
			{
				var openFileDialog = new OpenFileDialog
				{
					Title = PluginResources.OpenFileDialog_Select_Import_File
				};

				var myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

				var initialDirectory = !string.IsNullOrEmpty(ImportPath) ? Path.GetDirectoryName(ImportPath) : string.Empty;

				openFileDialog.InitialDirectory = !string.IsNullOrEmpty(initialDirectory) && Directory.Exists(initialDirectory)
					? initialDirectory
					: myDocuments;
				openFileDialog.Filter = "SDLXLIFF files|*.sdlxliff";
				openFileDialog.FilterIndex = 0;
				openFileDialog.RestoreDirectory = true;
				openFileDialog.Multiselect = false;

				if (openFileDialog.ShowDialog() == true)
				{
					if (!string.IsNullOrEmpty(openFileDialog.FileName) && File.Exists(openFileDialog.FileName))
					{
						ImportPath = openFileDialog.FileName;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, PluginResources.Plugin_Name, MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void DragAndDrop(object parameter)
		{
			if (!(parameter is DragEventArgs eventArgs))
			{
				return;
			}

			if (SelectedTabItem == 0)
			{
				ExportPathDragAndDrop(eventArgs);
			}
			else
			{
				ImportPathDragAndDrop(eventArgs);
			}
		}

		private void ExportPathDragAndDrop(DragEventArgs eventArgs)
		{
			var fileDrop = eventArgs.Data.GetData(DataFormats.FileDrop, false);
			if (fileDrop is string[] filesOrDirectories && filesOrDirectories.Length == 1)
			{
				var fullPath = filesOrDirectories.FirstOrDefault();
				var fileAttributes = File.GetAttributes(fullPath);
				if (fileAttributes.HasFlag(FileAttributes.Directory))
				{
					ExportPath = fullPath;
				}
			}
		}

		private void ImportPathDragAndDrop(DragEventArgs eventArgs)
		{
			var fileDrop = eventArgs.Data.GetData(DataFormats.FileDrop, false);
			if (fileDrop is string[] filesOrDirectories && filesOrDirectories.Length == 1)
			{
				var fullPath = filesOrDirectories.FirstOrDefault();
				var fileAttributes = File.GetAttributes(fullPath);
				if (!fileAttributes.HasFlag(FileAttributes.Directory) &&
					fullPath.EndsWith(".sdlxliff", StringComparison.InvariantCultureIgnoreCase))
				{
					ImportPath = fullPath;
				}
			}
		}

		private void Export(object param)
		{
			if (_activeDocument == null)
			{
				return;
			}

			if (string.IsNullOrEmpty(ExportPath) || !Directory.Exists(ExportPath) || File.Exists(ExportPath))
			{
				MessageBox.Show(PluginResources.Error_Message_Directory_Not_Found, PluginResources.Plugin_Name, MessageBoxButton.OK, MessageBoxImage.Warning);
			}

			ProcessingDateTime = DateTime.Now;
			var logFileName = "StudioViews_" + "Filtered" + "_" + _projectFileService.GetDateTimeToFilePartString(ProcessingDateTime) + ".log";
			LogFilePath = Path.Combine(ExportPath, logFileName);

			try
			{
				var segmentPairs = _projectFileService.GetSegmentPairs(_activeDocument, ExportSelectedSegments);
				if (segmentPairs?.Count <= 0)
				{
					MessageBox.Show(PluginResources.Error_Message_No_Segments_Selected, PluginResources.Plugin_Name, MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				var projectFiles = _projectFileService.GetProjectFiles(segmentPairs);
				var segmentPairInfos = _projectFileService.GetSegmentPairInfos(ProjectFileInfos, segmentPairs);
				var exportResult = ExportFiles(projectFiles, segmentPairInfos);
				var filePathOutput = exportResult.OutputFiles[0].FilePath;

				if (exportResult.OutputFiles.Count > 1)
				{
					filePathOutput = _projectFileService.GetUniqueFileName(Path.Combine(ExportPath, "StudioViewsFile.sdlxliff"), "Filtered");
					_sdlxliffMerger.MergeFiles(exportResult.OutputFiles.Select(a => a.FilePath).ToList(), filePathOutput, true, ProgressLogger);

					var outputFile = new OutputFile
					{
						FilePath = filePathOutput,
						SegmentCount = exportResult.OutputFiles.Sum(a => a.SegmentCount),
						WordCount = exportResult.OutputFiles.Sum(a => a.WordCount)
					};

					exportResult.OutputFiles = new List<OutputFile> { outputFile };
				}

				WriteFilteredLogFile(exportResult, LogFilePath);

				var message = PluginResources.Message_Successfully_Completed_Filter_Operation;
				message += Environment.NewLine + Environment.NewLine;
				message += string.Format(PluginResources.Message_Exported_Segments_From_Files,
					exportResult.OutputFiles.Sum(a => a.SegmentCount), exportResult.InputFiles.Count);
				message += Environment.NewLine + Environment.NewLine;
				message += string.Format(PluginResources.Message_Export_File, filePathOutput);

				ShowMessage(true, message, LogFilePath, ExportPath);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void ProgressLogger(string message, int min, int max)
		{
			
		}
		private void Import(object param)
		{
			if (_activeDocument == null)
			{
				return;
			}

			if (string.IsNullOrEmpty(ImportPath) || !File.Exists(ImportPath))
			{
				MessageBox.Show(PluginResources.Error_Message_File_Not_Found, PluginResources.Plugin_Name, MessageBoxButton.OK, MessageBoxImage.Warning);
			}

			if (!ImportPath.EndsWith(".sdlxliff", StringComparison.InvariantCultureIgnoreCase))
			{
				MessageBox.Show(PluginResources.Error_Message_Expected_Bilingual_Format, PluginResources.Plugin_Name, MessageBoxButton.OK, MessageBoxImage.Warning);
			}

			ProcessingDateTime = DateTime.Now;
			var logFileName = "StudioViews_" + "Import" + "_" + _projectFileService.GetDateTimeToFilePartString(ProcessingDateTime) + ".log";
			LogFilePath = Path.Combine(ExportPath, logFileName);

			try
			{
				var excludeFilterIds = SelectedExcludeFilterItems.Select(a => a.Id).ToList();
				var importResult = ImportFile(ImportPath, excludeFilterIds);

				if (!importResult.Success && importResult.Message == Constants.AlignmentDifferences)
				{
					ImportFileWithAlignmentDifferences(importResult.UpdatedFilePath, _activeDocument.ActiveFile.Language);
				}
				else
				{
					WriteImportLogFile(importResult, ImportPath, LogFilePath);

					string message;
					if (!importResult.Success && !string.IsNullOrEmpty(importResult.Message))
					{
						message = importResult.Message;
					}
					else
					{
						message = PluginResources.Message_Successfully_Updated_Document;
						message += Environment.NewLine + Environment.NewLine;
						message += PluginResources.Message_Segments;
						message += Environment.NewLine;
						message += string.Format(PluginResources.Message_Tab_Updated, importResult.UpdatedSegments);
						message += Environment.NewLine;
						message += string.Format(PluginResources.Message_Tab_Excluded, importResult.ExcludedSegments);
						message += Environment.NewLine + Environment.NewLine;
						message += string.Format(PluginResources.Message_Import_File, ImportPath);
					}

					ShowMessage(true, message, LogFilePath, Path.GetDirectoryName(importResult.FilePath));
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void ImportFileWithAlignmentDifferences(string updatedFilePath, Language language)
		{
			var dialogResult = MessageBox.Show(PluginResources.Message_UnableToImportTranslationsFromEditor +
				Environment.NewLine + Environment.NewLine + PluginResources.Message_SelectYesToProceedWithImport,
				PluginResources.Plugin_Name, MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (dialogResult == MessageBoxResult.Yes)
			{
				_editorController.Save(_activeDocument);
				_editorController.Close(_activeDocument);

				var importAction = SdlTradosStudio.Application.GetAction<ImportSelectedFilesAction>();
				importAction.Execute(new List<SystemFileInfo> { new SystemFileInfo(updatedFilePath) }, language);
			}
		}

		private void WriteImportLogFile(ImportResult importResult, string importFilePath, string logFilePath)
		{
			Dispatcher.CurrentDispatcher.Invoke(
				delegate
				{
					using (var sr = new StreamWriter(logFilePath, false, Encoding.UTF8))
					{
						sr.WriteLine(PluginResources.Plugin_Name);
						sr.WriteLine(PluginResources.LogFile_Title_Task_Import_Files);
						sr.WriteLine(PluginResources.LogFile_Label_Start_Processing + _projectFileService.GetDateTimeToString(ProcessingDateTime));

						if (SelectedExcludeFilterItems.Count > 0)
						{
							sr.WriteLine(string.Empty);
							var filterItems = _filterItemService.GetFilterItemsText(SelectedExcludeFilterItems.ToList());
							sr.WriteLine(PluginResources.LogFile_Label_Exclude_Filters + filterItems);
						}

						sr.WriteLine(string.Empty);
						sr.WriteLine(PluginResources.LogFile_Label_Import_Files, 1);
						sr.WriteLine(PluginResources.LogFile_Tab_Label_File_Number, importFilePath);


						sr.WriteLine(string.Empty);
						sr.WriteLine(PluginResources.LogFile_Label_Updated_Files, importResult.UpdatedSegments > 0);
						sr.WriteLine(PluginResources.LogFile_Label_Updated_Segments, importResult.UpdatedSegments);
						sr.WriteLine(string.Empty);
						sr.WriteLine(PluginResources.LogFile_Label_Selected_Files, 1);

						sr.WriteLine(PluginResources.LogFile_Label_File_Number_Path, 1, importResult.FilePath);
						sr.WriteLine(PluginResources.LogFile_Label_Tab_Success + importResult.Success);
						sr.WriteLine(PluginResources.LogFile_Label_Tab_Updated + (importResult.UpdatedSegments > 0 ? "True" : "False"));
						if (importResult.UpdatedSegments > 0)
						{
							sr.WriteLine(PluginResources.LogFile_Label_Tab_Backup + importResult.BackupFilePath);
						}
						sr.WriteLine(PluginResources.Message_Tab_Segments);
						sr.WriteLine(PluginResources.Message_Tab_Tab_Updated, importResult.UpdatedSegments);
						sr.WriteLine(PluginResources.Message_Tab_Tab_Excluded, importResult.ExcludedSegments);

						sr.WriteLine(string.Empty);
						sr.WriteLine(PluginResources.LogFile_Label_End_Processing, _projectFileService.GetDateTimeToString(DateTime.Now));

						sr.Flush();
						sr.Close();
					}
				});
		}

		private void WriteFilteredLogFile(ExportResult exportResult, string logFilePath)
		{
			Dispatcher.CurrentDispatcher.Invoke(
				delegate
				{
					using (var sr = new StreamWriter(logFilePath, false, Encoding.UTF8))
					{
						sr.WriteLine(PluginResources.Plugin_Name);
						sr.WriteLine(PluginResources.LogFile_Title_Task_Export_Filtered);
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
							if (file.WordCount > 0)
							{
								sr.WriteLine(PluginResources.Message_Tab_Tab_Words_Number, file.WordCount);
							}

							sr.WriteLine(string.Empty);
						}

						sr.WriteLine(string.Empty);
						sr.WriteLine(PluginResources.LogFile_Label_End_Processing, _projectFileService.GetDateTimeToString(DateTime.Now));

						sr.Flush();
						sr.Close();
					}
				});
		}

		private ImportResult ImportFile(string importFilePath, List<string> excludeFilterIds)
		{
			var filePath = GetDocumentPath(_activeDocument);
			var importResult = new ImportResult
			{
				FilePath = filePath,
				UpdatedFilePath = importFilePath,
				BackupFilePath = _projectFileService.GetUniqueFileName(filePath, "Backup"),
				UpdatedSegments = 0,
				ExcludedSegments = 0
			};

			if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
			{
				importResult.Success = false;
				importResult.Message = PluginResources.Error_Message_Unable_To_Locate_Document_File;
				return importResult;
			}

			_activeDocument.Project.Save();
			File.Copy(importResult.FilePath, importResult.BackupFilePath, true);

			var updatedSegmentPairs = _sdlxliffReader.GetSegmentPairs(importFilePath);

			// If there are alignment differences, then the import needs to be performed after the document is closed
			var alignmentDifferences = HasAlignmentDifferences(updatedSegmentPairs);
			if (alignmentDifferences)
			{
				importResult.Success = false;
				importResult.Message = Constants.AlignmentDifferences;
				return importResult;
			}

			var refreshDocument = false;
			foreach (var updatedSegmentPair in updatedSegmentPairs)
			{
				if (updatedSegmentPair.ParagraphUnit.IsStructure || !updatedSegmentPair.ParagraphUnit.SegmentPairs.Any())
				{
					continue;
				}

				var segmentPair = _projectFileService.GetSegmentPair(_activeDocument, updatedSegmentPair.ParagraphUnitId,
					updatedSegmentPair.SegmentId);
				
				if (segmentPair?.Target == null)
				{
					continue;
				}

				if (IsSame(segmentPair.Source, updatedSegmentPair.SegmentPair?.Source) &&
				    IsSame(segmentPair.Target, updatedSegmentPair.SegmentPair?.Target))
				{
					continue;
				}

				if (IsEmpty(updatedSegmentPair.SegmentPair?.Source) && !IsEmpty(segmentPair.Source) &&
				    updatedSegmentPair.SegmentPair?.Properties?.TranslationOrigin?.OriginSystem == Constants.MergedParagraph)
				{
					UpdateSegmentPair(segmentPair, updatedSegmentPair);

					importResult.UpdatedSegments++;
					refreshDocument = true;
					continue;
				}

				var excludeSegment = ExcludeSegment(excludeFilterIds, segmentPair);
				if (excludeSegment)
				{
					importResult.ExcludedSegments++;
					continue;
				}

				UpdateSegmentPair(segmentPair, updatedSegmentPair);
				importResult.UpdatedSegments++;
			}

			if (refreshDocument)
			{
				_activeDocument.ApplyFilterOnSegments(_activeDocument.DisplayFilter ?? _displayFilter);
			}

			return importResult;
		}

		private void UpdateSegmentPair(ISegmentPair segmentPair, SegmentPairInfo updatedSegmentPair)
		{
			segmentPair.Source.Clear();
			segmentPair.Target.Clear();

			foreach (var item in updatedSegmentPair.SegmentPair.Source)
			{
				segmentPair.Source.Add(item.Clone() as IAbstractMarkupData);
			}

			foreach (var item in updatedSegmentPair.SegmentPair.Target)
			{
				segmentPair.Target.Add(item.Clone() as IAbstractMarkupData);
			}

			_activeDocument.UpdateSegmentPair(segmentPair);
			_activeDocument.UpdateSegmentPairProperties(segmentPair, updatedSegmentPair.SegmentPair.Properties);
		}

		private bool ExcludeSegment(List<string> excludeFilterIds, ISegmentPair segmentPair)
		{
			var exclude = false;
			if (excludeFilterIds.Count > 0)
			{
				var status = segmentPair.Properties.ConfirmationLevel.ToString();
				var match = _filterItemService.GetTranslationOriginType(segmentPair.Target.Properties.TranslationOrigin);

				if ((segmentPair.Properties.IsLocked && excludeFilterIds.Exists(a => a == "Locked"))
				    || excludeFilterIds.Exists(a => a == status)
				    || excludeFilterIds.Exists(a => a == match))
				{
					exclude = true;
				}
			}

			return exclude;
		}

		private bool HasAlignmentDifferences(IEnumerable<SegmentPairInfo> updatedSegmentPairs)
		{
			var alignmentDifferences = false;
			var updatedParagraphUnits = GetUpdatedParagraphUnits(updatedSegmentPairs);
			foreach (var updatedParagraphUnit in updatedParagraphUnits)
			{
				var originalParagraphUnit = _projectFileService.GetParagraphUnit(_activeDocument,
					updatedParagraphUnit.ParagraphUnit.Properties.ParagraphUnitId.Id);
				if (originalParagraphUnit == null)
				{
					continue;
				}

				var alignments =
					_paragraphUnitProvider.GetSegmentPairAlignments(originalParagraphUnit,
						updatedParagraphUnit.ParagraphUnit);
				if (alignments.Exists(a => a.Alignment == AlignmentInfo.AlignmentType.Added ||
										  a.Alignment == AlignmentInfo.AlignmentType.Removed))
				{
					alignmentDifferences = true;
				}
			}

			return alignmentDifferences;
		}

		private static IEnumerable<ParagraphUnitInfo> GetUpdatedParagraphUnits(IEnumerable<SegmentPairInfo> updatedSegmentPairs)
		{
			var updatedParagraphUnits = new List<ParagraphUnitInfo>();
			foreach (var segmentPair in updatedSegmentPairs)
			{
				var paragraphUnit = updatedParagraphUnits.FirstOrDefault(
					a => a.ParagraphUnit.Properties.ParagraphUnitId.Id == segmentPair.ParagraphUnitId);

				if (paragraphUnit != null)
				{
					paragraphUnit.SegmentPairs.Add(segmentPair);
				}
				else
				{
					var paragraphUnitInfo = new ParagraphUnitInfo
					{
						ParagraphUnit = segmentPair.ParagraphUnit,
						SegmentPairs = new List<SegmentPairInfo> { segmentPair },
						FileId = segmentPair.FileId
					};
					updatedParagraphUnits.Add(paragraphUnitInfo);
				}
			}

			return updatedParagraphUnits;
		}

		private void ShowMessage(bool success, string message, string logFilePath, string folder)
		{
			var messageInfo = new MessageInfo
			{
				Title = PluginResources.Message_Title_Task_Result,
				Message = message,
				LogFilePath = logFilePath,
				Folder = folder,
				ShowImage = true,
				ImageUrl = success
					? "/Sdl.Community.StudioViews;component/Resources/information.png"
					: "/Sdl.Community.StudioViews;component/Resources/warning.png"
			};

			var messageView = new MessageBoxView();
			var messageViewModel = new MessageBoxViewModel(messageView, messageInfo);
			messageView.DataContext = messageViewModel;

			messageView.ShowDialog();
		}

		private static bool IsSame(ISegment segment, ISegment updatedSegment)
		{
			var originalTarget = segment.ToString();
			var updatedTarget = updatedSegment.ToString();

			var isSame = (originalTarget == updatedTarget) &&
						 (segment.Properties.IsLocked == updatedSegment.Properties.IsLocked) &&
						 (segment.Properties.ConfirmationLevel == updatedSegment.Properties.ConfirmationLevel);

			return isSame;
		}

		private static bool IsEmpty(ISegment segment)
		{
			return segment.ToString().Trim() == string.Empty;
		}

		private ExportResult ExportFiles(IReadOnlyCollection<ProjectFile> projectFiles, List<SegmentPairInfo> segmentPairInfos)
		{
			var exportResult = new ExportResult
			{
				InputFiles = new List<string>(projectFiles.Select(a => a.LocalFilePath))
			};

			var sourceLanguage = projectFiles.FirstOrDefault()?.SourceFile.Language.CultureInfo;
			var targetLanguage = projectFiles.FirstOrDefault()?.Language.CultureInfo;

			foreach (var documentFile in projectFiles)
			{
				var filePathInput = documentFile.LocalFilePath;
				var filePathInputName = Path.GetFileName(filePathInput);
				var filePathOutput = _projectFileService.GetUniqueFileName(Path.Combine(ExportPath, filePathInputName), "Filtered");

				var outputFile = _sdlxliffExporter.ExportFile(segmentPairInfos, filePathInput, filePathOutput, _wordCountProvider, ProgressLogger);
				if (outputFile != null)
				{
					exportResult.OutputFiles.Add(outputFile);
				}
			}

			exportResult.Success = true;
			return exportResult;
		}

		private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			ActivateDocument(e.Document);
		}

		private string GetDocumentPath(IStudioDocument document)
		{
			return document?.Files?.FirstOrDefault()?.LocalFilePath;
		}

		private void ActivateDocument(IStudioDocument document)
		{
			if (_activeDocument != null)
			{
				ProjectFileInfos = null;
			}

			_activeDocument = document;
			if (_activeDocument != null)
			{
				ProjectFileInfos = _projectFileService.GetProjectFileInfos(_activeDocument.Files.ToList());
				ExportPath = Path.GetDirectoryName(GetDocumentPath(_activeDocument));
			}
		}

		public void Dispose()
		{
			if (_editorController != null)
			{
				_editorController.ActiveDocumentChanged -= EditorController_ActiveDocumentChanged;
			}
		}
	}
}
