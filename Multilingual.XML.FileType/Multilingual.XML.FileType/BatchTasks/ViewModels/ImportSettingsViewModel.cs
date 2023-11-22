using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Multilingual.XML.FileType.BatchTasks.Settings;
using Multilingual.XML.FileType.Commands;
using Multilingual.XML.FileType.Common;
using Multilingual.XML.FileType.Models;

namespace Multilingual.XML.FileType.BatchTasks.ViewModels
{
	public class ImportSettingsViewModel : BaseModel
	{
		private bool _overwriteExistingTranslations;
		private List<ConfirmationStatus> _confirmationStatuses;
		private ConfirmationStatus _statusTranslationUpdated;
		private string _originSystem;
		private List<FilterItem> _filterItems;
		private ObservableCollection<FilterItem> _selectedExcludeFilterItems;
		private ICommand _clearFiltersCommand;

		public ImportSettingsViewModel(MultilingualXmlImportSettings settings)
		{
			Settings = settings;

			BackupFiles = true;
			OverwriteTranslations = Settings.OverwriteTranslations;
			OriginSystem = settings.OriginSystem;

			ConfirmationStatuses = Enumerators.GetConfirmationStatuses();
			StatusTranslationUpdated = Enumerators.GetConfirmationStatus(ConfirmationStatuses, Settings.StatusTranslationUpdatedId, "Draft");

			FilterItems = new List<FilterItem>(Enumerators.GetFilterItems());
			SelectedExcludeFilterItems = new ObservableCollection<FilterItem>(Enumerators.GetFilterItems(FilterItems, Settings.ExcludeFilterIds));
		}

		public MultilingualXmlImportSettings Settings { get; }


		public ICommand ClearFiltersCommand => _clearFiltersCommand ?? (_clearFiltersCommand = new CommandHandler(ClearFilters));

		public bool BackupFiles { get; set; }

		public bool OverwriteTranslations
		{
			get => _overwriteExistingTranslations;
			set
			{
				if (_overwriteExistingTranslations == value)
				{
					return;
				}

				_overwriteExistingTranslations = value;

				OnPropertyChanged(nameof(OverwriteTranslations));
			}
		}

		public string OriginSystem
		{
			get => _originSystem;
			set
			{
				if (value == _originSystem)
				{
					return;
				}

				_originSystem = value;
				OnPropertyChanged(nameof(OriginSystem));
			}
		}

		public string ExcludeSegmentsToolTip => "Exclude segments from being updated that match the properties selected.";

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

		public List<ConfirmationStatus> ConfirmationStatuses
		{
			get => _confirmationStatuses;
			set
			{
				_confirmationStatuses = value;
				OnPropertyChanged(nameof(ConfirmationStatuses));
			}
		}

		public ConfirmationStatus StatusTranslationUpdated
		{
			get => _statusTranslationUpdated;
			set
			{
				if (value == _statusTranslationUpdated)
				{
					return;
				}

				_statusTranslationUpdated = value;
				OnPropertyChanged(nameof(StatusTranslationUpdated));
			}
		}

		public MultilingualXmlImportSettings SaveSettings()
		{
			Settings.OverwriteTranslations = OverwriteTranslations;
			Settings.OriginSystem = OriginSystem;

			Settings.StatusTranslationUpdatedId = StatusTranslationUpdated.Id;
			Settings.ExcludeFilterIds = SelectedExcludeFilterItems.Select(a => a.Id).ToList();

			return Settings;
		}

		public MultilingualXmlImportSettings ResetToDefaults()
		{
			Settings.ResetToDefaults();

			OverwriteTranslations = Settings.OverwriteTranslations;
			OriginSystem = Settings.OriginSystem;
			StatusTranslationUpdated = Enumerators.GetConfirmationStatus(ConfirmationStatuses, Settings.StatusTranslationUpdatedId, "Draft");
			SelectedExcludeFilterItems = new ObservableCollection<FilterItem>(Enumerators.GetFilterItems(FilterItems, Settings.ExcludeFilterIds));

			return Settings;
		}

		private void ClearFilters(object parameter)
		{
			SelectedExcludeFilterItems.Clear();
			OnPropertyChanged(nameof(SelectedExcludeFilterItems));
		}
	}
}
