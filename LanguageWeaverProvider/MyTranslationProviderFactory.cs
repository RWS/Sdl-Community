using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace LanguageWeaverProvider
{
	[TranslationProviderFactory(Id = "Translation_Provider_Plug_inFactory",
								Name = "Translation_Provider_Plug_inFactory",
								Description = "Translation_Provider_Plug_inFactory")]
	internal class MyTranslationProviderFactory : ITranslationProviderFactory
	{
		#region ITranslationProviderFactory Members

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

		#endregion
	}
}
