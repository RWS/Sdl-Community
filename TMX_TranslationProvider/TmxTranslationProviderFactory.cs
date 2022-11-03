using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace TMX_TranslationProvider
{
	[TranslationProviderFactory(Id = "TmxTranslationProviderFactory",
								Name = "TmxTranslationProviderFactory",
								Description = "TmxTranslationProviderFactory")]
	public class TmxTranslationProviderFactory : ITranslationProviderFactory
	{
		#region ITranslationProviderFactory Members

		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			if (!SupportsTranslationProviderUri(translationProviderUri))
			{
				throw new Exception($"Cannot handle URI. {translationProviderUri}");
			}

			return new TmxTranslationProvider(new TmxTranslationsOptions(translationProviderUri));
		}

		public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
		{
			return new TranslationProviderInfo
			{
				TranslationMethod = TmxTranslationsOptions.ProviderTranslationMethod,
				Name = PluginResources.Plugin_Name, 
			};
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			return string.Equals(translationProviderUri.Scheme, TmxTranslationProvider.ProviderScheme, StringComparison.OrdinalIgnoreCase);
		}

		#endregion
	}
}
