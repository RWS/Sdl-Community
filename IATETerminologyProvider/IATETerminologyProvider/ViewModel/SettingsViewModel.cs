using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using IATETerminologyProvider.Commands;
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

				var selectedItems = Domains.Where(d => d.IsSelected).ToList();
				if (selectedItems != null)
				{
					foreach(var selectedItem in selectedItems)
					{
						ProviderSettings.Domains.Add(selectedItem.Name);
					}
				}
				//To Do: add the TermTypes to providerSettings
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
			foreach(var domain in domains)
			{
				var domainModel = new DomainModel
				{
					Code = domain.Code,
					Name = domain.Name
				};
				Domains.Add(domainModel);
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