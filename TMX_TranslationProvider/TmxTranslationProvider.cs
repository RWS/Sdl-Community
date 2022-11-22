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
		private TmxTranslationProviderLanguageDirection _languageDirection;
		private TmxParser _parser;

		public TmxTranslationProvider(TmxTranslationsOptions options)
		{
			_options = options;
			_parser = _options.FileName != "" ? new TmxParser(_options.FileName) : null;
			TryLoadLanguageDirection();
		}

		public TmxTranslationsOptions Options
		{
			get => _options;
			set
			{
				_options = value;

				// force reload
				_parser = null;
				TryLoadLanguageDirection();
			}
		}

		public TmxParser Parser => _parser;

		public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair pair)
		{
			if (_languageDirection != null && _languageDirection.SourceLanguage.IsoLanguageName() == pair.SourceCultureName &&
			    _languageDirection.TargetLanguage.IsoLanguageName() == pair.TargetCultureName)
				return _languageDirection;
			return null;
		}

		public bool IsReadOnly => true;

		public void LoadState(string translationProviderState)
		{
			_options = JsonConvert.DeserializeObject<TmxTranslationsOptions>(translationProviderState);
			_parser = _options.FileName != "" ? new TmxParser(_options.FileName) : null;
			_languageDirection = null;

			TryLoadLanguageDirection();
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

		private void TryLoadLanguageDirection()
		{
			if (_parser == null)
			{
				_languageDirection = null;
				return;
			}

			if (_languageDirection != null &&
			    (_languageDirection.SourceLanguage.Name != _parser.Header.SourceLanguage || _languageDirection.TargetLanguage.Name != _parser.Header.TargetLanguage))
				_languageDirection = new TmxTranslationProviderLanguageDirection(new LanguagePair(_parser.Header.SourceLanguage, _parser.Header.TargetLanguage), this);
			if (_languageDirection == null)
				_languageDirection = new TmxTranslationProviderLanguageDirection(new LanguagePair(_parser.Header.SourceLanguage, _parser.Header.TargetLanguage), this);
		}

		public bool SupportsLanguageDirection(LanguagePair pair)
		{
			if (_languageDirection == null)
				return false;
			var supports = _languageDirection.SourceLanguage.IsoLanguageName() == pair.SourceCultureName &&
			               _languageDirection.TargetLanguage.IsoLanguageName() == pair.TargetCultureName;
			return supports;
		}


		public TranslationMethod TranslationMethod => TmxTranslationsOptions.ProviderTranslationMethod;

		public Uri Uri => _options.Uri();
	}
}

