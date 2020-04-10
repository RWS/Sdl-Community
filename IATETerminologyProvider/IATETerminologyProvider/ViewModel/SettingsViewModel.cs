using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using IATETerminologyProvider.Commands;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Model;
using IATETerminologyProvider.Model.ResponseModels;
using IATETerminologyProvider.Service;

namespace IATETerminologyProvider.ViewModel
{
	public class SettingsViewModel : ViewModelBase
	{
		private ICommand _saveSettingsCommand;
		private DomainModel _selectedDomain;		
		private TermTypeModel _selectedTermType;
		private readonly DomainService _domainService;
		private readonly TermTypeService _termTypeService;
		private ObservableCollection<DomainModel> _domains;
		private ObservableCollection<TermTypeModel> _termTypes;
		public delegate ProviderSettings SaveSettingsEventRaiser();
		public event SaveSettingsEventRaiser OnSaveSettingsCommandRaised;

		public SettingsViewModel(ProviderSettings providerSettings)
		{			
			_domains = new ObservableCollection<DomainModel>();
			_termTypes = new ObservableCollection<TermTypeModel>();
			_termTypeService = new TermTypeService();
			_domainService = new DomainService();
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

		public ProviderSettings ProviderSettings { get; set; }

		public ICommand SaveSettingsCommand => _saveSettingsCommand ?? (_saveSettingsCommand = new CommandHandler(SaveSettingsAction, true));


		private void SaveSettingsAction()
		{
			if (Domains.Count > 0)
			{
				//TODO: Move initialization in constuctor
				ProviderSettings = new ProviderSettings
				{
					Domains = new List<string>(),
					TermTypes = new List<int>()
				};

				// Add selected domains to provider settings
				var selectedDomains = Domains?.Where(d => d.IsSelected).ToList();
				foreach (var selectedDomain in selectedDomains)
				{
					ProviderSettings.Domains.Add(selectedDomain.Code);
				}

				// Add selected term types to provider settings
				var selectedTermTypes = TermTypes?.Where(d => d.IsSelected).ToList();
				if (selectedTermTypes != null)
				{
					foreach (var selectedTermType in selectedTermTypes)
					{
						ProviderSettings.TermTypes.Add(selectedTermType.Code);
					}
				}

				var persistenceService = new PersistenceService();
				persistenceService.AddSettings(ProviderSettings);

				OnSaveSettingsCommandRaised?.Invoke();
			}
		}

		private void LoadDomains()
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
						Name = selectedDomainName
					};
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
				TermTypes.Add(termType);
			}
		}

		// Set UI Settings fields selection based on the provider settings file (for domains and term types).
		private void SetFieldsSelection(ProviderSettings providerSettings)
		{
			if (providerSettings is null) return;

			foreach (var domainCode in providerSettings.Domains)
			{
				var domain = Domains?.FirstOrDefault(d => d.Code.Equals(domainCode));
				if (domain != null)
				{
					domain.IsSelected = true;
				}
			}

			foreach (var termTypeCode in providerSettings.TermTypes)
			{
				var termType = TermTypes?.FirstOrDefault(t => t.Code.Equals(termTypeCode));
				if (termType != null)
				{
					termType.IsSelected = true;
				}
			}

		}
	}
}