using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
		#region Private Fields
		private ICommand _saveSettingsCommand;
		private ICommand _cancelCommand;
		private DomainModel _selectedDomain;
		private ObservableCollection<DomainModel> _domains = new ObservableCollection<DomainModel>();
		private TermTypeModel _selectedTermType;
		private ObservableCollection<TermTypeModel> _termTypes = new ObservableCollection<TermTypeModel>();
		#endregion

		#region Public Constructors
		public SettingsViewModel()
		{
			LoadDomains();
			LoadTermTypes();
		}
		#endregion

		#region Public Properties		
		
		public DomainModel SelectedDomain
		{
			get => _selectedDomain;
			set
			{
				_selectedDomain = value;				
				OnPropertyChanged();
			}
		}

		public ObservableCollection<TermTypeModel> TermTypes
		{
			get => _termTypes;
			set
			{
				_termTypes = value;
				OnPropertyChanged();
			}
		}

		public TermTypeModel SelectedTermType
		{
			get => _selectedTermType;
			set
			{
				_selectedTermType = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<DomainModel> Domains
		{
			get => _domains;
			set
			{
				_domains = value;
				OnPropertyChanged();
			}
		}


		public ProviderSettings ProviderSettings { get; set; }
		#endregion

		#region Commands
		public ICommand SaveSettingsCommand => _saveSettingsCommand ?? (_saveSettingsCommand = new CommandHandler(SaveSettingsAction, true));
		public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new CommandHandler(CancelAction, true));
		#endregion

		#region Actions
		private void SaveSettingsAction()
		{
			if (Domains.Count > 0)
			{
				ProviderSettings = new ProviderSettings();
				ProviderSettings.Domains = new List<string>();
				ProviderSettings.TermTypes = new List<string>();

				// Add selected domains to provider settings
				var selectedDomains = Domains.Where(d => d.IsSelected).ToList();
				if (selectedDomains != null)
				{
					foreach (var selectedDomain in selectedDomains)
					{
						ProviderSettings.Domains.Add(selectedDomain.Code);
					}
				}

				// Add selected term types to provider settings
				var selectedTermTypes = TermTypes.Where(d => d.IsSelected).ToList();
				if (selectedTermTypes != null)
				{
					foreach (var selectedTermType in selectedTermTypes)
					{
						ProviderSettings.TermTypes.Add(selectedTermType.Name);
					}
				}

				var persistenceService = new PersistenceService();
				persistenceService.AddSettings(ProviderSettings);

				OnSaveSettingsCommandRaised?.Invoke();
			}
		}

		private void CancelAction()
		{
		}
		#endregion

		#region PublicMethods
		#endregion

		#region Private Methods
		private void LoadDomains()
		{
			var domains = DomainService.GetDomains();
			foreach (var domain in domains)
			{
				if (!domain.Name.Equals(Constants.NotSpecifiedCode))
				{
					var selectedDomainName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(domain.Name.ToLower());
					var domainModel = new DomainModel
					{
						Code = domain.Code,
						Name = selectedDomainName
					};
					Domains.Add(domainModel);
				}
			}
		}

		private void LoadTermTypes()
		{
			TermTypes = TermTypeService.GetTermTypes();
		}
		#endregion

		#region Events
		public delegate ProviderSettings SaveSettingsEventRaiser();
		public event SaveSettingsEventRaiser OnSaveSettingsCommandRaised;
		#endregion
	}
}