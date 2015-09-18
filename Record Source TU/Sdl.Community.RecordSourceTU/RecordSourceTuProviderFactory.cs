using System;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.RecordSourceTU
{
    [TranslationProviderFactory(
        Id = "Sdl.Community.RecordSourceTuFactory",
        Name = "Sdl.Community.RecordSourceTuFactory",
        Description = "Sdl.Community.RecordSourceTuFactory")]
    public class RecordSourceTuProviderFactory : ITranslationProviderFactory
    {
        #region ITranslationProviderFactory Members

        public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            var fileBasedUri = translationProviderUri.GetInnerProviderUri();
            var tProviderFactory = TranslationProviderManager.GetTranslationProviderFactory(fileBasedUri);
            var tProvider = tProviderFactory.CreateTranslationProvider(fileBasedUri, translationProviderState, credentialStore);
            return new RecordSourceTuTmTranslationProvider(tProvider);
        }

        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            var innerFactory =
                TranslationProviderManager.GetTranslationProviderFactory(translationProviderUri.GetInnerProviderUri());
            var info = innerFactory.GetTranslationProviderInfo(translationProviderUri.GetInnerProviderUri(),
                translationProviderState);
            return new TranslationProviderInfo()
            {
                Name = "Add Source TM",
                TranslationMethod = TranslationMethod.TranslationMemory
            };
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            return translationProviderUri.Scheme.StartsWith(
                RecordSourceTuTmTranslationProvider.ProviderUriScheme, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
