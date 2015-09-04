using System;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.AddSourceTM
{
    [TranslationProviderFactory(
        Id = "Sdl.Community.AddSourceTMFactory",
        Name = "Sdl.Community.AddSourceTMFactory",
        Description = "Sdl.Community.AddSourceTMFactory")]
    public class AddSourceTmProviderFactory : ITranslationProviderFactory
    {
        #region ITranslationProviderFactory Members

        public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            var fileBasedUri = translationProviderUri.GetInnerProviderUri();
            var tProviderFactory = TranslationProviderManager.GetTranslationProviderFactory(fileBasedUri);
            var tProvider = tProviderFactory.CreateTranslationProvider(fileBasedUri, translationProviderState, credentialStore);
            return new AddSourceTmTranslationProvider(tProvider);
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
                AddSourceTmTranslationProvider.ProviderUriScheme, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
