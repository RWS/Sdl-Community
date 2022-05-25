using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Rws.MultiSelectComboBox.EventArgs;
using Trados.Transcreate.Commands;
using Trados.Transcreate.Common;
using Trados.Transcreate.Interfaces;
using Trados.Transcreate.Model;

namespace Trados.Transcreate.Wizard.ViewModel.Export
{
	public class WizardPageExportOptionsViewModel : WizardPageViewModelBase, IDisposable
	{
		private readonly IDialogService _dialogService;
		private string _outputFolder;
		private bool _copySourceToTarget;
		private bool _copySourceToTargetEnabled;
		private bool _includeTranslations;
		private bool _includeBackTranslations;
		private List<FilterItem> _filterItems;
		private ObservableCollection<FilterItem> _selectedExcludeFilterItems;
		private ICommand _clearExportFileCommand;
		private ICommand _browseFolderCommand;
		private ICommand _clearFiltersCommand;
		private ICommand _selectedItemsChangedCommand;	

		public WizardPageExportOptionsViewModel(Window owner, object view, TaskContext taskContext, IDialogService dialogService) 
			: base(owner, view, taskContext)
		{
			_dialogService = dialogService;
			
			OutputFolder = TaskContext.WorkflowFolder;
			CopySourceToTarget = taskContext.ExportOptions.CopySourceToTarget;
			IncludeTranslations = taskContext.ExportOptions.IncludeTranslations;
			
			if (!(taskContext.Project is BackTranslationProject))
			{
				ShowIncludeBackTranslations = true;
				IncludeBackTranslations = taskContext.ExportOptions.IncludeBackTranslations;
			}
			else
			{
				IncludeBackTranslations = false;
				ShowIncludeBackTranslations = false;
			}

			FilterItems = new List<FilterItem>(Enumerators.GetFilterItems());
			SelectedExcludeFilterItems = new ObservableCollection<FilterItem>(Enumerators.GetFilterItems(FilterItems, TaskContext.ExportOptions.ExcludeFilterIds));

			LoadPage += OnLoadPage;
			LeavePage += OnLeavePage;
		}
	
		public ICommand ClearExportFileCommand => _clearExportFileCommand ?? (_clearExportFileCommand = new CommandHandler(ClearExportFile));

		public ICommand BrowseFolderCommand => _browseFolderCommand ?? (_browseFolderCommand = new CommandHandler(BrowseFolder));

		public ICommand ClearFiltersCommand => _clearFiltersCommand ?? (_clearFiltersCommand = new CommandHandler(ClearFilters));

		public ICommand SelectedItemsChangedCommand => _selectedItemsChangedCommand ?? (_selectedItemsChangedCommand = new CommandHandler(SelectedItemsChanged));

		public string ExcludeSegmentsToolTip => PluginResources.ToolTip_Label_ExcludeSegments_Export;
		
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

		public bool IncludeBackTranslations
		{
			get => _includeBackTranslations;
			set
			{
				if (_includeBackTranslations == value)
				{
					return;
				}

				_includeBackTranslations = value;
				OnPropertyChanged(nameof(IncludeBackTranslations));
			}
		}

		public bool ShowIncludeBackTranslations { get; set; }

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
			TaskContext.WorkflowFolder = OutputFolder;
			TaskContext.ExportOptions.CopySourceToTarget = CopySourceToTarget;
			TaskContext.ExportOptions.IncludeTranslations = IncludeTranslations;
			TaskContext.ExportOptions.IncludeBackTranslations = IncludeBackTranslations;
			TaskContext.ExportOptions.ExcludeFilterIds = SelectedExcludeFilterItems.Select(a => a.Id).ToList();
		}

		public void Dispose()
		{
			LoadPage -= OnLoadPage;
			LeavePage -= OnLeavePage;
		}
	}
}
