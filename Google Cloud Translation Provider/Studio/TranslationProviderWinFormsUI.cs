using GoogleCloudTranslationProvider.Extensions;
using GoogleCloudTranslationProvider.Interfaces;
using GoogleCloudTranslationProvider.Models;
using GoogleCloudTranslationProvider.ViewModel;
using GoogleCloudTranslationProvider.Views;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GoogleCloudTranslationProvider.Studio;

[TranslationProviderWinFormsUi(Id = Constants.Provider_TranslationProviderWinFormsUi,
                               Name = Constants.Provider_TranslationProviderWinFormsUi,
                               Description = Constants.Provider_TranslationProviderWinFormsUi)]
public class TranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
{
    public string TypeDescription => PluginResources.Plugin_Description;

    public string TypeName => Constants.GoogleNaming_FullName;

    public bool SupportsEditing => true;

    public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
    {
        var options = new TranslationOptions(true);
        var mainWindowViewModel = ShowRequestedView(languagePairs, credentialStore, options);
        return mainWindowViewModel.DialogResult ? new ITranslationProvider[] { new TranslationProvider(options) }
                                                : null;
    }

    public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
    {
        if (translationProvider is not TranslationProvider editProvider)
        {
            return false;
        }

        var mainWindowViewModel = ShowRequestedView(languagePairs, credentialStore, editProvider.Options, true);
        return mainWindowViewModel.DialogResult;
    }

    public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
    {
        if (string.IsNullOrEmpty(translationProviderState))
        {
            return new TranslationProviderDisplayInfo()
            {
                SearchResultImage = PluginResources.my_image,
                TranslationProviderIcon = PluginResources.appicon,
                TooltipText = Constants.GoogleNaming_ShortName,
                Name = Constants.GoogleNaming_ShortName
            };
        }

        var options = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
        return new TranslationProviderDisplayInfo()
        {
            SearchResultImage = PluginResources.my_image,
            TranslationProviderIcon = PluginResources.appicon,
            TooltipText = options.ProviderName,
            Name = options.ProviderName
        };
    }

    public bool SupportsTranslationProviderUri(Uri translationProviderUri)
    {
        return translationProviderUri switch
        {
            null => throw new ArgumentNullException(PluginResources.UriNotSupportedMessage),
            _ => translationProviderUri.Scheme.Contains(Constants.GoogleTranslationScheme)
        };
    }

    private MainWindowViewModel ShowRequestedView(IEnumerable<LanguagePair> languagePairs, ITranslationProviderCredentialStore credentialStore, ITranslationOptions loadOptions, bool editProvider = false)
    {
        SetSavedCredentialsOnUi(credentialStore, loadOptions);
        if (editProvider)
        {
            _ = DatabaseExtensions.CreateDatabase(loadOptions);
        }

        var mainWindowViewModel = new MainWindowViewModel(loadOptions, credentialStore, languagePairs, editProvider);
        var mainWindowView = new MainWindowView { DataContext = mainWindowViewModel };
        mainWindowViewModel.CloseEventRaised += () =>
        {
            UpdateProviderCredentials(credentialStore, loadOptions);
            mainWindowView.Close();
        };

        mainWindowView.ShowDialog();
        return mainWindowViewModel;
    }

    private void UpdateProviderCredentials(ITranslationProviderCredentialStore credentialStore, ITranslationOptions options)
    {
        SetCredentialsOnCredentialStore(credentialStore, Constants.GoogleTranslationFullScheme, options.ApiKey, options.PersistGoogleKey);
    }

    private void SetSavedCredentialsOnUi(ITranslationProviderCredentialStore credentialStore, ITranslationOptions loadOptions)
    {
        var credentials = GetCredentialsFromStore(credentialStore, Constants.GoogleTranslationFullScheme);
        if (credentials is null)
        {
            return;
        }

        loadOptions.ApiKey = credentials.Credential;
        loadOptions.PersistGoogleKey = credentials.Persist;
    }

    private TranslationProviderCredential GetCredentialsFromStore(ITranslationProviderCredentialStore credentialStore, string uri)
    {
        var credentials = credentialStore.GetCredential(new Uri(uri));
        return credentials is not null ? new TranslationProviderCredential(credentials.Credential, credentials.Persist)
                                       : null;
    }

    private void SetCredentialsOnCredentialStore(ITranslationProviderCredentialStore credentialStore, string providerUri, string apiKey, bool persistKey)
    {
        var uri = new Uri(providerUri);
        var credentials = new TranslationProviderCredential(apiKey, persistKey);
        credentialStore.RemoveCredential(uri);
        credentialStore.AddCredential(uri, credentials);
    }

    //TODO PACH (06/04/2021): Confirm if this is still required/ remove if obsolete code
    /// <summary>
    /// This gets called when a TranslationProviderAuthenticationException is thrown
    /// Since SDL Studio doesn't pass the provider instance here and even if we do a workaround...
    /// any new options set in the form that comes up are never saved to the project XML...
    /// so there is no way to change any options, only to provide the credentials
    /// </summary>
    public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState,
        ITranslationProviderCredentialStore credentialStore)
    {
        var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
        var projectInfo = projectController?.CurrentProject?.GetProjectInfo();
        var languagePairs = new List<LanguagePair>();
        if (projectInfo is not null)
        {
            foreach (var targetLanguage in projectInfo.TargetLanguages)
            {
                var languagePair = new LanguagePair(projectInfo.SourceLanguage.CultureInfo, targetLanguage.CultureInfo);
                languagePairs.Add(languagePair);
            }
        }

        var options = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
        var mainWindowViewModel = ShowRequestedView(languagePairs.ToArray(), credentialStore, options);
        return mainWindowViewModel.DialogResult;
    }
}