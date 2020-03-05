using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Newtonsoft.Json;

namespace Sdl.Community.ControlledMTProviders.Provider
{
    [TranslationProviderFactory(
        Id = "Sdl.Community.ControlledMTProvidersFactory",
        Name = "ControledMTProviders",
        Description = "ControledMTProviders")]
    public class ControlledMtProvidersFactory : ITranslationProviderFactory
    {

        public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            var puis = JsonConvert.DeserializeObject<List<ProviderUriInfo>>(translationProviderState);
            var managedTranslationProviders = (from pui in puis
                let providerFactory = TranslationProviderManager.GetTranslationProviderFactory(pui.Uri)
                select providerFactory.CreateTranslationProvider(pui.Uri, pui.SerializedState, credentialStore))
                .ToList();

            return new ControlledMtProvidersProvider(managedTranslationProviders);
        }

        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            var translationProviderInfo = new TranslationProviderInfo
            {
                Name = "Controled MT Providers",
                TranslationMethod = TranslationMethod.MachineTranslation
            };
            return translationProviderInfo;
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            return translationProviderUri.Scheme.Equals(new Uri(ControlledMtProvidersProvider.ProviderUri).Scheme);
        }

    }
}
