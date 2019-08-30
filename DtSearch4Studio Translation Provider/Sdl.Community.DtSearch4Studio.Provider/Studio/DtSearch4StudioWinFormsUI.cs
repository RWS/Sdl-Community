using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Sdl.Community.DtSearch4Studio.Provider.Helpers;
using Sdl.Community.DtSearch4Studio.Provider.Model;
using Sdl.Community.DtSearch4Studio.Provider.Service;
using Sdl.Community.DtSearch4Studio.Provider.Ui;
using Sdl.Community.DtSearch4Studio.Provider.ViewModel;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.DtSearch4Studio.Provider.Studio
{
	[TranslationProviderWinFormsUi(Id = "DtSearch4StudioUiId",
								   Name = "DtSearch4StudioTranslationProviderUi",
								   Description = "DtSearch4Studio Translation Provider")]
	public class DtSearch4StudioWinFormsUI : ITranslationProviderWinFormsUI
	{
		private SettingsViewModel _settingsViewModel;
		private SettingsWindow _settingsWindow;
		private Dictionary<string, Assembly> _assemblies = new Dictionary<string, Assembly>();


		public string TypeName => "DtSearch4Studio Translation Provider";
		public string TypeDescription => "DtSearch4Studio Translation Provider";
		public static readonly Log Log = Log.Instance;

		// To be implemented all the methods /properties bellow
		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			try
			{
				var result = SetTranslationProvider(null, null);
				return result;
			}
			catch(Exception e)
			{
				Log.Logger.Error($"{Constants.Browse}: {e.Message}\n {e.StackTrace}");
			}			
			return null;
		}

		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			var persistenceService = new PersistenceService();
			var settings = persistenceService.GetProviderSettings();
			SetTranslationProvider((DtSearch4StudioProvider)translationProvider, settings);

			return true;
		}

		public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			return true;
		}

		public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
		{
			return new TranslationProviderDisplayInfo
			{
				Name = Constants.AppUpperName,
				TooltipText = Constants.AppLowerName,
				TranslationProviderIcon = PluginResources.dtSearch4Studio
			};
		}

		public bool SupportsEditing
		{
			get => true;
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			return translationProviderUri.Scheme == Constants.ProviderScheme;
		}

		private ProviderSettings GetProviderSettings()
		{
			_settingsWindow?.Close();
			return _settingsViewModel.ProviderSettings;
		}

		private ITranslationProvider[] SetTranslationProvider(DtSearch4StudioProvider provider, ProviderSettings providerSettings)
		{
			var result = new List<ITranslationProvider>();
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
			if (provider == null)
			{
				provider = new DtSearch4StudioProvider(providerSettings);
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