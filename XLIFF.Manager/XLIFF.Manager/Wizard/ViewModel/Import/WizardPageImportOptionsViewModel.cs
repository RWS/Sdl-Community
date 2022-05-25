using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Import
{
	public class WizardPageImportOptionsViewModel : WizardPageViewModelBase, IDisposable
	{
		private List<ConfirmationStatus> _confirmationStatuses;
		private ConfirmationStatus _statusTranslationUpdated;
		private ConfirmationStatus _statusTranslationNotUpdated;
		private ConfirmationStatus _statusSegmentNotImported;
		private string _originSystem;
		private List<FilterItem> _filterItems;
		private ObservableCollection<FilterItem> _selectedExcludeFilterItems;
		private ICommand _clearFiltersCommand;

		public WizardPageImportOptionsViewModel(Window owner, object view, WizardContext wizardContext) : base(owner, view, wizardContext)
		{
			BackupFiles = wizardContext.ImportOptions.BackupFiles;
				OverwriteTranslations = wizardContext.ImportOptions.OverwriteTranslations;
				OriginSystem = wizardContext.ImportOptions.OriginSystem;

				ConfirmationStatuses = Enumerators.GetConfirmationStatuses();
				StatusTranslationUpdated = Enumerators.GetConfirmationStatus(ConfirmationStatuses, wizardContext.ImportOptions.StatusTranslationUpdatedId, "Draft");
				StatusTranslationNotUpdated = Enumerators.GetConfirmationStatus(ConfirmationStatuses, wizardContext.ImportOptions.StatusTranslationNotUpdatedId, string.Empty);
				StatusSegmentNotImported = Enumerators.GetConfirmationStatus(ConfirmationStatuses, wizardContext.ImportOptions.StatusSegmentNotImportedId, string.Empty);

				FilterItems = new List<FilterItem>(Enumerators.GetFilterItems());
				SelectedExcludeFilterItems = new ObservableCollection<FilterItem>(Enumerators.GetFilterItems(FilterItems, wizardContext.ImportOptions.ExcludeFilterIds));

				VerifyIsValid();

				LoadPage += OnLoadPage;
				LeavePage += OnLeavePage;
		}

		public ICommand ClearFiltersCommand => _clearFiltersCommand ?? (_clearFiltersCommand = new CommandHandler(ClearFilters));

		public override string DisplayName => PluginResources.PageName_Options;

		public override bool IsValid { get; set; }

		private void VerifyIsValid()
		{
			IsValid = !string.IsNullOrEmpty(OriginSystem.Trim());
		}

		public bool BackupFiles { get; set; }

		public bool OverwriteTranslations { get; set; }

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
				VerifyIsValid();
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

		public ConfirmationStatus StatusTranslationNotUpdated
		{
			get => _statusTranslationNotUpdated;
			set
			{
				if (value == _statusTranslationNotUpdated)
				{
					return;
				}

				_statusTranslationNotUpdated = value;
				OnPropertyChanged(nameof(StatusTranslationNotUpdated));
			}
		}

		public ConfirmationStatus StatusSegmentNotImported
		{
			get => _statusSegmentNotImported;
			set
			{
				if (value == _statusSegmentNotImported)
				{
					return;
				}

				_statusSegmentNotImported = value;
				OnPropertyChanged(nameof(StatusSegmentNotImported));
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

		private void OnLoadPage(object sender, EventArgs e)
		{
			OriginSystem = WizardContext.ImportOptions.OriginSystem;
			VerifyIsValid();
		}

		private void OnLeavePage(object sender, EventArgs e)
		{
			WizardContext.ImportOptions.BackupFiles = BackupFiles;
			WizardContext.ImportOptions.OverwriteTranslations = OverwriteTranslations;
			WizardContext.ImportOptions.OriginSystem = OriginSystem;

			WizardContext.ImportOptions.StatusTranslationUpdatedId = StatusTranslationUpdated.Id;
			WizardContext.ImportOptions.StatusTranslationNotUpdatedId = StatusTranslationNotUpdated.Id;
			WizardContext.ImportOptions.StatusSegmentNotImportedId = StatusSegmentNotImported.Id;
			WizardContext.ImportOptions.ExcludeFilterIds = SelectedExcludeFilterItems.Select(a => a.Id).ToList();
		}

		private void ClearFilters(object parameter)
		{
			SelectedExcludeFilterItems.Clear();
			OnPropertyChanged(nameof(SelectedExcludeFilterItems));
		}

		public void Dispose()
		{
			LoadPage -= OnLoadPage;
			LeavePage -= OnLeavePage;
		}
	}
}
