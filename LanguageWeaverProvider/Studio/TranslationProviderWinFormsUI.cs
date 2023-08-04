using System;
using System.Windows.Forms;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Model.Options;
using LanguageWeaverProvider.View;
using LanguageWeaverProvider.ViewModel;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace LanguageWeaverProvider
{
	[TranslationProviderWinFormsUi(Id = "Translation_Provider_Plug_inWinFormsUI",
								   Name = "Translation_Provider_Plug_inWinFormsUI",
								   Description = "Translation_Provider_Plug_inWinFormsUI")]
	internal class TranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
	{
		public bool SupportsEditing => true;

		public string TypeName => Constants.PluginName;

		public string TypeDescription => Constants.PluginName;

		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			var options = new TranslationOptions();
			var mainWindowViewModel = ShowRequestedView(languagePairs, credentialStore, options);
			if (!mainWindowViewModel.SaveChanges)
			{
				return null;
			}

			var translationProvider = new TranslationProvider(options);
			return new ITranslationProvider[] { translationProvider };
		}

		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			if (translationProvider is not TranslationProvider editProvider)
			{
				return false;
			}

			var mainWindowViewModel = ShowRequestedView(languagePairs, credentialStore, editProvider.TranslationOptions, true);
			return mainWindowViewModel.SaveChanges;
		}

		private MainViewModel ShowRequestedView(LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore, ITranslationOptions loadOptions, bool editProvider = false)
		{
			var mainWindowViewModel = new MainViewModel(loadOptions);
			var mainWindowView = new MainWindowView { DataContext = mainWindowViewModel };
			mainWindowViewModel.CloseEventRaised += () =>
			{
				// UpdateProviderCredentials(credentialStore, loadOptions);
				mainWindowView.Close();
			};

			mainWindowView.ShowDialog();
			return mainWindowViewModel;
		}

		public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
		{
			var displayInfo = new TranslationProviderDisplayInfo()
			{
				Name = PluginResources.Plugin_Name,
				TooltipText = PluginResources.Plugin_Name,
				TranslationProviderIcon = PluginResources.lwLogoIco,
				SearchResultImage = PluginResources.lwLogoPng
			};

			return displayInfo;
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
			=> translationProviderUri is not null;

		public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
			=> false;
	}
}