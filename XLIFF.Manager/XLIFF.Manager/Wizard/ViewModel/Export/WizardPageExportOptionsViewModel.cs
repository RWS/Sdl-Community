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
using Sdl.Core.Globalization;
using Sdl.MultiSelectComboBox.API;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Export
{
	public class WizardPageExportOptionsViewModel : WizardPageViewModelBase, IDisposable
	{
		private readonly IDialogService _dialogService;
		private List<XLIFFSupport> _xliffSupport;
		private XLIFFSupport _selectedXliffSupportModel;
		private string _outputFolder;
		private bool _copySourceToTarget;
		private bool _copySourceToTargetEnabled;
		private bool _includeTranslations;
		private ObservableCollection<FilterItem> _filterItems;
		private ObservableCollection<FilterItem> _selectedExcludeFilterItems;
		private ICommand _clearExportFileCommand;
		private ICommand _browseFolderCommand;

		public WizardPageExportOptionsViewModel(Window owner, object view, WizardContext wizardContext, IDialogService dialogService) 
			: base(owner, view, wizardContext)
		{
			_dialogService = dialogService;
			SelectedXliffSupport = XLIFFSupportList.FirstOrDefault(a => a.SupportType == WizardContext.ExportSupport);
			OutputFolder = WizardContext.TransactionFolder;
			CopySourceToTarget = wizardContext.ExportCopySourceToTarget;
			IncludeTranslations = wizardContext.ExportIncludeTranslations;

			InitializeFilterItems();
			SelectedExcludeFilterItems = new ObservableCollection<FilterItem>(wizardContext.ExcludeFilterItems);

			LoadPage += OnLoadPage;
			LeavePage += OnLeavePage;
		}
	
		public ICommand ClearExportFileCommand => _clearExportFileCommand ?? (_clearExportFileCommand = new CommandHandler(ClearExportFile));

		public ICommand BrowseFolderCommand => _browseFolderCommand ?? (_browseFolderCommand = new CommandHandler(BrowseFolder));

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

		public override string DisplayName => PluginResources.PageName_Options;

		public override bool IsValid { get; set; }

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
			WizardContext.ExportSupport = SelectedXliffSupport.SupportType;
			WizardContext.ExportCopySourceToTarget = CopySourceToTarget;
			WizardContext.ExportIncludeTranslations = IncludeTranslations;
			WizardContext.ExcludeFilterItems = SelectedExcludeFilterItems.ToList();
		}

		public void Dispose()
		{
			LoadPage -= OnLoadPage;
			LeavePage -= OnLeavePage;
		}
	}
}
