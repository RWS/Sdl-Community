using System;
using System.Windows.Forms;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Model.Options;
using LanguageWeaverProvider.Services;
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
            ApplicationInitializer.CredentialStore = credentialStore;

            var translationOptions = new TranslationOptions(true);
            CredentialManager.GetCredentials(translationOptions);

            var credentialsMainViewModel = new CredentialsMainViewModel(translationOptions);
            var credentialsMainView = new CredentialsMainView { DataContext = credentialsMainViewModel };
            credentialsMainViewModel.CloseEventRaised += () =>
            {
                CredentialManager.UpdateCredentials(credentialStore, translationOptions);
                credentialsMainView.Close();
            };

            credentialsMainView.ShowDialog();
            if (!credentialsMainViewModel.SaveChanges)
            {
                return null;
            }

            var pairMappingViewModel = ShowPairMappingView(languagePairs, translationOptions);
            if (!pairMappingViewModel.SaveChanges)
            {
                return null;
            }

            var translationProvider = new TranslationProvider(translationOptions);
            return [translationProvider];
        }

        public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            ApplicationInitializer.CredentialStore = credentialStore;

            if (translationProvider is not TranslationProvider editProvider)
            {
                return false;
            }

            var pairMappingViewModel = ShowPairMappingView(languagePairs, editProvider.TranslationOptions);
            return pairMappingViewModel.SaveChanges;
        }

        private PairMappingViewModel ShowPairMappingView(LanguagePair[] languagePairs, ITranslationOptions translationOptions)
        {
            Service.ValidateToken(translationOptions);
            var pairMappingViewModel = new PairMappingViewModel(translationOptions, languagePairs);
            var pairMappingView = new PairMappingView() { DataContext = pairMappingViewModel };
            pairMappingViewModel.CloseEventRaised += pairMappingView.Close;
            pairMappingView.ShowDialog();
            return pairMappingViewModel;
        }

        public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
        {
            var pluginName = string.IsNullOrEmpty(translationProviderState) switch
            {
                true => Constants.PluginName,
                false => JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState).ProviderName
            };

            var images = translationProviderUri.AbsoluteUri switch
            {
                Constants.CloudFullScheme => (PluginResources.lwLogo_Cloud_Icon, PluginResources.lwLogo_Cloud16),
                Constants.EdgeFullScheme => (PluginResources.lwLogo_Edge_Icon, PluginResources.lwLogo_Edge16),
                _ => throw new ArgumentException("Unsupported PluginVersion value"),
            };

            return new TranslationProviderDisplayInfo()
            {
                Name = pluginName,
                TooltipText = pluginName,
                TranslationProviderIcon = images.Item1,
                SearchResultImage = images.Item2
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