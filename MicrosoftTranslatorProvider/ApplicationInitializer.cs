using MicrosoftTranslatorProvider.Helpers;
using MicrosoftTranslatorProvider.Interfaces;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;

namespace MicrosoftTranslatorProvider
{
    [ApplicationInitializer]
    public class ApplicationInitializer : IApplicationInitializer
    {
        public static ITranslationProviderCredentialStore CredentialStore { get; set; }

        public static IDictionary<string, ITranslationOptions> TranslationOptions { get; set; } = new Dictionary<string, ITranslationOptions>();
        public static bool IsStandalone { get; set; } = true;

        public void Execute()
        {
            IsStandalone = false;
            Log.Setup();
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver.CurrentDomain_AssemblyResolve;
        }
    }
}