using Newtonsoft.Json;
using NLog;
using Sdl.Community.MTEdge.Provider.Helpers;
using Sdl.Community.MTEdge.Provider.Model;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Linq;

namespace Sdl.Community.MTEdge.Provider.Studio
{
    public class Provider : ITranslationProvider
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public Provider(TranslationOptions options)
		{
			_logger.Trace("");
			Options = options;
		}

		public string Name => PluginResources.Plugin_NiceName;

        public ProviderStatusInfo StatusInfo => new(true, PluginResources.Plugin_NiceName);

        public TranslationMethod TranslationMethod => TranslationMethod.MachineTranslation;

        public TranslationOptions Options { get; set; }

        public Uri Uri => Options.Uri;
		
        public bool SupportsSearchForTranslationUnits => true;
		
        public bool SupportsSourceConcordanceSearch => false;
		
        public bool SupportsTargetConcordanceSearch => false;
		
        public bool SupportsConcordanceSearch => false;
		
        public bool SupportsDocumentSearches => false;
		
        public bool SupportsStructureContext => false;
		
        public bool SupportsMultipleResults => false;
		
        public bool SupportsFuzzySearch => false;
		
        public bool SupportsPlaceables => false;
		
        public bool SupportsTaggedInput => true;
		
        public bool SupportsTranslation => true;
		
        public bool SupportsWordCounts => false;
		
        public bool SupportsPenalties => false;
		
        public bool SupportsFilters => false;
		
        public bool SupportsScoring => true;
		
        public bool SupportsUpdate => false;
		
        public bool IsReadOnly => true;

        public bool SupportsLanguageDirection(LanguagePair languageDirection)
        {
            // MtEdge doesn't have ptb as source language, we need to map it to por
            if (languageDirection.SourceCulture.ToMTEdgeCode().Equals("ptb"))
            {
                return SDLMTEdgeTranslatorHelper
                      .GetLanguagePairs(Options)
                      .Any(lp => lp.SourceLanguageId.Equals("por")
                              && languageDirection.TargetCulture.ToMTEdgeCode().Equals(lp.TargetLanguageId));
            }

            return SDLMTEdgeTranslatorHelper
                  .GetLanguagePairs(Options)
                  .Any(lp => languageDirection.SourceCulture.ToMTEdgeCode().Equals(lp.SourceLanguageId)
                          && languageDirection.TargetCulture.ToMTEdgeCode().Equals(lp.TargetLanguageId));
        }

        public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
        {
            _logger.Trace("");
            return new ProviderLanguageDirection(this, languageDirection);
        }

        public void LoadState(string translationProviderState)
        {
            _logger.Trace("");
            Options = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
        }

        public string SerializeState()
        {
            _logger.Trace("");
            return JsonConvert.SerializeObject(Options);
        }

        public void RefreshStatusInfo()
        {
            _logger.Trace("");
        }
    }
}