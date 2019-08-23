using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.DtSearch4Studio.Provider.Commands;
using Sdl.Community.DtSearch4Studio.Provider.Helpers;
using Sdl.Community.DtSearch4Studio.Provider.Model;
using Sdl.Community.DtSearch4Studio.Provider.Service;

namespace Sdl.Community.DtSearch4Studio.Provider.ViewModel
{
	public class SettingsViewModel : BaseViewModel
	{
		#region Private Fields
		private ICommand _okCommand;
		private ICommand _browseCommand;
		private string _indexLocation;

		#endregion

		#region Public Constructors
		public SettingsViewModel(ProviderSettings providerSettings)
		{
			SetFieldsSelection(providerSettings);
		}
		#endregion

		#region Public Properties				
		public ProviderSettings ProviderSettings { get; set; }

		public string IndexLocation
		{
			get => _indexLocation;
			set
			{
				_indexLocation = value;
				OnPropertyChanged(nameof(IndexLocation));
			}
		}

		#endregion

		#region Commands
		public ICommand OkCommand => _okCommand ?? (_okCommand = new CommandHandler(OkAction, true));
		public ICommand BrowseCommand => _browseCommand ?? (_browseCommand = new CommandHandler(BrowseAction, true));

		#endregion

		#region Actions		
		private void OkAction()
		{
			ProviderSettings = new ProviderSettings
			{
				IndexPath = IndexLocation
			};

			var persistenceService = new PersistenceService();
			persistenceService.AddSettings(ProviderSettings);

			OnSaveSettingsCommandRaised?.Invoke();
		}

		/// <summary>
		/// Select index from the local machine
		/// </summary>
		private void BrowseAction()
		{
			var fbd = new FolderSelectDialog();
			if (fbd.ShowDialog())
			{
				if (!string.IsNullOrEmpty(fbd.FileName))
				{
					// user can select only one index
					IndexLocation = fbd.FileName;
				}
				else
				{
					MessageBox.Show(Constants.NoIndexSelected, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
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