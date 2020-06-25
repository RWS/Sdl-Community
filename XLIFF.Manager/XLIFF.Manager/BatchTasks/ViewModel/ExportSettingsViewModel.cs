using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Interfaces;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Core.Globalization;
using Sdl.MultiSelectComboBox.API;
using Sdl.MultiSelectComboBox.EventArgs;

namespace Sdl.Community.XLIFF.Manager.BatchTasks.ViewModel
{
	public class ExportSettingsViewModel : INotifyPropertyChanged
	{
		private readonly IDialogService _dialogService;
		private List<XLIFFSupport> _xliffSupport;
		private XLIFFSupport _selectedXliffSupportModel;
		private string _outputFolder;
		private bool _copySourceToTarget;
		private bool _copySourceToTargetEnabled;
		private bool _includeTranslations;
		private IList _selectedProjectFiles;
		private List<ProjectFile> _projectFiles;
		private ProjectFile _selectedProjectFile;
		private ObservableCollection<FilterItem> _filterItems;
		private ObservableCollection<FilterItem> _selectedExcludeFilterItems;
		private ICommand _clearExportFileCommand;
		private ICommand _browseFolderCommand;
		private ICommand _clearFiltersCommand;
		private ICommand _checkAllCommand;
		private ICommand _checkSelectedComand;
		private ICommand _selectedItemsChangedCommand;
		private bool _checkedAll;
		private bool _checkingAllAction;
		private int _tabControlSelectedIndex;

		public ExportSettingsViewModel(ExportSettings exportSettings, IDialogService dialogService)
		{
			_dialogService = dialogService;

			if (exportSettings.ExportOptions == null)
			{
				exportSettings.ExportOptions = new ExportOptions();
			}			

			ProjectFiles = exportSettings.ProjectFiles;
			VerifyProjectFiles();

			SelectedXliffSupport = XLIFFSupportList.FirstOrDefault(a => a.SupportType == exportSettings.ExportOptions.XliffSupport);
			OutputFolder = exportSettings.TransactionFolder;
			CopySourceToTarget = exportSettings.ExportOptions.CopySourceToTarget;
			IncludeTranslations = exportSettings.ExportOptions.IncludeTranslations;

			InitializeFilterItems(exportSettings.SelectedFilterItemIds);
		}

		public int TabControlSelectedIndex
		{
			get => _tabControlSelectedIndex;
			set
			{
				if (_tabControlSelectedIndex == value)
				{
					return;
				}

				_tabControlSelectedIndex = value;
				OnPropertyChanged(nameof(TabControlSelectedIndex));
			}
		}


		public ICommand CheckAllCommand => _checkAllCommand ?? (_checkAllCommand = new RelayCommand(CheckAll));

		public ICommand CheckSelectedCommand => _checkSelectedComand ?? (_checkSelectedComand = new CommandHandler(CheckSelected));

		public ICommand ClearExportFileCommand => _clearExportFileCommand ?? (_clearExportFileCommand = new CommandHandler(ClearExportFile));

		public ICommand BrowseFolderCommand => _browseFolderCommand ?? (_browseFolderCommand = new CommandHandler(BrowseFolder));

		public ICommand ClearFiltersCommand => _clearFiltersCommand ?? (_clearFiltersCommand = new CommandHandler(ClearFilters));

		public ICommand SelectedItemsChangedCommand => _selectedItemsChangedCommand ?? (_selectedItemsChangedCommand = new CommandHandler(SelectedItemsChanged));

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

				OnPropertyChanged(nameof(ProjectFiles));
				OnPropertyChanged(nameof(StatusLabel));
			}
		}

		public ProjectFile SelectedProjectFile
		{
			get => _selectedProjectFile;
			set
			{
				_selectedProjectFile = value;
				OnPropertyChanged(nameof(SelectedProjectFile));
			}
		}

		public IList SelectedProjectFiles
		{
			get => _selectedProjectFiles ?? (_selectedProjectFiles = new ObservableCollection<ProjectFile>());
			set
			{
				_selectedProjectFiles = value;
				OnPropertyChanged(nameof(SelectedProjectFiles));
			}
		}

		public List<XLIFFSupport> XLIFFSupportList
		{
			get
			{
				if (_xliffSupport != null)
				{
					return _xliffSupport;
				}

				_xliffSupport = new List<XLIFFSupport>
				{
					new XLIFFSupport
					{
						Name = "XLIFF 1.2 SDL",
						SupportType = Enumerators.XLIFFSupport.xliff12sdl
					},
					new XLIFFSupport
					{
						Name = "XLIFF 1.2 Polyglot",
						SupportType = Enumerators.XLIFFSupport.xliff12polyglot
					}
					// TODO spport for this format will come later on in the development cycle
					//new XLIFFSupportModel
					//{
					//	Name = "XLIFF 2.0 SDL",
					//	SupportType = Enumerators.XLIFFSupport.xliff20sdl
					//}
				};

				return _xliffSupport;
			}
		}

		public XLIFFSupport SelectedXliffSupport
		{
			get
			{
				return _selectedXliffSupportModel
					   ?? (_selectedXliffSupportModel = XLIFFSupportList.FirstOrDefault(a => a.SupportType == Enumerators.XLIFFSupport.xliff12polyglot));
			}
			set
			{
				if (_selectedXliffSupportModel == value)
				{
					return;
				}

				_selectedXliffSupportModel = value;
				OnPropertyChanged(nameof(SelectedXliffSupport));
			}
		}

		public ObservableCollection<FilterItem> FilterItems
		{
			get => _filterItems ?? (_filterItems = new ObservableCollection<FilterItem>());
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

		public string OutputFolder
		{
			get => _outputFolder;
			set
			{
				if (_outputFolder == value)
				{
					return;
				}

				_outputFolder = value;
				OnPropertyChanged(nameof(OutputFolder));

				VerifyIsValid();
			}
		}

		public bool CopySourceToTarget
		{
			get => _copySourceToTarget;
			set
			{
				if (_copySourceToTarget == value)
				{
					return;
				}

				_copySourceToTarget = value;
				OnPropertyChanged(nameof(CopySourceToTarget));

				VerifyIsValid();
			}
		}

		public bool CopySourceToTargetEnabled
		{
			get => _copySourceToTargetEnabled;
			set
			{
				if (_copySourceToTargetEnabled == value)
				{
					return;
				}

				_copySourceToTargetEnabled = value;
				OnPropertyChanged(nameof(CopySourceToTargetEnabled));
			}
		}

		public bool IncludeTranslations
		{
			get => _includeTranslations;
			set
			{
				if (_includeTranslations == value)
				{
					return;
				}

				_includeTranslations = value;
				OnPropertyChanged(nameof(IncludeTranslations));

				if (!_includeTranslations)
				{
					CopySourceToTarget = false;
					CopySourceToTargetEnabled = false;
				}
				else
				{
					CopySourceToTargetEnabled = true;
				}

				VerifyIsValid();
			}
		}


		private void InitializeFilterItems(IReadOnlyCollection<string> selecteFilterItemIds)
		{
			var filterItems = new List<FilterItem>();

			AddSegmentPropertyFilters(filterItems, new FilterItemGroup(1, "Properties"));
			AddSegmentStatusFilters(filterItems, new FilterItemGroup(2, "Status"));
			AddMatchTypeFilters(filterItems, new FilterItemGroup(3, "Match"));

			FilterItems = new ObservableCollection<FilterItem>(filterItems);

			SetSelectedFilterItems(selecteFilterItemIds);
		}

		private void SetSelectedFilterItems(IReadOnlyCollection<string> selecteFilterItemIds)
		{
			if (selecteFilterItemIds != null && selecteFilterItemIds.Any())
			{
				var selectedFilterItems = new List<FilterItem>();
				foreach (var id in selecteFilterItemIds)
				{
					var filterItem = FilterItems.FirstOrDefault(a => a.Id == id);
					if (filterItem != null && selectedFilterItems.Exists(a => a.Id == id))
					{
						selectedFilterItems.Add(filterItem);
					}
				}

				SelectedExcludeFilterItems = new ObservableCollection<FilterItem>(selectedFilterItems);
			}
		}


		private void AddMatchTypeFilters(ICollection<FilterItem> filterItems, IItemGroup filterItemGroup)
		{
			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = "PM",
				Name = "Perfect Match"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = "CM",
				Name = "Context Match"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = "Exact",
				Name = "Exact Match"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = "MT",
				Name = "Machine Translation"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = "AMT",
				Name = "Adaptive Machine Translation"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = "NMT",
				Name = "Neural Machine Translation"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = "Fuzzy",
				Name = "Fuzzy Match"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = "New",
				Name = "New"
			});
		}

		private void AddSegmentStatusFilters(ICollection<FilterItem> filterItems, IItemGroup filterItemGroup)
		{
			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = ConfirmationLevel.Unspecified.ToString(),
				Name = "Unspecified"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = ConfirmationLevel.Draft.ToString(),
				Name = "Draft"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = ConfirmationLevel.Translated.ToString(),
				Name = "Translated"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = ConfirmationLevel.RejectedTranslation.ToString(),
				Name = "Translation Rejected"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = ConfirmationLevel.ApprovedTranslation.ToString(),
				Name = "Translation Approved"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = ConfirmationLevel.RejectedSignOff.ToString(),
				Name = "SignOff Rejected"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = ConfirmationLevel.ApprovedSignOff.ToString(),
				Name = "SignOff Approved"
			});
		}

		private void AddSegmentPropertyFilters(ICollection<FilterItem> filterItems, IItemGroup filterItemGroup)
		{
			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = "Locked",
				Name = "Locked"
			});
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

		private void UpdateCheckAll()
		{
			CheckedAll = ProjectFiles.Count == ProjectFiles.Count(a => a.Selected);
			OnPropertyChanged(nameof(StatusLabel));
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

		private void VerifyProjectFiles()
		{
			foreach (var projectFile in ProjectFiles)
			{
				if (projectFile.Action == Enumerators.Action.Export)
				{
					var activityfile = projectFile.ProjectFileActivities.OrderByDescending(a => a.Date).FirstOrDefault(a => a.Action == Enumerators.Action.Export);

					projectFile.Status = Enumerators.Status.Warning;
					projectFile.ShortMessage = string.Format(PluginResources.Message_Exported_on_0, activityfile?.DateToString);					
				}
				else if (projectFile.Action == Enumerators.Action.Import)
				{
					var activityfile = projectFile.ProjectFileActivities.OrderByDescending(a => a.Date).FirstOrDefault(a => a.Action == Enumerators.Action.Import);

					projectFile.Status = Enumerators.Status.Warning;
					projectFile.ShortMessage = string.Format(PluginResources.Message_Imported_on_0, activityfile?.DateToString);
				}
				else
				{
					projectFile.Status = Enumerators.Status.Ready;
					projectFile.ShortMessage = string.Empty;
					projectFile.Details = string.Empty;
				}
			}
		}

		private void ProjectFile_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!_checkingAllAction && e.PropertyName == nameof(ProjectFile.Selected))
			{
				UpdateCheckAll();
			}

			VerifyIsValid();
		}

		private void VerifyIsValid()
		{

		}

		private void ClearExportFile(object parameter)
		{
			OutputFolder = string.Empty;
		}

		private void BrowseFolder(object parameter)
		{
			var folderPath = _dialogService.ShowFolderDialog(PluginResources.FolderDialog_Title, GetValidFolderPath());
			if (string.IsNullOrEmpty(folderPath) && Directory.Exists(folderPath))
			{
				OutputFolder = folderPath;
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

		private string GetValidFolderPath()
		{
			if (string.IsNullOrWhiteSpace(OutputFolder))
			{
				return string.Empty;
			}

			var outputFolder = OutputFolder;
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

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
