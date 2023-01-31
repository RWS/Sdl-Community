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
using TMX_UI.ViewModel;

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

		public TmxTranslationProvider(TmxTranslationsOptions options)
		{
			_options = options;
		}

		public TmxTranslationsOptions Options => _options;

		public TmxSearchService SearchService => TmxSearchServiceProvider.GetSearchService(_options.Databases);
		public void UpdateOptions(OptionsViewModel options)
		{
			lock (this)
			{
				_options.CopyFrom(options);
				_languageDirections.Clear();
			}

			SaveState();
		}

		public void SaveState()
		{
			var currentProject = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
			var settings = currentProject.GetTranslationProviderConfiguration();
			var translationProvider = settings.Entries.FirstOrDefault(entry =>
				entry.MainTranslationProvider.Uri.OriginalString.StartsWith(TmxTranslationProvider.ProviderScheme)
				&& new TmxTranslationsOptions(entry.MainTranslationProvider.Uri).OptionsGuid == this.Options.OptionsGuid
				);

			if (translationProvider != null)
			{
				translationProvider.MainTranslationProvider.Uri = _options.Uri();
				currentProject.UpdateTranslationProviderConfiguration(settings);
			}
			else
			{
				var newEntry = new TranslationProviderCascadeEntry(new TranslationProviderReference(this.Options.Uri()), true, true, true, 0);
				settings.Entries.Add(newEntry);
			}
		}

		public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair pair)
		{
			TmxSearchService searchService;
			lock (this)
			{
				if (_languageDirections.TryGetValue(pair, out var ld))
					return ld;
				searchService = SearchService;
			}

			if (searchService?.SupportsLanguage(pair, Options.CareForLocale) ?? false)
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

		public bool IsReadOnly => false;

		public void LoadState(string translationProviderState)
		{
			_options = JsonConvert.DeserializeObject<TmxTranslationsOptions>(translationProviderState);
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
		public bool SupportsUpdate => true;

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
				searchService = SearchService;
			return (searchService?.SupportsLanguage(pair, _options.CareForLocale) ?? false);
		}


		public TranslationMethod TranslationMethod => TmxTranslationsOptions.ProviderTranslationMethod;

		public Uri Uri => _options.Uri();
	}
}

