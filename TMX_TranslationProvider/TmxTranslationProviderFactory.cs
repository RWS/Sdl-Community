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

		private static object _lock = new object();
		private static Dictionary<string, TmxSearchService> _services = new Dictionary<string, TmxSearchService>();

		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			if (!SupportsTranslationProviderUri(translationProviderUri))
			{
				throw new Exception($"Cannot handle URI. {translationProviderUri}");
			}

			var options = new TmxTranslationsOptions(translationProviderUri);
			if (options.Guid == "")
			{
				// i don't know the GUID - can't cache
				var service = new TmxSearchService(options);
				return new TmxTranslationProvider(options, service);
			}

			lock (_lock)
			{
				if (_services.TryGetValue(options.Guid, out var existingService))
				{
					// async call
					existingService.SetOptionsAsync(options);
					return new TmxTranslationProvider(options, existingService);
				}
				else
				{
					var service = new TmxSearchService(options);
					_services.Add(options.Guid, service);
					return new TmxTranslationProvider(options, service);
				}
			}
		}

		public static void ReplaceSearchService(string guid, TmxSearchService service)
		{
			lock (_lock)
			{
				if (_services.ContainsKey(guid))
					_services[guid] = service;
				else 
					_services.Add(guid, service);
			}
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
