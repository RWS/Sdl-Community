using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Newtonsoft.Json;

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

		public TmxTranslationProvider(TmxTranslationsOptions options)
		{
			_options = options.Clone();
		}

		public TmxTranslationsOptions Options
		{
			get => _options;
			set => _options = value;
		}

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


		public bool SupportsConcordanceSearch => true;

		public bool SupportsDocumentSearches => true;

		public bool SupportsFilters => false;

		public bool SupportsFuzzySearch => true;

		public bool SupportsMultipleResults => true;

		public bool SupportsPenalties => true;

		public bool SupportsPlaceables => true;

		public bool SupportsScoring => true;

		public bool SupportsSearchForTranslationUnits => true;

		public bool SupportsSourceConcordanceSearch => true;

		public bool SupportsStructureContext => true;

		public bool SupportsTaggedInput => true;

		public bool SupportsTargetConcordanceSearch => true;

		public bool SupportsTranslation => true;

		public bool SupportsUpdate => true;

		public bool SupportsWordCounts => true;


		public bool SupportsLanguageDirection(LanguagePair languageDirection)
		{
			return true;
		}


		public TranslationMethod TranslationMethod => TmxTranslationsOptions.ProviderTranslationMethod;

		public Uri Uri => _options.Uri();
	}
}

