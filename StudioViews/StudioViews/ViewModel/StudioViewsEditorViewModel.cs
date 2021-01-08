using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.MultiSelectComboBox.EventArgs;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using StudioViews.Commands;
using StudioViews.Controls.Folder;
using StudioViews.Model;
using StudioViews.Services;

namespace StudioViews.ViewModel
{
	public class StudioViewsEditorViewModel : BaseModel, IDisposable
	{
		private readonly EditorController _editorController;
		private readonly FileInfoService _fileInfoService;
		private readonly FilterItemHelper _filterItemHelper;
		private readonly ProjectHelper _projectHelper;

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

		public StudioViewsEditorViewModel(EditorController editorController, FileInfoService fileInfoService,
			FilterItemHelper filterItemHelper, ProjectHelper projectHelper)
		{
			_fileInfoService = fileInfoService;
			_filterItemHelper = filterItemHelper;
			_projectHelper = projectHelper;

			_editorController = editorController;
			_editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;

			ActivateDocument(_editorController.ActiveDocument);

			// Default values
			ExportSelectedSegments = true;
			//OverwriteTranslations = true;
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

		//public bool OverwriteTranslations
		//{
		//	get => _overwriteTranslations;
		//	set
		//	{
		//		if (_overwriteTranslations == value)
		//		{
		//			return;
		//		}

		//		_overwriteTranslations = value;
		//		OnPropertyChanged(nameof(OverwriteTranslations));
		//	}
		//}

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

		private List<ProjectFileInfo> ProjectFiles { get; set; }

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
				var initialDirectory = GetValidFolderPath(ExportPath);
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

			var segmentPairs = ExportSelectedSegments
					? _activeDocument.GetSelectedSegmentPairs().ToList()
					: _activeDocument.FilteredSegmentPairs.ToList();

			var fileNames = string.Empty;
			var projectFiles = new List<ProjectFile>();
			foreach (var segmentPair in segmentPairs)
			{
				var projectFile = segmentPair.GetProjectFile();
				var projectFileId = projectFile.Id.ToString();
				if (!projectFiles.Exists(a => a.Id.ToString() == projectFileId))
				{
					projectFiles.Add(projectFile);
					fileNames = fileNames + "\r\n" + Path.GetFileName(projectFile.LocalFilePath);
				}
			}

			var segmentPairContexts = GetSegmentPairContexts(segmentPairs);
			var exporter = new SdlxliffExporter();

			try
			{
				foreach (var documentFile in projectFiles)
				{
					var filePathInput = documentFile.LocalFilePath;
					var filePathOutput = GetUniqueFileName(Path.Combine(ExportPath, filePathInput), "filtered");
					var success = exporter.ExportFile(segmentPairContexts, filePathInput, filePathOutput);
				}

				MessageBox.Show(string.Format("Exported {0} segments\r\n\r\nFiles {1}", segmentPairs.Count, fileNames), 
					"Studio Views", MessageBoxButton.OK, MessageBoxImage.Information);
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

				var reader = new SdlxliffReader();
				var updatedParagraphUnits = reader.GetParagraphUnits(ImportPath);

				var importedCount = 0;
				var ignoredCount = 0;

				foreach (var updatedParagraphUnit in updatedParagraphUnits)
				{
					if (updatedParagraphUnit.IsStructure || !updatedParagraphUnit.SegmentPairs.Any())
					{
						continue;
					}

					foreach (var updatedSegmentPair in updatedParagraphUnit.SegmentPairs)
					{
						var segmentPair = GetSegmentPair(updatedParagraphUnit.Properties.ParagraphUnitId.Id, updatedSegmentPair.Properties.Id.Id);
						if (segmentPair == null)
						{
							continue;
						}


						var exclude = false;
						if (excludeFilterIds.Count > 0)
						{
							var status = segmentPair.Properties.ConfirmationLevel.ToString();
							var match = _filterItemHelper.GetTranslationOriginType(segmentPair.Target.Properties.TranslationOrigin, analysisBands);

							exclude = (segmentPair.Properties.IsLocked && excludeFilterIds.Exists(a => a == "Locked"))
											|| excludeFilterIds.Exists(a => a == status)
											|| excludeFilterIds.Exists(a => a == match);
						}

						if (exclude)
						{
							ignoredCount++;
							continue;
						}

						segmentPair.Target.Clear();
						foreach (var item in updatedSegmentPair.Target)
						{
							segmentPair.Target.Add(item.Clone() as IAbstractMarkupData);
						}

						_activeDocument.UpdateSegmentPair(segmentPair);

						//segmentPair.Properties.ConfirmationLevel = updatedSegmentPair.Properties.ConfirmationLevel;
						//segmentPair.Properties.TranslationOrigin = updatedSegmentPair.Properties.TranslationOrigin;
						
						//_activeDocument.UpdateSegmentPair(segmentPair);

						_activeDocument.UpdateSegmentPairProperties(segmentPair, updatedSegmentPair.Properties);

						importedCount++;
					}
				}

				MessageBox.Show(string.Format("Successfully updated the document\r\n\r\nImported: {0}\r\nIgnored: {1}\r\nFile: {2}", 
					importedCount, ignoredCount, Path.GetFileName(ImportPath)), "Studio Views", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private ISegmentPair GetSegmentPair(string paragraphUnitId, string segmentId)
		{
			foreach (var segmentPair in _activeDocument.SegmentPairs)
			{
				if (paragraphUnitId == segmentPair.GetParagraphUnitProperties().ParagraphUnitId.Id
					&& segmentId == segmentPair.Properties.Id.Id)
				{
					return segmentPair;
				}
			}

			return null;
		}

		private List<SegmentPairContext> GetSegmentPairContexts(IEnumerable<ISegmentPair> selectedSegmentPairs)
		{
			var segmentPairContexts = new List<SegmentPairContext>();

			foreach (var selectedSegmentPair in selectedSegmentPairs)
			{
				var pp = selectedSegmentPair.GetParagraphUnitProperties();
				
				var segmentPairContext =
					new SegmentPairContext
					{
						ParagraphUnitId = selectedSegmentPair.GetParagraphUnitProperties().ParagraphUnitId.Id,
						SegmentId = selectedSegmentPair.Properties.Id.Id
					};

				segmentPairContext.FileId = GetProjectFileInfo(segmentPairContext.ParagraphUnitId,
					segmentPairContext.SegmentId).FileId;

				segmentPairContexts.Add(segmentPairContext);
			}

			return segmentPairContexts;
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

		private ProjectFileInfo GetProjectFileInfo(string paragraphId, string segmentId)
		{
			if (ProjectFiles == null)
			{
				return null;
			}

			foreach (var projectFile in ProjectFiles)
			{
				var paragraphInfo = projectFile.ParagraphInfos.FirstOrDefault(a => a.ParagraphId == paragraphId);
				var segmentInfo = paragraphInfo?.SegmentInfos.FirstOrDefault(a => a.OriginalSegmentId == segmentId);
				if (segmentInfo != null)
				{
					return projectFile;
				}
			}

			return null;
		}

		private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			ActivateDocument(e.Document);
		}

		private string GetDocumentPath(IStudioDocument document)
		{
			return document?.Files?.FirstOrDefault()?.LocalFilePath;
		}

		private void BackupFile(string fullFilePath)
		{
			if (!File.Exists(fullFilePath))
			{
				return;
			}

			var directoryName = Path.GetDirectoryName(fullFilePath);
			var fileName = Path.GetFileName(fullFilePath);
			var fileExtension = Path.GetExtension(fileName);
			var fileNameWithoutExtension = GetFileNameWithoutExtension(fileName, fileExtension);

			var index = 1;
			var backupPath = Path.Combine(directoryName, fileNameWithoutExtension + "." + index.ToString().PadLeft(3, '0') + fileExtension);

			if (File.Exists(backupPath))
			{
				while (File.Exists(backupPath))
				{
					index++;
					backupPath = Path.Combine(directoryName, fileNameWithoutExtension + "." + index.ToString().PadLeft(3, '0') + fileExtension);
				}
			}

			File.Move(fullFilePath, backupPath);
		}

		private string GetUniqueFileName(string documentPath, string suffix)
		{
			if (!File.Exists(documentPath))
			{
				return null;
			}

			var directoryName = Path.GetDirectoryName(documentPath);
			var fileName = Path.GetFileName(documentPath);
			var fileExtension = Path.GetExtension(fileName);
			var fileNameWithoutExtension = GetFileNameWithoutExtension(fileName, fileExtension);

			var index = 1;
			var uniqueFilePath = Path.Combine(directoryName, fileNameWithoutExtension
															 + "." + suffix + "." + index.ToString().PadLeft(3, '0') + fileExtension);

			if (File.Exists(uniqueFilePath))
			{
				while (File.Exists(uniqueFilePath))
				{
					index++;
					uniqueFilePath = Path.Combine(directoryName, fileNameWithoutExtension
																 + "." + suffix + "." + index.ToString().PadLeft(3, '0') + fileExtension);
				}
			}

			return uniqueFilePath;
		}

		private string GetFileNameWithoutExtension(string fileName, string extension)
		{
			if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(extension))
			{
				return fileName;
			}

			if (extension.Length > fileName.Length || !fileName.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase))
			{
				return fileName;
			}

			return fileName.Substring(0, fileName.Length - extension.Length);
		}

		private void ActivateDocument(IStudioDocument document)
		{
			if (_activeDocument != null)
			{
				ProjectFiles = null;
			}

			_activeDocument = document;
			if (_activeDocument != null)
			{
				ProjectFiles = new List<ProjectFileInfo>();
				foreach (var documentFile in _activeDocument.Files)
				{
					var files = _fileInfoService.GetProjectFiles(documentFile.LocalFilePath);
					foreach (var projectFileInfo in files)
					{
						ProjectFiles.Add(projectFileInfo);
					}
				}

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
