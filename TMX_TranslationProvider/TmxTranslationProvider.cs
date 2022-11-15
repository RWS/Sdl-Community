using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Newtonsoft.Json;
using TMX_TranslationProvider.TmxFormat;

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
		private TmxTranslationProviderLanguageDirection _languageDirection;
		private TmxParser _parser;

		public TmxTranslationProvider(TmxTranslationsOptions options)
		{
			_options = options;
			_parser = _options.FileName != "" ? new TmxParser(_options.FileName) : null;
			if (_parser != null)
				// loading happens async
				_parser.LoadAsync();
		}

		public TmxTranslationsOptions Options
		{
			get => _options;
			set => _options = value;
		}

		public TmxParser Parser => _parser;

		public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair pair)
		{
			if (_languageDirection == null)
				_languageDirection = new TmxTranslationProviderLanguageDirection(pair, this);

			if (_languageDirection.SourceLanguage.Name != pair.SourceCulture.Name || _languageDirection.TargetLanguage.Name != pair.TargetCulture.Name)
				_languageDirection = new TmxTranslationProviderLanguageDirection(pair, this);

			return _languageDirection;
		}

		public bool IsReadOnly => true;

		public void LoadState(string translationProviderState)
		{
			_options = JsonConvert.DeserializeObject<TmxTranslationsOptions>(translationProviderState);
			_parser = _options.FileName != "" ? new TmxParser(_options.FileName) : null;

			if (_parser != null)
				// loading happens async
				_parser.LoadAsync();
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


		public bool SupportsLanguageDirection(LanguagePair languageDirection)
		{
			return true;
		}


		public TranslationMethod TranslationMethod => TmxTranslationsOptions.ProviderTranslationMethod;

		public Uri Uri => _options.Uri();
	}
}

