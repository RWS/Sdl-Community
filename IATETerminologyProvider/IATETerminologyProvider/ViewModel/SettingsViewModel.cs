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
	public class SettingsViewModel : ViewModelBase
	{
		private ICommand _saveSettingsCommand;
		private ICommand _resetToDefault;
		private ICommand _clearCache;
		private DomainModel _selectedDomain;		
		private TermTypeModel _selectedTermType;
		private readonly DomainService _domainService;
		private readonly TermTypeService _termTypeService;
		private readonly IMessageBoxService _messageBoxService;
		private ObservableCollection<DomainModel> _domains;
		private ObservableCollection<TermTypeModel> _termTypes;
		private bool _dialogResult;
		private bool _searchInSubdomains;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public SettingsViewModel(SettingsModel providerSettings, IMessageBoxService messageBocBoxService)
		{
			_domains = new ObservableCollection<DomainModel>();
			_termTypes = new ObservableCollection<TermTypeModel>();
			_termTypeService = new TermTypeService();
			_domainService = new DomainService();
			_messageBoxService = messageBocBoxService;
			ProviderSettings = new SettingsModel
			{
				Domains = new List<DomainModel>(),
				TermTypes = new List<TermTypeModel>()
			};
			LoadDomains();
			LoadTermTypes();
			SetFieldsSelection(providerSettings);
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

		public ObservableCollection<TermTypeModel> TermTypes
		{
			get => _termTypes;
			set
			{
				_termTypes = value;
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
				if (_dialogResult == value) return;
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
				if (_searchInSubdomains == value) return;
				_searchInSubdomains = value;
				OnPropertyChanged(nameof(SearchInSubdomains));
			}
		}

		public ObservableCollection<DomainModel> Domains
		{
			get => _domains;
			set
			{
				_domains = value;
				OnPropertyChanged(nameof(Domains));
			}
		}
		public NotifyTaskCompletion<ObservableCollection<ItemsResponseModel>> IateDomains { get; set; }
		public NotifyTaskCompletion<ObservableCollection<ItemsResponseModel>> IateTermTypes { get; set; }

		public SettingsModel ProviderSettings { get; set; }

		public ICommand SaveSettingsCommand => _saveSettingsCommand ?? (_saveSettingsCommand = new CommandHandler(SaveSettingsAction, true));
		public ICommand ResetToDefault => _resetToDefault ?? (_resetToDefault = new CommandHandler(Reset, true));
		public ICommand ClearCache => _clearCache ?? (_clearCache = new CommandHandler(Clear, true));

		private void Clear()
		{
			var result = _messageBoxService.ShowYesNoMessageBox("", PluginResources.ClearConfirmation);
			if (result != MessageDialogResult.Yes) return;
			var activeProjectName = Utils.GetCurrentProjectName();
			if (string.IsNullOrEmpty(activeProjectName)) return;

			var cacheService = new CacheService(activeProjectName);
			Task.Run(async () => await cacheService.ClearCachedResults());
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
				ProviderSettings.Domains = Domains.ToList();
			}

			if (TermTypes.Count > 0)
			{
				ProviderSettings.TermTypes = TermTypes.ToList();
			}
			ProviderSettings.SearchInSubdomains = SearchInSubdomains;

			DialogResult = true;
			UnSubscribeToEvents();
		}

		private void LoadDomains()
		{
			try
			{
				if (DomainService.Domains?.Count > 0)
				{
					SetDomains(DomainService.Domains);
				}
				else
				{
					IateDomains = new NotifyTaskCompletion<ObservableCollection<ItemsResponseModel>>(_domainService.GetDomains());
					IateDomains.PropertyChanged += IateDomains_PropertyChanged;
				}
			}
			catch (InvalidAsynchronousStateException e)
			{
				_logger.Error(e);
			}
		}

		private void LoadTermTypes()
		{
			if (TermTypeService.IateTermType?.Count > 0)
			{
				SetTermTypes(TermTypeService.IateTermType);
			}
			else
			{
				IateTermTypes = new NotifyTaskCompletion<ObservableCollection<ItemsResponseModel>>(_termTypeService.GetTermTypes());
				IateTermTypes.PropertyChanged += IateTermTypes_PropertyChanged;
			}
		}

		private void IateTermTypes_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!e.PropertyName.Equals("Result")) return;
			if (!(IateTermTypes.Result?.Count > 0)) return;
			SetTermTypes(IateTermTypes.Result);
		}

		private void IateDomains_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!e.PropertyName.Equals("Result")) return;
			if (!(IateDomains.Result?.Count > 0)) return;
			SetDomains(IateDomains.Result);
		}

		private void SetDomains(ObservableCollection<ItemsResponseModel>iateDomains)
		{
			foreach (var domain in iateDomains)
			{
				if (!domain.Name.Equals(Constants.NotSpecifiedCode))
				{
					var selectedDomainName = Utils.UppercaseFirstLetter(domain.Name.ToLower());
					var domainModel = new DomainModel
					{
						Code = domain.Code,
						Name = selectedDomainName,
					};
					domainModel.PropertyChanged += DomainModel_PropertyChanged;
					Domains.Add(domainModel);
				}
			}
		}

		private void SetTermTypes(ObservableCollection<ItemsResponseModel> termTypesResponse)
		{
			foreach (var item in termTypesResponse)
			{
				var selectedTermTypeName = Utils.UppercaseFirstLetter(item.Name.ToLower());

				var termType = new TermTypeModel
				{
					Code = int.TryParse(item.Code, out _) ? int.Parse(item.Code) : 0,
					Name = selectedTermTypeName
				};
				termType.PropertyChanged += TermType_PropertyChanged;
				TermTypes.Add(termType);
			}
		}

		// Set UI Settings fields selection based on the project settings (for domains and term types).
		private void SetFieldsSelection(SettingsModel providerSettings)
		{
			if (providerSettings is null) return;

			if (providerSettings.Domains?.Count> 0)
			{
				Domains = new ObservableCollection<DomainModel>(providerSettings.Domains);
			}
			if (providerSettings.TermTypes?.Count > 0)
			{
				TermTypes = new ObservableCollection<TermTypeModel>(providerSettings.TermTypes);
			}
			SearchInSubdomains = providerSettings.SearchInSubdomains;
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
		private void TermType_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "IsSelected") return;
			OnPropertyChanged(nameof(AllTermTypesChecked));
		}
		private void DomainModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "IsSelected") return;
			OnPropertyChanged(nameof(AllDomainsChecked));
		}
		private void UnSubscribeToEvents()
		{
			foreach (var domain in Domains)
			{
				domain.PropertyChanged -= DomainModel_PropertyChanged;
			}
			foreach (var termType in TermTypes)
			{
				termType.PropertyChanged -= TermType_PropertyChanged;
			}
			if (IateDomains != null)
			{
				IateDomains.PropertyChanged -= IateDomains_PropertyChanged;
			}
			if (IateTermTypes != null)
			{
				IateTermTypes.PropertyChanged -= IateTermTypes_PropertyChanged;
			}
		}
	}
}