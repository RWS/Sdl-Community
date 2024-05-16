using Sdl.Community.AmazonTranslateTradosPlugin.Model;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System.Collections.Generic;

namespace Sdl.Community.AmazonTranslateTradosPlugin
{
    [ApplicationInitializer]
    public class ApplicationInitializer : IApplicationInitializer
    {
        public static ITranslationProviderCredentialStore CredentialStore { get; set; }

        public static IDictionary<string, TranslationOptions> TranslationOptions { get; set; }

        public void Execute()
        {
            TranslationOptions = new Dictionary<string, TranslationOptions>();
        }
    }
}