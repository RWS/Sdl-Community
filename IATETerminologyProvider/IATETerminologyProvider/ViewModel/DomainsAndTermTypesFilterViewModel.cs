using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using NLog;
using Sdl.Community.IATETerminologyProvider.Commands;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.Community.IATETerminologyProvider.Model.ResponseModels;
using Sdl.Community.IATETerminologyProvider.Service;

namespace Sdl.Community.IATETerminologyProvider.ViewModel
{
	public class DomainsAndTermTypesFilterViewModel : SettingsViewModelBase
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly InventoriesProvider _inventoriesProvider;
		private readonly ICacheProvider _cacheService;
		private readonly IMessageBoxService _messageBoxService;
		
		private ICommand _saveSettingsCommand;
		private ICommand _resetToDefault;
		private ICommand _clearCache;
		private DomainModel _selectedDomain;
		private TermTypeModel _selectedTermType;

		private ObservableCollection<DomainModel> _domains = new ObservableCollection<DomainModel>();
		private ObservableCollection<TermTypeModel> _termTypes= new ObservableCollection<TermTypeModel>();
		private int _maxEntries;
		private bool _dialogResult;
		private bool _searchInSubdomains;
		private bool _isLoading;
		private bool _isEnabled;

		public DomainsAndTermTypesFilterViewModel(InventoriesProvider inventoriesProvider,
			ICacheProvider cacheService, IMessageBoxService messageBocBoxService)
		{
			_inventoriesProvider = inventoriesProvider;
			_cacheService = cacheService;
			_messageBoxService = messageBocBoxService;

			PropertyChanged += DomainsAndTermTypesFilterViewModel_PropertyChanged;
		}

		private async void DomainsAndTermTypesFilterViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Settings))
			{
				await Setup();
			}
		}

		public bool IsLoading
		{
			get => _isLoading;
			set
			{
				if (_isLoading == value)
				{
					return;
				}

				_isLoading = value;
				IsEnabled = !_isLoading;
				
				OnPropertyChanged(nameof(IsLoading));
			}
		}

		public bool IsEnabled
		{
			get => _isEnabled;
			set
			{
				if (_isEnabled == value)
				{
					return;
				}

				_isEnabled = value;
				OnPropertyChanged(nameof(IsEnabled));
			}
		}

		public int MaxEntries
		{
			get => _maxEntries;
			set
			{
				if (_maxEntries == value)
				{
					return;
				}

				_maxEntries = value;
				OnPropertyChanged(nameof(MaxEntries));
			}
		}
		
		public DomainModel SelectedDomain
		{
			get => _selectedDomain;
			set
			{
				_selectedDomain = value;
				OnPropertyChanged(nameof(SelectedDomain));
			}
		}

		public ObservableCollection<DomainModel> Domains
		{
			get => _domains;
			set
			{
				if (_domains.Any())
				{
					foreach (var domain in _domains)
					{
						domain.PropertyChanged -= DomainsOnPropertyChanged;
					}
				}

				_domains = value;

				if (_domains.Any())
				{
					foreach (var domain in _domains)
					{
						domain.PropertyChanged += DomainsOnPropertyChanged;
					}
				}

				OnPropertyChanged(nameof(Domains));
			}
		}

		public ObservableCollection<TermTypeModel> TermTypes
		{
			get => _termTypes;
			set
			{
				if (_termTypes.Any())
				{
					foreach (var termType in _termTypes)
					{
						termType.PropertyChanged -= TermTypesOnPropertyChanged;
					}
				}
				
				_termTypes = value;

				if (_termTypes.Any())
				{
					foreach (var termType in _termTypes)
					{
						termType.PropertyChanged += TermTypesOnPropertyChanged;
					}
				}

				OnPropertyChanged(nameof(TermTypes));
			}
		}

		public TermTypeModel SelectedTermType
		{
			get => _selectedTermType;
			set
			{
				_selectedTermType = value;

				OnPropertyChanged(nameof(SelectedTermType));
			}
		}

		public bool DialogResult
		{
			get => _dialogResult;
			set
			{
				if (_dialogResult == value)
				{
					return;
				}

				_dialogResult = value;

				OnPropertyChanged(nameof(DialogResult));
			}
		}

		public bool AllDomainsChecked
		{
			get => AreAllDomainsSelected();
			set
			{
				if (value)
				{
					SelectAllDomains(true);
				}

				OnPropertyChanged(nameof(AllDomainsChecked));
			}
		}

		public bool AllTermTypesChecked
		{
			get => AreAllTypesSelected();
			set
			{
				if (value)
				{
					SelectAllTermTypes(true);
				}

				OnPropertyChanged(nameof(AllTermTypesChecked));
			}
		}

		public bool SearchInSubdomains
		{
			get => _searchInSubdomains;
			set
			{
				if (_searchInSubdomains == value)
				{
					return;
				}

				_searchInSubdomains = value;

				OnPropertyChanged(nameof(SearchInSubdomains));
			}
		}

		public ICommand SaveSettingsCommand => _saveSettingsCommand ?? (_saveSettingsCommand = new CommandHandler(SaveSettingsAction, true));

		public ICommand ResetToDefault => _resetToDefault ?? (_resetToDefault = new CommandHandler(Reset, true));

		public ICommand ClearCache => _clearCache ?? (_clearCache = new CommandHandler(Clear, true));

		private async Task Setup()
		{
			if (!_inventoriesProvider.IsInitialized)
			{
				try
				{
					IsLoading = true;
					await _inventoriesProvider.Initialize();
				}
				finally
				{
					IsLoading = false;
				}
			}

			LoadDomains();
			LoadTermTypes();

			SetFieldsSelection();

			IsEnabled = true;
		}

		private void Clear()
		{
			var result = _messageBoxService.ShowYesNoMessageBox("", PluginResources.ClearConfirmation);
			if (result != MessageDialogResult.Yes)
			{
				return;
			}

			_cacheService?.ClearCachedResults();
		}

		private void Reset()
		{
			ResetDomains();
			ResetTypes();
			
			SearchInSubdomains = false;
		}

		private void ResetTypes()
		{
			foreach (var type in TermTypes)
			{
				type.IsSelected = false;
			}
			
			OnPropertyChanged(nameof(AllTermTypesChecked));
		}

		private void ResetDomains()
		{
			foreach (var domain in Domains)
			{
				domain.IsSelected = false;
			}

			OnPropertyChanged(nameof(AllDomainsChecked));
		}

		private void SaveSettingsAction()
		{
			if (Domains.Count > 0)
			{
				Settings.Domains = Domains.ToList();
			}

			if (TermTypes.Count > 0)
			{
				Settings.TermTypes = TermTypes.ToList();
			}

			Settings.SearchInSubdomains = SearchInSubdomains;

			DialogResult = true;

			UnSubscribeToEvents();
		}

		private void LoadDomains()
		{
			try
			{
				if (_inventoriesProvider.Domains?.Count > 0)
				{
					SetDomains(_inventoriesProvider.Domains);
				}
			}
			catch (InvalidAsynchronousStateException e)
			{
				_logger.Error(e);
			}
		}

		private void LoadTermTypes()
		{
			if (_inventoriesProvider.TermTypes?.Count > 0)
			{
				SetTermTypes(_inventoriesProvider.TermTypes);
			}
		}

		private void SetDomains(List<ItemsResponseModel> iateDomains)
		{
			var domains = new List<DomainModel>();
			foreach (var domain in iateDomains)
			{
				if (domain.Name.Equals(Constants.NotSpecifiedCode)) domain.EurovocCode = "00";

				var selectedDomainName = Utils.UppercaseFirstLetter(domain.Name.ToLower());

				var discriminator = "";
				if (!string.IsNullOrWhiteSpace(domain.CjeuCode)) discriminator = "CJEU";

				var domainModel = new DomainModel
				{
					Code = domain.Code,
					Name = $"{domain.EurovocCode ?? domain.CjeuCode} {selectedDomainName} {discriminator}"
				};

				domains.Add(domainModel);
			}

			Domains = new ObservableCollection<DomainModel>(domains);
		}

		private void SetTermTypes(List<ItemsResponseModel> termTypesResponse)
		{
			var termTypes = new List<TermTypeModel>();
			foreach (var item in termTypesResponse)
			{
				var selectedTermTypeName = Utils.UppercaseFirstLetter(item.Name.ToLower());

				var termType = new TermTypeModel
				{
					Code = int.TryParse(item.Code, out _) ? int.Parse(item.Code) : 0,
					Name = selectedTermTypeName
				};

				termTypes.Add(termType);
			}

			TermTypes = new ObservableCollection<TermTypeModel>(termTypes);
		}

		private void SetFieldsSelection()
		{
			if (Settings is null)
			{
				return;
			}

			if (Settings.Domains?.Count > 0)
			{
				Domains = new ObservableCollection<DomainModel>(Settings.Domains);
			}

			if (Settings.TermTypes?.Count > 0)
			{
				TermTypes = new ObservableCollection<TermTypeModel>(Settings.TermTypes);
			}

			SearchInSubdomains = Settings.SearchInSubdomains;
		}

		private bool AreAllDomainsSelected()
		{
			return Domains.Count > 0 && Domains.All(d => d.IsSelected);
		}
		
		private bool AreAllTypesSelected()
		{
			return TermTypes.Count > 0 && TermTypes.All(t => t.IsSelected);
		}

		private void SelectAllDomains(bool select)
		{
			foreach (var domain in Domains)
			{
				domain.IsSelected = select;
			}
		}

		private void SelectAllTermTypes(bool select)
		{
			foreach (var termType in TermTypes)
			{
				termType.IsSelected = select;
			}
		}

		private void DomainsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "IsSelected")
			{
				return;
			}

			OnPropertyChanged(nameof(AllDomainsChecked));
		}

		private void TermTypesOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "IsSelected")
			{
				return;
			}

			OnPropertyChanged(nameof(AllTermTypesChecked));
		}

		private void UnSubscribeToEvents()
		{
			foreach (var domain in Domains)
			{
				domain.PropertyChanged -= DomainsOnPropertyChanged;
			}

			foreach (var termType in TermTypes)
			{
				termType.PropertyChanged -= TermTypesOnPropertyChanged;
			}
		}
	}
}