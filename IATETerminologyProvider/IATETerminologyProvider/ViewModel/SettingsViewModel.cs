using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using IATETerminologyProvider.Commands;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Model;
using IATETerminologyProvider.Service;

namespace IATETerminologyProvider.ViewModel
{
	public class SettingsViewModel : ViewModelBase
	{
		private ICommand _saveSettingsCommand;
		private DomainModel _selectedDomain;		
		private TermTypeModel _selectedTermType;
		private ObservableCollection<DomainModel> _domains;
		private ObservableCollection<TermTypeModel> _termTypes;
		public delegate ProviderSettings SaveSettingsEventRaiser();
		public event SaveSettingsEventRaiser OnSaveSettingsCommandRaised;

		public SettingsViewModel(ProviderSettings providerSettings)
		{			
			_domains = new ObservableCollection<DomainModel>();
			_termTypes = new ObservableCollection<TermTypeModel>();
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
		
		public ProviderSettings ProviderSettings { get; set; }

		public ICommand SaveSettingsCommand => _saveSettingsCommand ?? (_saveSettingsCommand = new CommandHandler(SaveSettingsAction, true));


		private void SaveSettingsAction()
		{
			if (Domains?.Count > 0)
			{
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
				foreach (var selectedTermType in selectedTermTypes)
				{
					ProviderSettings.TermTypes.Add(selectedTermType.Code);
				}

				var persistenceService = new PersistenceService();
				persistenceService.AddSettings(ProviderSettings);

				OnSaveSettingsCommandRaised?.Invoke();
			}
		}

		private void LoadDomains()
		{
			var domains = DomainService.GetDomains();
			if (domains?.Count>0)
			{
				foreach (var domain in domains)
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
		}

		private void LoadTermTypes()
		{
			TermTypes = TermTypeService.GetTermTypes();
		}

		// Set UI Settings fields selection based on the provider settings file (for domains and term types).
		private void SetFieldsSelection(ProviderSettings providerSettings)
		{
			if (providerSettings != null)
			{
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
}