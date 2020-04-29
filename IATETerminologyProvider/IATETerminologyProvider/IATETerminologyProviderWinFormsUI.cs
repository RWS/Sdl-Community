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
		private readonly IateSettingsService _settingsService = new IateSettingsService();


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
			var savedSettings = _settingsService.GetProviderSettings();
			var providerSettings = new SettingsModel
			{
				Domains = new List<DomainModel>(),
				TermTypes = new List<TermTypeModel>()
			};
			if (savedSettings != null)
			{
				providerSettings.Domains.AddRange(savedSettings.Domains);
				providerSettings.TermTypes.AddRange(savedSettings.TermTypes);
			}
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

		private ITerminologyProvider[] SetTerminologyProvider(IATETerminologyProvider provider, SettingsModel providerSettings)
		{
			var result = new List<ITerminologyProvider>();
			_settingsViewModel = new SettingsViewModel(providerSettings, _settingsService);
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