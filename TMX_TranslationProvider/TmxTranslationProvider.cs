using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Newtonsoft.Json;
using Sdl.FileTypeSupport.Framework;
using TMX_Lib.Search;
using TMX_Lib.TmxFormat;
using TMX_Lib.Utils;

namespace TMX_TranslationProvider
{

	public class TmxTranslationProvider : ITranslationProvider
	{
		///<summary>
		/// This string needs to be a unique value.
		/// This is the string that precedes the plug-in URI.
		///</summary>    
		public static readonly string ProviderScheme = "tmxsearch";

		private TmxTranslationsOptions _options;
		private Dictionary<LanguagePair, TmxTranslationProviderLanguageDirection> _languageDirections = new Dictionary<LanguagePair, TmxTranslationProviderLanguageDirection>();
		private TmxSearchService _searchService;

		public TmxTranslationProvider(TmxTranslationsOptions options, TmxSearchService service)
		{
			_options = options;
			_searchService = service;
		}

		public TmxTranslationsOptions Options => _options;
		public TmxSearchService SearchService => _searchService;
		public void SetSearchService(TmxSearchService service, TmxTranslationsOptions options)
		{
			lock (this)
			{
				_options = options;
				_searchService = service;
				_languageDirections.Clear();
			}
		}

		public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair pair)
		{
			TmxSearchService searchService;
			lock (this)
			{
				if (_languageDirections.TryGetValue(pair, out var ld))
					return ld;
				searchService = _searchService;
			}

			if (searchService?.SupportsLanguage(pair) ?? false)
			{
				var ld = new TmxTranslationProviderLanguageDirection(pair, this);
				lock (this)
				{
					if (_languageDirections.ContainsKey(pair))
						_languageDirections[pair] = ld;
					else
						_languageDirections.Add(pair, ld);
				}

				return ld;
			}
			else
				return null;
		}

		public bool IsReadOnly => true;

		public void LoadState(string translationProviderState)
		{
			_options = JsonConvert.DeserializeObject<TmxTranslationsOptions>(translationProviderState);
			_searchService = new TmxSearchService(_options);
			_languageDirections.Clear();
		}

		public string Name => PluginResources.Plugin_NiceName;
		public ProviderStatusInfo StatusInfo => new ProviderStatusInfo(true, Name);

		public void RefreshStatusInfo()
		{
		}

		public string SerializeState()
		{
			return JsonConvert.SerializeObject(_options);
		}

		public bool SupportsFilters => false;
		public bool SupportsWordCounts => false;
		public bool SupportsUpdate => false;

		// FIXME -> true
		public bool SupportsConcordanceSearch => false;
		public bool SupportsDocumentSearches => false;
		public bool SupportsFuzzySearch => false;
		public bool SupportsMultipleResults => false;
		public bool SupportsPenalties => true;
		public bool SupportsPlaceables => false;
		public bool SupportsScoring => false;
		public bool SupportsSearchForTranslationUnits => true;
		public bool SupportsSourceConcordanceSearch => false;
		public bool SupportsStructureContext => false;
		public bool SupportsTaggedInput => true;
		public bool SupportsTargetConcordanceSearch => false;
		public bool SupportsTranslation => true;


		public bool SupportsLanguageDirection(LanguagePair pair)
		{
			TmxSearchService searchService;
			lock (this)
				searchService = _searchService;
			return (searchService?.SupportsLanguage(pair) ?? false);
		}


		public TranslationMethod TranslationMethod => TmxTranslationsOptions.ProviderTranslationMethod;

		public Uri Uri => _options.Uri();
	}
}

