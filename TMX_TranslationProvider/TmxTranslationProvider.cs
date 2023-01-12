using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Newtonsoft.Json;
using Sdl.FileTypeSupport.Framework;
using Sdl.ProjectAutomation.Core;
using TMX_Lib.Search;
using TMX_Lib.TmxFormat;
using TMX_Lib.Utils;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using MongoDB.Driver;

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
			Debug.Assert(options.Guid != "");
			lock (this)
			{
				_options = options;
				_searchService = service;
				_languageDirections.Clear();
			}
			TmxTranslationProviderFactory.ReplaceSearchService(_options.Guid, service);

			SaveState();
		}

		public void SaveState()
		{
			// FIXME this can be problematic if we have several TMX providers
			var currentProject = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
			var settings = currentProject.GetTranslationProviderConfiguration();
			var translationProvider = settings.Entries.FirstOrDefault(entry =>
				entry.MainTranslationProvider.Uri.OriginalString.Contains(TmxTranslationProvider.ProviderScheme));

			if (translationProvider != null)
			{
				translationProvider.MainTranslationProvider.Uri = _options.Uri();
				currentProject.UpdateTranslationProviderConfiguration(settings);
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

		public string Name => $"TMX Translation: {_options.FriendlyName}";
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

		public bool SupportsConcordanceSearch => true;
		public bool SupportsDocumentSearches => true;
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

