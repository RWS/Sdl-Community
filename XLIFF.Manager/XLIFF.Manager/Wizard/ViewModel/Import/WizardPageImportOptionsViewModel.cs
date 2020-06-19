using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Core.Globalization;
using Sdl.MultiSelectComboBox.API;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Import
{
	public class WizardPageImportOptionsViewModel : WizardPageViewModelBase, IDisposable
	{
		private List<ConfirmationStatus> _confirmationStatuses;
		private ConfirmationStatus _confirmationStatusTranslationUpdated;
		private ConfirmationStatus _confirmationStatusTranslationNotUpdated;
		private ConfirmationStatus _confirmationStatusNotImported;
		private string _originSystem;
		private ObservableCollection<FilterItem> _filterItems;
		private ObservableCollection<FilterItem> _selectedExcludeFilterItems;
		private ICommand _clearFiltersCommand;

		public WizardPageImportOptionsViewModel(Window owner, object view, WizardContext wizardContext) : base(owner, view, wizardContext)
		{
			BackupFiles = wizardContext.ImportBackupFiles;
			OverwriteTranslations = wizardContext.ImportOverwriteTranslations;
			OriginSystem = wizardContext.ImportOriginSystem;

			// TODO initialize this within the context
			ConfirmationStatuses = Enumerators.GetConfirmationStatuses();
			AssignConfirmationStatuses(wizardContext);

			InitializeFilterItems();
			SelectedExcludeFilterItems = new ObservableCollection<FilterItem>(wizardContext.ExcludeFilterItems);

			VerifyIsValid();

			LoadPage += OnLoadPage;
			LeavePage += OnLeavePage;
		}

		public ICommand ClearFiltersCommand => _clearFiltersCommand ?? (_clearFiltersCommand = new CommandHandler(ClearFilters));


		private void AssignConfirmationStatuses(WizardContext wizardContext)
		{
			if (wizardContext.ImportConfirmationStatusTranslationUpdatedId != null)
			{
				ConfirmationStatusTranslationUpdated =
					ConfirmationStatuses.FirstOrDefault(a => a.Id == wizardContext.ImportConfirmationStatusTranslationUpdatedId);				
			}

			if (ConfirmationStatusTranslationUpdated == null)
			{
				ConfirmationStatusTranslationUpdated = ConfirmationStatuses.FirstOrDefault(a => a.Id == "Draft");
			}


			if (wizardContext.ImportConfirmationStatusTranslationNotUpdatedId != null)
			{
				ConfirmationStatusTranslationNotUpdated =
					ConfirmationStatuses.FirstOrDefault(a => a.Id == wizardContext.ImportConfirmationStatusTranslationNotUpdatedId);
			}

			if (ConfirmationStatusTranslationNotUpdated == null)
			{
				ConfirmationStatusTranslationNotUpdated = ConfirmationStatuses.FirstOrDefault(a => a.Id == string.Empty);
			}

			if (wizardContext.ImportConfirmationStatusNotImportedId != null)
			{
				ConfirmationStatusNotImported =
					ConfirmationStatuses.FirstOrDefault(a => a.Id == wizardContext.ImportConfirmationStatusNotImportedId);
			}

			if (ConfirmationStatusNotImported == null)
			{
				ConfirmationStatusNotImported = ConfirmationStatuses.FirstOrDefault(a => a.Id == string.Empty);
			}
		}


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

		public ConfirmationStatus ConfirmationStatusTranslationUpdated
		{
			get => _confirmationStatusTranslationUpdated;
			set
			{
				if (value == _confirmationStatusTranslationUpdated)
				{
					return;
				}

				_confirmationStatusTranslationUpdated = value;
				OnPropertyChanged(nameof(ConfirmationStatusTranslationUpdated));
			}
		}

		public ConfirmationStatus ConfirmationStatusTranslationNotUpdated
		{
			get => _confirmationStatusTranslationNotUpdated;
			set
			{
				if (value == _confirmationStatusTranslationNotUpdated)
				{
					return;
				}

				_confirmationStatusTranslationNotUpdated = value;
				OnPropertyChanged(nameof(ConfirmationStatusTranslationNotUpdated));
			}
		}

		public ConfirmationStatus ConfirmationStatusNotImported
		{
			get => _confirmationStatusNotImported;
			set
			{
				if (value == _confirmationStatusNotImported)
				{
					return;
				}

				_confirmationStatusNotImported = value;
				OnPropertyChanged(nameof(ConfirmationStatusNotImported));
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

		private void InitializeFilterItems()
		{
			var filterItems = new List<FilterItem>();

			AddSegmentPropertyFilters(filterItems, new FilterItemGroup(1, "Properties"));
			AddSegmentStatusFilters(filterItems, new FilterItemGroup(2, "Status"));
			AddMatchTypeFilters(filterItems, new FilterItemGroup(3, "Match"));

			FilterItems = new ObservableCollection<FilterItem>(filterItems);
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

		private void OnLoadPage(object sender, EventArgs e)
		{
			OriginSystem = WizardContext.ImportOriginSystem;
			VerifyIsValid();
		}

		private void OnLeavePage(object sender, EventArgs e)
		{
			WizardContext.ImportBackupFiles = BackupFiles;
			WizardContext.ImportOverwriteTranslations = OverwriteTranslations;
			WizardContext.ImportOriginSystem = OriginSystem;

			WizardContext.ImportConfirmationStatusTranslationUpdatedId = ConfirmationStatusTranslationUpdated.Id;
			WizardContext.ImportConfirmationStatusTranslationNotUpdatedId = ConfirmationStatusTranslationNotUpdated.Id;
			WizardContext.ImportConfirmationStatusNotImportedId = ConfirmationStatusNotImported.Id;

			WizardContext.ExcludeFilterItems = SelectedExcludeFilterItems.ToList();
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
