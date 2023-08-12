using System;
using System.Windows.Forms;
using LanguageWeaverProvider.LanguageMappingProvider;
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
			var translationOptions = new TranslationOptions();

			var CredentialsMainViewModel = new CredentialsMainViewModel(translationOptions);
			var CredentialsMainView = new CredentialsMainView { DataContext = CredentialsMainViewModel };
			CredentialsMainViewModel.CloseEventRaised += () =>
			{
				// UpdateProviderCredentials(credentialStore, loadOptions);
				CredentialsMainView.Close();
			};

			CredentialsMainView.ShowDialog();
			if (!CredentialsMainViewModel.SaveChanges)
			{
				return null;
			}

			var pairMappingViewModel = ShowPairMappingView(languagePairs, credentialStore, translationOptions);
			if (!pairMappingViewModel.SaveChanges)
			{
				return null;
			}

			var translationProvider = new TranslationProvider(translationOptions);
			return new ITranslationProvider[] { translationProvider };
		}

		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			if (translationProvider is not TranslationProvider editProvider)
			{
				return false;
			}

			var pairMappingViewModel = ShowPairMappingView(languagePairs, credentialStore, editProvider.TranslationOptions, true);
			return pairMappingViewModel.SaveChanges;
		}

		private PairMappingViewModel ShowPairMappingView(LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore, ITranslationOptions translationOptions, bool editProvider = false)
		{
			var pairMappingViewModel = new PairMappingViewModel(translationOptions, languagePairs);
			var pairMappingView = new PairMappingView() { DataContext = pairMappingViewModel };
			pairMappingViewModel.CloseEventRaised += pairMappingView.Close;
			pairMappingView.ShowDialog();
			return pairMappingViewModel;
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