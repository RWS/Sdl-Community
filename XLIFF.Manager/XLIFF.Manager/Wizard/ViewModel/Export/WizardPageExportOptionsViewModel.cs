using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Interfaces;
using Sdl.Community.XLIFF.Manager.Model;
using Rws.MultiSelectComboBox.EventArgs;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Export
{
	public class WizardPageExportOptionsViewModel : WizardPageViewModelBase, IDisposable
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

		public WizardPageExportOptionsViewModel(Window owner, object view, WizardContext wizardContext, IDialogService dialogService)
			: base(owner, view, wizardContext)
		{
			_dialogService = dialogService;
			XLIFFSupportItems = Enumerators.GetXLIFFSupportItems();


			SelectedXliffSupportItem = XLIFFSupportItems.FirstOrDefault(a => a.SupportType == WizardContext.ExportOptions.XliffSupport);
			OutputFolder = WizardContext.TransactionFolder;
			CopySourceToTarget = wizardContext.ExportOptions.CopySourceToTarget;
			IncludeTranslations = wizardContext.ExportOptions.IncludeTranslations;

			FilterItems = new List<FilterItem>(Enumerators.GetFilterItems());
			SelectedExcludeFilterItems = new ObservableCollection<FilterItem>(Enumerators.GetFilterItems(FilterItems, WizardContext.ExportOptions.ExcludeFilterIds));

			LoadPage += OnLoadPage;
			LeavePage += OnLeavePage;
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

		public override string DisplayName => PluginResources.PageName_Options;

		public override bool IsValid { get; set; }

		private void VerifyIsValid()
		{
			IsValid = Directory.Exists(OutputFolder);
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

		private void OnLoadPage(object sender, EventArgs e)
		{
			VerifyIsValid();
		}

		private void OnLeavePage(object sender, EventArgs e)
		{
			WizardContext.TransactionFolder = OutputFolder;
			WizardContext.ExportOptions.XliffSupport = SelectedXliffSupportItem.SupportType;
			WizardContext.ExportOptions.CopySourceToTarget = CopySourceToTarget;
			WizardContext.ExportOptions.IncludeTranslations = IncludeTranslations;
			WizardContext.ExportOptions.ExcludeFilterIds = SelectedExcludeFilterItems.Select(a => a.Id).ToList();
		}

		public void Dispose()
		{
			LoadPage -= OnLoadPage;
			LeavePage -= OnLeavePage;
		}
	}
}
