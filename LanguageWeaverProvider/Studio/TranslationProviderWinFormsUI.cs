using System;
using System.Windows.Forms;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Model.Options;
using LanguageWeaverProvider.View;
using LanguageWeaverProvider.ViewModel;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace LanguageWeaverProvider
{
	[TranslationProviderWinFormsUi(Id = Constants.Provider_TranslationProviderWinFormsUi,
								   Name = Constants.Provider_TranslationProviderWinFormsUi,
								   Description = Constants.Provider_TranslationProviderWinFormsUi)]
	internal class TranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
	{
		public bool SupportsEditing => true;

		public string TypeName => Constants.PluginName;

		public string TypeDescription => Constants.PluginName;

		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			var translationOptions = new TranslationOptions();
			GetProviderCredentials(credentialStore, translationOptions);
			var CredentialsMainViewModel = new CredentialsMainViewModel(translationOptions);
			var CredentialsMainView = new CredentialsMainView { DataContext = CredentialsMainViewModel };
			CredentialsMainViewModel.CloseEventRaised += () =>
			{
				UpdateProviderCredentials(credentialStore, translationOptions);
				CredentialsMainView.Close();
			};

			CredentialsMainView.ShowDialog();
			if (!CredentialsMainViewModel.SaveChanges)
			{
				return null;
			}

			var pairMappingViewModel = ShowPairMappingView(languagePairs, translationOptions);
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

			var pairMappingViewModel = ShowPairMappingView(languagePairs, editProvider.TranslationOptions, true);
			return pairMappingViewModel.SaveChanges;
		}

		private PairMappingViewModel ShowPairMappingView(LanguagePair[] languagePairs, ITranslationOptions translationOptions, bool editProvider = false)
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

		private void UpdateProviderCredentials(ITranslationProviderCredentialStore credentialStore, ITranslationOptions translationOptions)
		{
			if (translationOptions.AuthenticationType != AuthenticationType.CloudCredentials)
			{
				return;
			}

			var uri = new Uri(Constants.CloudFullScheme);
			var cloudCredentials = new CloudCredentials()
			{
				UserID = translationOptions.CloudCredentials.UserID,
				UserPassword = translationOptions.CloudCredentials.UserPassword
			};

			var cloudCredentialsJson = JsonConvert.SerializeObject(cloudCredentials);
			var credentials = new TranslationProviderCredential(cloudCredentialsJson, true);

			credentialStore.RemoveCredential(uri);
			credentialStore.AddCredential(uri, credentials);
		}

		private void GetProviderCredentials(ITranslationProviderCredentialStore credentialStore, ITranslationOptions translationOptions)
		{
			var uri = new Uri(Constants.CloudFullScheme);
			var credentials = credentialStore.GetCredential(uri);
			if (credentials is null)
			{
				return;
			}

			var cloudCredentials = JsonConvert.DeserializeObject<CloudCredentials>(credentials.Credential);
			translationOptions.CloudCredentials = cloudCredentials;
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
			=> translationProviderUri is not null;

		public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
			=> false;
	}
}