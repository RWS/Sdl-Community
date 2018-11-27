using System;
using System.Collections.Generic;
using System.Windows.Forms;
using IATETerminologyProvider.Model;
using IATETerminologyProvider.Service;
using IATETerminologyProvider.Ui;
using IATETerminologyProvider.ViewModel;
using Sdl.Terminology.TerminologyProvider.Core;

namespace IATETerminologyProvider
{
	[TerminologyProviderWinFormsUI]
	public class IATETerminologyProviderWinFormsUI : ITerminologyProviderWinFormsUI
	{
		#region Private Fields
		private SettingsViewModel _settingsViewModel;
		private SettingsWindow _settingsWindow;
		#endregion

		#region Public Properties
		public string TypeName => PluginResources.IATETerminologyProviderName;
		public string TypeDescription => PluginResources.IATETerminologyProviderDescription;
		public bool SupportsEditing => true;
		#endregion

		#region Public Methods
		public ITerminologyProvider[] Browse(IWin32Window owner, ITerminologyProviderCredentialStore credentialStore)
		{
			// open the Setting page when adding a new termbase provider
			var result = new List<ITerminologyProvider>();
			_settingsViewModel = new SettingsViewModel();
			_settingsWindow = new SettingsWindow(_settingsViewModel);
			_settingsViewModel.OnSaveSettingsCommandRaised += GetProviderSettings;

			_settingsWindow.ShowDialog();

			var termSearchService = new TermSearchService(_settingsViewModel.ProviderSettings);
			var IATETerminologyProvider = new IATETerminologyProvider(_settingsViewModel.ProviderSettings);

			result.Add(IATETerminologyProvider);
			return result.ToArray();

			//// use this part in case Settings page not needed anymore
			//var result = new List<ITerminologyProvider>();
			//var IATETerminologyProvider = new IATETerminologyProvider(null);
			//result.Add(IATETerminologyProvider);
			//return result.ToArray();
		}
		
		public bool Edit(IWin32Window owner, ITerminologyProvider terminologyProvider)
		{
			var persistenceService = new PersistenceService();
			var providerSettings = persistenceService.Load();

			//_settingsWindow.ShowDialog();

			return true;
		}

		public TerminologyProviderDisplayInfo GetDisplayInfo(Uri terminologyProviderUri)
		{
			return new TerminologyProviderDisplayInfo
			{
				Name = "IATE Terminology Provider",
				TooltipText = "IATE Terminology Provider"
			};
		}

		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
		{
			return terminologyProviderUri.Scheme == "iateglossary";
		}
		#endregion

		#region Private Methods
		private ProviderSettings GetProviderSettings()
		{
			_settingsWindow?.Close();
			return _settingsViewModel.ProviderSettings;
		}
		#endregion
	}
}