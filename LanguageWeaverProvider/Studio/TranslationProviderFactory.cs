using System;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace LanguageWeaverProvider
{
	[TranslationProviderFactory(Id = "Translation_Provider_Plug_inFactory",
								Name = "Translation_Provider_Plug_inFactory",
								Description = "Translation_Provider_Plug_inFactory")]
	internal class TranslationProviderFactory : ITranslationProviderFactory
	{
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			throw new NotImplementedException();
		}

		public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
		{
			throw new NotImplementedException();
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			throw new NotImplementedException();
		}
	}
}