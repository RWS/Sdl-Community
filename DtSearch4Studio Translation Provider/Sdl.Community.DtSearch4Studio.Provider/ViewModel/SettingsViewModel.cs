using System.Configuration;
using System.Windows.Input;
using Sdl.Community.DtSearch4Studio.Provider.Commands;
using Sdl.Community.DtSearch4Studio.Provider.Service;

namespace Sdl.Community.DtSearch4Studio.Provider.ViewModel
{
	public class SettingsViewModel : BaseViewModel
	{
		#region Private Fields
		private ICommand _saveSettingsCommand;
		#endregion

		#region Public Constructors
		public SettingsViewModel(ProviderSettings providerSettings)
		{
			SetFieldsSelection(providerSettings);
		}
		#endregion

		#region Public Properties		
		

		public ProviderSettings ProviderSettings { get; set; }
		#endregion

		#region Commands
		public ICommand SaveSettingsCommand => _saveSettingsCommand ?? (_saveSettingsCommand = new CommandHandler(SaveSettingsAction, true));
		#endregion

		#region Actions
		private void SaveSettingsAction()
		{
			// save
			ProviderSettings = new ProviderSettings
			{

			};
			//To do: add the index to provider settings

			var persistenceService = new PersistenceService();
			persistenceService.AddSettings(ProviderSettings);

			OnSaveSettingsCommandRaised?.Invoke();

		}
		#endregion

		#region PublicMethods
		#endregion

		#region Private Methods
		private void LoadIndexes()
		{
			//To do: Load the indexes from dtSearch Desktop app 
			
		}

		// Set UI Settings fields selection based on the provider settings file (for indexes).
		private void SetFieldsSelection(ProviderSettings providerSettings)
		{
			if (providerSettings != null)
			{
				// to do: set the selection of index
				//foreach (var domainCode in providerSettings.Domains)
				//{
				//	var domain = Domains?.FirstOrDefault(d => d.Code.Equals(domainCode));
				//	if (domain != null)
				//	{
				//		domain.IsSelected = true;
				//	}
				//}
			}
		}
		#endregion

		#region Events
		public delegate ProviderSettings SaveSettingsEventRaiser();
		public event SaveSettingsEventRaiser OnSaveSettingsCommandRaised;
		#endregion
	}
}