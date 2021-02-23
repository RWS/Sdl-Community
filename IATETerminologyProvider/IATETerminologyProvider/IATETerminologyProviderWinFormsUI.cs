using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.Community.IATETerminologyProvider.Service;
using Sdl.Community.IATETerminologyProvider.Ui;
using Sdl.Community.IATETerminologyProvider.ViewModel;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.IATETerminologyProvider
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
			var provider = terminologyProvider as IATETerminologyProvider;
			var savedSettings = provider?.ProviderSettings;

			SetTerminologyProvider(provider, savedSettings);

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

		private ITerminologyProvider[] SetTerminologyProvider(IATETerminologyProvider provider,
			SettingsModel providerSettings)
		{
			var result = new List<ITerminologyProvider>();
			var messageBoxService = new MessageBoxService();

			_settingsViewModel = new SettingsViewModel(providerSettings, messageBoxService);
			_settingsWindow = new SettingsWindow
			{
				DataContext = _settingsViewModel
			};

			_settingsWindow.ShowDialog();
			if (!_settingsViewModel.DialogResult) return null;
			providerSettings = _settingsViewModel.ProviderSettings;

			if (provider is null)
			{
				provider = new IATETerminologyProvider(providerSettings);
			}
			else
			{
				provider.UpdateSettings(providerSettings);
			}

			result.Add(provider);

			return result.ToArray();
		}
	}
}