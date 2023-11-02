using System;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Windows.Forms;
using System.Windows.Media;
using LanguageWeaverProvider.Extensions;
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
			CredentialManager.GetCredentials(credentialStore, translationOptions);
			var CredentialsMainViewModel = new CredentialsMainViewModel(translationOptions);
			var CredentialsMainView = new CredentialsMainView { DataContext = CredentialsMainViewModel };
			CredentialsMainViewModel.CloseEventRaised += () =>
			{
				CredentialManager.UpdateCredentials(credentialStore, translationOptions);
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

			var translationProvider = new TranslationProvider(translationOptions, credentialStore);
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
			if (string.IsNullOrEmpty(translationProviderState))
			{
				return new TranslationProviderDisplayInfo()
				{
					Name = PluginResources.Plugin_Name,
					TooltipText = PluginResources.Plugin_Name,
					TranslationProviderIcon = PluginResources.lwLogoIco,
					SearchResultImage = PluginResources.lwLogoPng
				};

			}

			var translationOptions = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
			var pluginName = translationOptions.Version == PluginVersion.LanguageWeaverCloud ? Constants.PluginNameCloud : Constants.PluginNameEdge;
			if (!string.IsNullOrEmpty(translationOptions.ProviderSettings.CustomName) && translationOptions.ProviderSettings.UseCustomName)
			{
				pluginName += $" - {translationOptions.ProviderSettings.CustomName}";
			}

			return new TranslationProviderDisplayInfo()
			{
				Name = pluginName,
				TooltipText = pluginName,
				TranslationProviderIcon = PluginResources.lwLogoIco,
				SearchResultImage = PluginResources.lwLogoPng
			};
		}
		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			var supportsTranslationProviderUri = translationProviderUri switch
			{
				null => throw new ArgumentNullException("Unsuported"),
				_ => translationProviderUri.Scheme.StartsWith(Constants.BaseTranslationScheme)
			};

			return supportsTranslationProviderUri;
		}

		public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
			=> false;
	}
}