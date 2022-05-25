using System;
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
using Sdl.Community.XLIFF.Manager.Model.ProjectSettings;
using Rws.MultiSelectComboBox.EventArgs;

namespace Sdl.Community.XLIFF.Manager.BatchTasks.ViewModel
{
	public class ExportOptionsViewModel : INotifyPropertyChanged
	{
		private readonly IDialogService _dialogService;
		private List<XLIFFSupportItem> _xliffSupportItems;
		private XLIFFSupportItem _selectedXliffSupportItemModel;
		private string _outputFolder;
		private bool _copySourceToTarget;
		private bool _copySourceToTargetEnabled;
		private bool _includeTranslations;
		private List<FilterItem> _filterItems;
		private ObservableCollection<FilterItem> _selectedExcludeFilterItems;
		private ICommand _clearExportFileCommand;
		private ICommand _browseFolderCommand;
		private ICommand _clearFiltersCommand;
		private ICommand _selectedItemsChangedCommand;

		public ExportOptionsViewModel(XliffManagerExportSettings exportSettings, IDialogService dialogService)
		{
			_dialogService = dialogService;

			if (exportSettings.ExportOptions == null)
			{
				exportSettings.ExportOptions = new ExportOptions();
			}

			XLIFFSupportItems = Enumerators.GetXLIFFSupportItems();

			SelectedXliffSupportItem = XLIFFSupportItems.FirstOrDefault(a => a.SupportType == exportSettings.ExportOptions.XliffSupport);
			OutputFolder = exportSettings.TransactionFolder;
			CopySourceToTarget = exportSettings.ExportOptions.CopySourceToTarget;
			IncludeTranslations = exportSettings.ExportOptions.IncludeTranslations;

			InitializeFilterItems(exportSettings.ExportOptions.ExcludeFilterIds);
		}

		public ICommand ClearExportFileCommand => _clearExportFileCommand ?? (_clearExportFileCommand = new CommandHandler(ClearExportFile));

		public ICommand BrowseFolderCommand => _browseFolderCommand ?? (_browseFolderCommand = new CommandHandler(BrowseFolder));

		public ICommand ClearFiltersCommand => _clearFiltersCommand ?? (_clearFiltersCommand = new CommandHandler(ClearFilters));

		public ICommand SelectedItemsChangedCommand => _selectedItemsChangedCommand ?? (_selectedItemsChangedCommand = new CommandHandler(SelectedItemsChanged));

		public List<XLIFFSupportItem> XLIFFSupportItems
		{
			get => _xliffSupportItems;
			set
			{
				_xliffSupportItems = value;
				OnPropertyChanged(nameof(XLIFFSupportItems));
			}
		}

		public XLIFFSupportItem SelectedXliffSupportItem
		{
			get
			{
				return _selectedXliffSupportItemModel
					   ?? (_selectedXliffSupportItemModel = XLIFFSupportItems.FirstOrDefault(a => a.SupportType == Enumerators.XLIFFSupport.xliff12polyglot));
			}
			set
			{
				if (_selectedXliffSupportItemModel == value)
				{
					return;
				}

				_selectedXliffSupportItemModel = value;
				OnPropertyChanged(nameof(SelectedXliffSupportItem));
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
			}
		}

		private void InitializeFilterItems(IReadOnlyCollection<string> selecteFilterItemIds)
		{
			FilterItems = new List<FilterItem>(Enumerators.GetFilterItems());
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
					if (filterItem != null && !selectedFilterItems.Exists(a => a.Id == id))
					{
						selectedFilterItems.Add(filterItem);
					}
				}

				SelectedExcludeFilterItems = new ObservableCollection<FilterItem>(selectedFilterItems);
			}
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
