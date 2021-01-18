using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Sdl.Community.StudioViews.Commands;
using Sdl.Community.StudioViews.Controls.Folder;
using Sdl.Community.StudioViews.Model;
using Sdl.Community.StudioViews.Services;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.MultiSelectComboBox.EventArgs;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StudioViews.ViewModel
{
	public class StudioViewsEditorViewModel : BaseModel, IDisposable
	{
		private readonly EditorController _editorController;
		private readonly FilterItemHelper _filterItemHelper;
		private readonly ProjectHelper _projectHelper;
		private readonly CommonService _commonService;
		private readonly SdlxliffMerger _sdlxliffMerger;
		private readonly SdlxliffExporter _sdlxliffExporter;
		private readonly SdlxliffReader _sdlxliffReader;

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
			FilterItemHelper filterItemHelper, ProjectHelper projectHelper, CommonService commonService,
			SdlxliffMerger sdlxliffMerger, SdlxliffExporter sdlxliffExporter, SdlxliffReader sdlxliffReader)
		{
			_filterItemHelper = filterItemHelper;
			_projectHelper = projectHelper;
			_commonService = commonService;
			_sdlxliffMerger = sdlxliffMerger;
			_sdlxliffExporter = sdlxliffExporter;
			_sdlxliffReader = sdlxliffReader;

			_editorController = editorController;
			_editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;

			ActivateDocument(_editorController.ActiveDocument);

			// Default values
			ExportSelectedSegments = true;
			FilterItems = new List<FilterItem>(_filterItemHelper.GetFilterItems());
			SelectedExcludeFilterItems = new ObservableCollection<FilterItem>(
				_filterItemHelper.GetFilterItems(FilterItems, new List<string> { "Locked" }));
			SelectedTabItem = 0;
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
				var initialDirectory = _sdlxliffMerger.GetValidFolderPath(ExportPath);
				if (string.IsNullOrEmpty(initialDirectory) || !Directory.Exists(initialDirectory))
				{
					initialDirectory = null;
				}

				var folderDialog = new FolderSelectDialog
				{
					Title = "Select the export folder location",
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
					Title = "Select the bilingual SDLXLIFF file"
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
				MessageBox.Show("Directory not found!", "Studio Views", MessageBoxButton.OK, MessageBoxImage.Warning);
			}

			try
			{
				var segmentPairs = _commonService.GetSegmentPairs(_activeDocument, ExportSelectedSegments);
				if (segmentPairs?.Count <= 0)
				{
					MessageBox.Show("No segments selected!", "Studio Views", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				var projectFiles = _commonService.GetProjectFiles(segmentPairs);
				var segmentPairInfos = _commonService.GetSegmentPairInfos(ProjectFileInfos, segmentPairs);
				var filesExported = ExportFiles(projectFiles, segmentPairInfos);

				var message = GetMessage(segmentPairs, filesExported, filesExported[0]);
				if (filesExported.Count > 1)
				{
					var fileDirectory = Path.GetDirectoryName(projectFiles[0].LocalFilePath);
					var filePathOutput = _sdlxliffMerger.GetUniqueFileName(Path.Combine(fileDirectory, "StudioViewsFile.sdlxliff"), "Filtered");

					_sdlxliffMerger.MergeFiles(filesExported, filePathOutput, true);

					message = GetMessage(segmentPairs, filesExported, filePathOutput);
				}

				MessageBox.Show(message, "Studio Views", MessageBoxButton.OK, MessageBoxImage.Information);

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void Import(object param)
		{
			if (_activeDocument == null)
			{
				return;
			}

			if (string.IsNullOrEmpty(ImportPath) || !File.Exists(ImportPath))
			{
				MessageBox.Show("File not found!", "Studio Views", MessageBoxButton.OK, MessageBoxImage.Warning);
			}

			if (!ImportPath.EndsWith(".sdlxliff", StringComparison.InvariantCultureIgnoreCase))
			{
				MessageBox.Show("Expected bilingual SDLXLIFF format!", "Studio Views", MessageBoxButton.OK, MessageBoxImage.Warning);
			}

			try
			{
				var analysisBands = _projectHelper.GetAnalysisBands(_activeDocument.Project);
				var excludeFilterIds = SelectedExcludeFilterItems.Select(a => a.Id).ToList();

				var updatedSegmentPairs = _sdlxliffReader.GetSegmentPairs(ImportPath);

				var importedCount = 0;
				var ignoredCount = 0;

				foreach (var updatedSegmentPair in updatedSegmentPairs)
				{
					if (updatedSegmentPair.ParagraphUnit.IsStructure || !updatedSegmentPair.ParagraphUnit.SegmentPairs.Any())
					{
						continue;
					}

					var segmentPair = _commonService.GetSegmentPair(_activeDocument, updatedSegmentPair.ParagraphUnitId, updatedSegmentPair.SegmentId);
					if (segmentPair == null)
					{
						continue;
					}

					if (excludeFilterIds.Count > 0)
					{
						var status = segmentPair.Properties.ConfirmationLevel.ToString();
						var match = _filterItemHelper.GetTranslationOriginType(segmentPair.Target.Properties.TranslationOrigin, analysisBands);

						if ((segmentPair.Properties.IsLocked && excludeFilterIds.Exists(a => a == "Locked"))
							|| excludeFilterIds.Exists(a => a == status)
							|| excludeFilterIds.Exists(a => a == match))
						{
							ignoredCount++;
							continue;
						}
					}

					segmentPair.Target.Clear();
					foreach (var item in updatedSegmentPair.SegmentPair.Target)
					{
						segmentPair.Target.Add(item.Clone() as IAbstractMarkupData);
					}

					_activeDocument.UpdateSegmentPair(segmentPair);
					_activeDocument.UpdateSegmentPairProperties(segmentPair, updatedSegmentPair.SegmentPair.Properties);

					importedCount++;
				}

				MessageBox.Show(string.Format("Successfully updated the document\r\n\r\nImported: {0}\r\nIgnored: {1}\r\n\r\nInput: {2}",
					importedCount, ignoredCount, ImportPath), "Studio Views", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private List<string> ExportFiles(IEnumerable<ProjectFile> projectFiles, List<SegmentPairInfo> segmentPairInfos)
		{
			var exportFiles = new List<string>();
			foreach (var documentFile in projectFiles)
			{
				var filePathInput = documentFile.LocalFilePath;
				var filePathOutput = _sdlxliffMerger.GetUniqueFileName(Path.Combine(ExportPath, filePathInput), "Filtered");

				var success = _sdlxliffExporter.ExportFile(segmentPairInfos, filePathInput, filePathOutput);
				if (success)
				{
					exportFiles.Add(filePathOutput);
				}
			}

			return exportFiles;
		}

		private string GetMessage(IReadOnlyCollection<ISegmentPair> segmentPairs, IReadOnlyCollection<string> filesExported, string filePathOutput)
		{
			string message;
			message = string.Format("Exported {0} segments from {1} files", segmentPairs.Count, filesExported.Count);
			message += Environment.NewLine + Environment.NewLine;
			message += string.Format("Output: {0}", filePathOutput);
			return message;
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
				ProjectFileInfos = _commonService.GetProjectFileInfos(_activeDocument.Files.ToList());
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
