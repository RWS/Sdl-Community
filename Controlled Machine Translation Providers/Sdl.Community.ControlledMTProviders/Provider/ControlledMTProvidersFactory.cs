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
    public class ControlledMTProvidersFactory : ITranslationProviderFactory
    {
        #region ITranslationProviderFactory Members

        public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            var puis = JsonConvert.DeserializeObject<List<ProviderUriInfo>>(translationProviderState);
            List<ITranslationProvider> managedTranslationProviders = new List<ITranslationProvider>();

            foreach (var pui in puis)
            {
               ITranslationProviderFactory providerFactory = TranslationProviderManager.GetTranslationProviderFactory(pui.Uri);
               managedTranslationProviders.Add(providerFactory.CreateTranslationProvider(pui.Uri, pui.SerializedState, credentialStore));
            }
            
            return new ControlledMTProvidersProvider(managedTranslationProviders);
        }

        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            TranslationProviderInfo translationProviderInfo = new TranslationProviderInfo();
            translationProviderInfo.Name = "Controled MT Providers";
            translationProviderInfo.TranslationMethod = TranslationMethod.MachineTranslation;
            return translationProviderInfo;
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            return translationProviderUri.Scheme.Equals(new Uri(ControlledMTProvidersProvider.ProviderUri).Scheme);
        }

        #endregion
    }
}
