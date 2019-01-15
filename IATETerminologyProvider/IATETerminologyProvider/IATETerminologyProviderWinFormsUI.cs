using System;
using System.Collections.Generic;
using System.Windows.Forms;
using IATETerminologyProvider.Helpers;
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
		private SettingsViewModel _settingsViewModel;
		private SettingsWindow _settingsWindow;		

		public string TypeName => PluginResources.IATETerminologyProviderName;
		public string TypeDescription => PluginResources.IATETerminologyProviderDescription;
		public bool SupportsEditing => true;
		public ITerminologyProvider[] Browse(IWin32Window owner, ITerminologyProviderCredentialStore credentialStore)
		{
			var result = SetTerminologyProvider(null, null);
			return result;					
		}
		
		public bool Edit(IWin32Window owner, ITerminologyProvider terminologyProvider)
		{
			var persistenceService = new PersistenceService();
			var providerSettings = persistenceService.Load();	

			SetTerminologyProvider(terminologyProvider as IATETerminologyProvider, providerSettings);

			return true;
		}

		public TerminologyProviderDisplayInfo GetDisplayInfo(Uri terminologyProviderUri)
		{
			return new TerminologyProviderDisplayInfo
			{
				Name = Constants.IATEProviderName,
				TooltipText = Constants.IATEProviderDescription
			};
		}

		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
		{
			return terminologyProviderUri.Scheme == Constants.IATEGlossary;
		}

		private ProviderSettings GetProviderSettings()
		{
			_settingsWindow?.Close();
			return _settingsViewModel.ProviderSettings;
		}

		private ITerminologyProvider[] SetTerminologyProvider(IATETerminologyProvider provider, ProviderSettings providerSettings)
		{
			var result = new List<ITerminologyProvider>();

			if (_settingsViewModel != null)
			{
				_settingsViewModel.OnSaveSettingsCommandRaised -= GetProviderSettings;
			}
			
			_settingsViewModel = new SettingsViewModel(providerSettings);
			_settingsWindow = new SettingsWindow(_settingsViewModel);

			if (_settingsViewModel != null)
			{				
				_settingsViewModel.OnSaveSettingsCommandRaised += GetProviderSettings;
			}
			
			_settingsWindow.ShowDialog();

			providerSettings = _settingsViewModel.ProviderSettings;
			provider.UpdateSettings(providerSettings);

			result.Add(provider);

			return result.ToArray();
		}
	}
}