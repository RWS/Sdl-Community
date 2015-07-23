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
            var fileBasedUri = new Uri("sdltm.file://"+translationProviderUri.AbsolutePath);
            var tProviderFactory = TranslationProviderManager.GetTranslationProviderFactory(fileBasedUri);
            var tProvider = tProviderFactory.CreateTranslationProvider(fileBasedUri, translationProviderState, credentialStore);
            return new AddSourceTmTranslationProvider(tProvider);
        }

        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            return new TranslationProviderInfo()
            {
                Name = "AddSourceTM",
                TranslationMethod = TranslationMethod.TranslationMemory
            };
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            return String.Equals(translationProviderUri.Scheme,
                AddSourceTmTranslationProvider.ProviderUriScheme, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
