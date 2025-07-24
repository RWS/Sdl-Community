using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Options;
using LanguageWeaverProvider.Services;
using LanguageWeaverProvider.WindowsCredentialStore;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;

namespace LanguageWeaverProvider;

[TranslationProviderFactory(Id = Constants.Provider_TranslationProviderFactory,
                            Name = Constants.Provider_TranslationProviderFactory,
                            Description = Constants.Provider_TranslationProviderFactory)]
internal class TranslationProviderFactory : ITranslationProviderFactory
{
    public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore _)
    {
        ApplicationInitializer.PluginVersion = translationProviderUri.ToPluginVersion();

        if (translationProviderState is null) return new TranslationProvider(new TranslationOptions());

        var serializedCredentials = CredentialStore.Load($"{translationProviderUri}cred");
        var standaloneCredentials = new StandaloneCredentials(serializedCredentials);

        ApplicationInitializer.IsStandalone = standaloneCredentials.AuthenticationType != AuthenticationType.None;

        var options = GetOptions(translationProviderState);
        CredentialManager.GetCredentials(options, true, standaloneCredentials);

        Service.ValidateToken(options);
        ApplicationInitializer.TranslationOptions[options.Id] = options;

        return new TranslationProvider(options);
    }

    public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
    {
        var pluginName = string.IsNullOrEmpty(translationProviderState)
                       ? Constants.PluginName
                       : JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState).ProviderName;

        return new TranslationProviderInfo
        {
            TranslationMethod = TranslationMethod.MachineTranslation,
            Name = pluginName
        };
    }

    public bool SupportsTranslationProviderUri(Uri translationProviderUri)
    {
        return translationProviderUri switch
        {
            null => throw new ArgumentNullException("Unsuported"),
            _ => translationProviderUri.Scheme.StartsWith(Constants.BaseTranslationScheme)
        };
    }

    private static TranslationOptions GetOptions(string translationProviderState)
    {
        TranslationOptions options;
        try
        {
            options = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
        }
        catch
        {
            options = new TranslationOptions();
        }
        if (options.Id is null) options.AssignId();

        return options;
    }
}