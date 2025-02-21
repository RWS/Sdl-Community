using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using Rws.MultiSelectComboBox.EventArgs;
using Trados.Transcreate.Commands;
using Trados.Transcreate.Common;
using Trados.Transcreate.Model;

namespace Trados.Transcreate.ViewModel
{
	public class SettingsViewModel : BaseModel, IDisposable
	{		
		private Window _window;
		private readonly Settings _settings;
		private readonly PathInfo _pathInfo;
		private string _windowTitle;
		private ICommand _saveCommand;
		private ICommand _resetCommand;
		private ICommand _clearFiltersCommand;
		private ICommand _selectedItemsChangedCommand;
		private int _selectedPage;

		// Convert options
		private int _maxAlternativeTranslations;

		// Export options
		private List<FilterItem> _excludeFilterItems;
		private bool _exportCopySourceToTarget;
		private bool _exportCopySourceToTargetEnabled;
		private bool _exportIncludeTranslations;
		private bool _exportIncludeBackTranslations;
		private ObservableCollection<FilterItem> _exportSelectedExcludeFilterItems;

		// Import options
		private List<FilterItem> _importFilterItems;
		private List<ConfirmationStatus> _confirmationStatuses;
		private ConfirmationStatus _importStatusTranslationUpdated;
		private ConfirmationStatus _importStatusTranslationNotUpdated;
		private ConfirmationStatus _importStatusSegmentNotImported;
		private string _importOriginSystem;
		private ObservableCollection<FilterItem> _importSelectedExcludeFilterItems;

		public SettingsViewModel(Window window, Settings settings, PathInfo pathInfo)
		{
			SetWindowReference(window);
			_settings = settings;
			_pathInfo = pathInfo;

			WindowTitle = "Settings";

			LoadSettings();		
		}
		
		public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new CommandHandler(SaveChanges));

		public ICommand ResetCommand => _resetCommand ?? (_resetCommand = new CommandHandler(Reset));

		public ICommand ClearFiltersCommand => _clearFiltersCommand ?? (_clearFiltersCommand = new CommandHandler(ClearFilters));

		public ICommand SelectedItemsChangedCommand => _selectedItemsChangedCommand ?? (_selectedItemsChangedCommand = new CommandHandler(SelectedItemsChanged));

		public string WindowTitle
		{
			get => _windowTitle;
			set
			{
				_windowTitle = value;
				OnPropertyChanged(nameof(WindowTitle));
			}
		}

		public int SelectedPage
		{
			get => _selectedPage;
			set
			{
				if (_selectedPage == value)
				{
					return;
				}

				_selectedPage = value;
				OnPropertyChanged(nameof(SelectedPage));
			}
		}

		#region  |  Convert Options Tab  |

		public int MaxAlternativeTranslations
		{
			get => _maxAlternativeTranslations;
			set
			{
				if (_maxAlternativeTranslations == value)
				{
					return;
				}

				_maxAlternativeTranslations = value;
				OnPropertyChanged(nameof(MaxAlternativeTranslations));
			}
		}
		

		#endregion


		#region  |  Export Options Tab  |


		public List<FilterItem> ExcludeFilterItems
		{
			get => _excludeFilterItems;
			set
			{
				if (_excludeFilterItems == value)
				{
					return;
				}

				_excludeFilterItems = value;
				OnPropertyChanged(nameof(ExcludeFilterItems));
			}
		}

		public ObservableCollection<FilterItem> ExportSelectedExcludeFilterItems
		{
			get => _exportSelectedExcludeFilterItems ?? (_exportSelectedExcludeFilterItems = new ObservableCollection<FilterItem>());
			set
			{
				if (_exportSelectedExcludeFilterItems == value)
				{
					return;
				}

				_exportSelectedExcludeFilterItems = value;
				OnPropertyChanged(nameof(ExportSelectedExcludeFilterItems));
			}
		}

		public bool ExportCopySourceToTarget
		{
			get => _exportCopySourceToTarget;
			set
			{
				if (_exportCopySourceToTarget == value)
				{
					return;
				}

				_exportCopySourceToTarget = value;
				OnPropertyChanged(nameof(ExportCopySourceToTarget));
			}
		}

		public bool ExportIncludeBackTranslations
		{
			get => _exportIncludeBackTranslations;
			set
			{
				if (_exportIncludeBackTranslations == value)
				{
					return;
				}

				_exportIncludeBackTranslations = value;
				OnPropertyChanged(nameof(ExportIncludeBackTranslations));
			}
		}

		public bool ExportCopySourceToTargetEnabled
		{
			get => _exportCopySourceToTargetEnabled;
			set
			{
				if (_exportCopySourceToTargetEnabled == value)
				{
					return;
				}

				_exportCopySourceToTargetEnabled = value;
				OnPropertyChanged(nameof(ExportCopySourceToTargetEnabled));
			}
		}

		public bool ExportIncludeTranslations
		{
			get => _exportIncludeTranslations;
			set
			{
				if (_exportIncludeTranslations == value)
				{
					return;
				}

				_exportIncludeTranslations = value;
				OnPropertyChanged(nameof(ExportIncludeTranslations));

				if (!_exportIncludeTranslations)
				{
					ExportCopySourceToTarget = false;
					ExportCopySourceToTargetEnabled = false;
				}
				else
				{
					ExportCopySourceToTargetEnabled = true;
				}

				//VerifyIsValid();
			}
		}

		#endregion

		#region  |  Import Options Tab  |

		public List<ConfirmationStatus> ConfirmationStatuses
		{
			get => _confirmationStatuses;
			set
			{
				_confirmationStatuses = value;
				OnPropertyChanged(nameof(ConfirmationStatuses));
			}
		}

		public List<FilterItem> ImportFilterItems
		{
			get => _importFilterItems;
			set
			{
				if (_importFilterItems == value)
				{
					return;
				}

				_importFilterItems = value;
				OnPropertyChanged(nameof(ImportFilterItems));
			}
		}

		public bool ImportOverwriteTranslations { get; set; }

		public string ImportOriginSystem
		{
			get => _importOriginSystem;
			set
			{
				if (value == _importOriginSystem)
				{
					return;
				}

				_importOriginSystem = value;
				OnPropertyChanged(nameof(ImportOriginSystem));
				//VerifyIsValid();
			}
		}


		public ObservableCollection<FilterItem> ImportSelectedExcludeFilterItems
		{
			get => _importSelectedExcludeFilterItems ?? (_importSelectedExcludeFilterItems = new ObservableCollection<FilterItem>());
			set
			{
				if (_importSelectedExcludeFilterItems == value)
				{
					return;
				}

				_importSelectedExcludeFilterItems = value;
				OnPropertyChanged(nameof(ImportSelectedExcludeFilterItems));
			}
		}

		public ConfirmationStatus ImportStatusTranslationUpdated
		{
			get => _importStatusTranslationUpdated;
			set
			{
				if (value == _importStatusTranslationUpdated)
				{
					return;
				}

				_importStatusTranslationUpdated = value;
				OnPropertyChanged(nameof(ImportStatusTranslationUpdated));
			}
		}

		public ConfirmationStatus ImportStatusTranslationNotUpdated
		{
			get => _importStatusTranslationNotUpdated;
			set
			{
				if (value == _importStatusTranslationNotUpdated)
				{
					return;
				}

				_importStatusTranslationNotUpdated = value;
				OnPropertyChanged(nameof(ImportStatusTranslationNotUpdated));
			}
		}

		public ConfirmationStatus ImportStatusSegmentNotImported
		{
			get => _importStatusSegmentNotImported;
			set
			{
				if (value == _importStatusSegmentNotImported)
				{
					return;
				}

				_importStatusSegmentNotImported = value;
				OnPropertyChanged(nameof(ImportStatusSegmentNotImported));
			}
		}

		#endregion


		private void SaveChanges(object parameter)
		{
			_settings.ConvertOptions.MaxAlternativeTranslations = MaxAlternativeTranslations;

			_settings.ExportOptions.IncludeBackTranslations = ExportIncludeBackTranslations;
			_settings.ExportOptions.IncludeTranslations = ExportIncludeTranslations;
			_settings.ExportOptions.CopySourceToTarget = ExportCopySourceToTarget;
			_settings.ExportOptions.ExcludeFilterIds = ExportSelectedExcludeFilterItems.Select(a => a.Id).ToList();

			_settings.ImportOptions.OverwriteTranslations = ImportOverwriteTranslations;
			_settings.ImportOptions.OriginSystem = ImportOriginSystem;
			_settings.ImportOptions.StatusTranslationUpdatedId = ImportStatusTranslationUpdated.Id;
			_settings.ImportOptions.StatusTranslationNotUpdatedId = ImportStatusTranslationNotUpdated.Id;
			_settings.ImportOptions.StatusSegmentNotImportedId = ImportStatusSegmentNotImported.Id;
			_settings.ImportOptions.ExcludeFilterIds = ImportSelectedExcludeFilterItems.Select(a => a.Id).ToList();

			File.WriteAllText(_pathInfo.SettingsFilePath, JsonConvert.SerializeObject(_settings));

			_window?.Close();
		}

		private void Reset(object paramter)
		{
			var settings = new Settings();
			if (SelectedPage == 0)
			{
				LoadConvertSettings(settings.ConvertOptions);
			}

			if (SelectedPage == 1)
			{
				LoadExportSettings(settings.ExportOptions);
			}

			if (SelectedPage == 2)
			{
				LoadImportSettings(settings.ImportOptions);
			}
		}

		private void ClearFilters(object parameter)
		{
			if (SelectedPage == 1)
			{
				ExportSelectedExcludeFilterItems.Clear();
				OnPropertyChanged(nameof(ExportSelectedExcludeFilterItems));
			}

			if (SelectedPage == 2)
			{
				ImportSelectedExcludeFilterItems.Clear();
				OnPropertyChanged(nameof(ImportSelectedExcludeFilterItems));
			}
		}

		private void SelectedItemsChanged(object parameter)
		{
			if (parameter is SelectedItemsChangedEventArgs)
			{				
				if (SelectedPage == 1)
				{
					OnPropertyChanged(nameof(ExportSelectedExcludeFilterItems));
				}

				if (SelectedPage == 2)
				{
					OnPropertyChanged(nameof(ImportSelectedExcludeFilterItems));
				}
			}
		}

		private void LoadSettings()
		{
			LoadConvertSettings(_settings.ConvertOptions);
			LoadImportSettings(_settings.ImportOptions);

			SelectedPage = 1;
			if (_window == null)
			{
				LoadExportSettings(_settings.ExportOptions);
			}
		}

		private void LoadConvertSettings(ConvertOptions options)
		{
			MaxAlternativeTranslations = options.MaxAlternativeTranslations;
		}

		private void LoadExportSettings(ExportOptions exportOptions)
		{
			if (ExcludeFilterItems == null)
			{
				ExcludeFilterItems = new List<FilterItem>(Enumerators.GetFilterItems());
			}

			ExportCopySourceToTarget = exportOptions.CopySourceToTarget;
			ExportIncludeTranslations = exportOptions.IncludeTranslations;
			ExportIncludeBackTranslations = exportOptions.IncludeBackTranslations;
			ExportSelectedExcludeFilterItems = new ObservableCollection<FilterItem>(Enumerators.GetFilterItems(ExcludeFilterItems, exportOptions.ExcludeFilterIds));
		}

		private void LoadImportSettings(ImportOptions importOptions)
		{
			if (ConfirmationStatuses == null)
			{
				ConfirmationStatuses = Enumerators.GetConfirmationStatuses();
			}

			if (ImportFilterItems == null)
			{
				ImportFilterItems = new List<FilterItem>(Enumerators.GetFilterItems());
			}

			ImportOverwriteTranslations = importOptions.OverwriteTranslations;
			ImportOriginSystem = importOptions.OriginSystem;
			ImportStatusTranslationUpdated = Enumerators.GetConfirmationStatus(ConfirmationStatuses, importOptions.StatusTranslationUpdatedId, "Draft");
			ImportStatusTranslationNotUpdated = Enumerators.GetConfirmationStatus(ConfirmationStatuses, importOptions.StatusTranslationNotUpdatedId, string.Empty);
			ImportStatusSegmentNotImported = Enumerators.GetConfirmationStatus(ConfirmationStatuses, importOptions.StatusSegmentNotImportedId, string.Empty);
			ImportSelectedExcludeFilterItems = new ObservableCollection<FilterItem>(Enumerators.GetFilterItems(ImportFilterItems, importOptions.ExcludeFilterIds));
		}

		private void SetWindowReference(Window window)
		{
			if (_window != null)
			{
				_window.Loaded -= Window_Loaded;
			}

			_window = window;


			if (_window != null)
			{
				_window.Loaded += Window_Loaded;
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			LoadExportSettings(_settings.ExportOptions);
		}

		public void Dispose()
		{
			if (_window != null)
			{
				_window.Loaded -= Window_Loaded;
			}
		}
	}
}
