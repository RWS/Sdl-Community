using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.DeepLMTProvider
{
    [TranslationProviderFactory(Id = "DeepLMtTranslationProviderFactory",
                             Name = "DeepLMtTranslationProviderFactory",
                             Description = "DeepL Mt Translation Provider")]
    public class DeepLMtTranslationProviderFactory : ITranslationProviderFactory
    {
        public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            var tp = new DeepLMtTranslationProvider();
            return tp;
        }

        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            throw new NotImplementedException();
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            //here see MT enhanced for the actual  value
            return true;
        }
    }
}
