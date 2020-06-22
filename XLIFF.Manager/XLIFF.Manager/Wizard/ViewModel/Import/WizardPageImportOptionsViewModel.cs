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
		private ConfirmationStatus _statusTranslationUpdated;
		private ConfirmationStatus _statusTranslationNotUpdated;
		private ConfirmationStatus _statusSegmentNotImported;
		private string _originSystem;
		private ObservableCollection<FilterItem> _filterItems;
		private ObservableCollection<FilterItem> _selectedExcludeFilterItems;
		private ICommand _clearFiltersCommand;

		public WizardPageImportOptionsViewModel(Window owner, object view, WizardContext wizardContext) : base(owner, view, wizardContext)
		{
			BackupFiles = wizardContext.ImportOptions.BackupFiles;
			OverwriteTranslations = wizardContext.ImportOptions.OverwriteTranslations;
			OriginSystem = wizardContext.ImportOptions.OriginSystem;

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
			if (wizardContext.ImportOptions.StatusTranslationUpdatedId != null)
			{
				StatusTranslationUpdated =
					ConfirmationStatuses.FirstOrDefault(a => a.Id == wizardContext.ImportOptions.StatusTranslationUpdatedId);				
			}

			if (StatusTranslationUpdated == null)
			{
				StatusTranslationUpdated = ConfirmationStatuses.FirstOrDefault(a => a.Id == "Draft");
			}


			if (wizardContext.ImportOptions.StatusTranslationNotUpdatedId != null)
			{
				StatusTranslationNotUpdated =
					ConfirmationStatuses.FirstOrDefault(a => a.Id == wizardContext.ImportOptions.StatusTranslationNotUpdatedId);
			}

			if (StatusTranslationNotUpdated == null)
			{
				StatusTranslationNotUpdated = ConfirmationStatuses.FirstOrDefault(a => a.Id == string.Empty);
			}

			if (wizardContext.ImportOptions.StatusSegmentNotImportedId != null)
			{
				StatusSegmentNotImported =
					ConfirmationStatuses.FirstOrDefault(a => a.Id == wizardContext.ImportOptions.StatusSegmentNotImportedId);
			}

			if (StatusSegmentNotImported == null)
			{
				StatusSegmentNotImported = ConfirmationStatuses.FirstOrDefault(a => a.Id == string.Empty);
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
