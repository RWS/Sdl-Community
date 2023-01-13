using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using TMX_Lib.Search;
using LogManager = NLog.LogManager;

namespace TMX_TranslationProvider
{
	[TranslationProviderFactory(Id = "TmxTranslationProviderFactory",
								Name = "TmxTranslationProviderFactory",
								Description = "TmxTranslationProviderFactory")]
	public class TmxTranslationProviderFactory : ITranslationProviderFactory
	{
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		#region ITranslationProviderFactory Members


		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			if (!SupportsTranslationProviderUri(translationProviderUri))
			{
				throw new Exception($"Cannot handle URI. {translationProviderUri}");
			}

			var options = new TmxTranslationsOptions(translationProviderUri);
			return new TmxTranslationProvider( options );
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
