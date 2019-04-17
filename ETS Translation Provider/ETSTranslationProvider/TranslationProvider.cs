using System;
using System.Linq;
using ETSLPConverter;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace ETSTranslationProvider
{
	public class TranslationProvider : ITranslationProvider
    {
        public const string TranslationProviderScheme = "ets";

        public TranslationOptions Options { get; set; }

        public TranslationProvider(TranslationOptions options)
        {
            Log.Logger.Trace("");
            Options = options;
        }

        #region ITranslationProvider Properties
        public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
        {
            Log.Logger.Trace("");
            return new TranslationProviderLanguageDirection(this, languageDirection);
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public void LoadState(string translationProviderState)
        {
            Log.Logger.Trace("");
            Options = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
        }

        public string Name
        {
            get { return PluginResources.Plugin_NiceName; }
        }

        public void RefreshStatusInfo()
        {
            Log.Logger.Trace("");
        }

        public string SerializeState()
        {
            Log.Logger.Trace("");
            return JsonConvert.SerializeObject(Options);
        }

        public ProviderStatusInfo StatusInfo
        {
            get { return new ProviderStatusInfo(true, PluginResources.Plugin_NiceName); }
        }

        public bool SupportsConcordanceSearch
        {
            get { return false; }
        }

        public bool SupportsDocumentSearches
        {
            get { return false; }
        }

        public bool SupportsFilters
        {
            get { return false; }
        }

        public bool SupportsFuzzySearch
        {
            get { return false; }
        }

        public bool SupportsLanguageDirection(LanguagePair languageDirection)
        {
            Log.Logger.Trace("");
            return ETSApi.ETSTranslatorHelper.GetLanguagePairs(Options).Any(lp =>
                languageDirection.SourceCulture.ToETSCode().Equals(lp.SourceLanguageId) &&
                languageDirection.TargetCulture.ToETSCode().Equals(lp.TargetLanguageId));
        }

        public bool SupportsMultipleResults
        {
            get { return false; }
        }

        public bool SupportsPenalties
        {
            get { return false; }
        }

        public bool SupportsPlaceables
        {
            get { return false; }
        }

        public bool SupportsScoring
        {
            get { return true; }
        }

        public bool SupportsSearchForTranslationUnits
        {
            get { return true; }
        }

        public bool SupportsSourceConcordanceSearch
        {
            get { return false; }
        }

        public bool SupportsTargetConcordanceSearch
        {
            get { return false; }
        }

        public bool SupportsStructureContext
        {
            get { return false; }
        }

        public bool SupportsTaggedInput
        {
            get { return true; }
        }

        public bool SupportsTranslation
        {
            get { return true; }
        }

        public bool SupportsUpdate
        {
            get { return false; }
        }

        public bool SupportsWordCounts
        {
            get { return false; }
        }

        public TranslationMethod TranslationMethod
        {
            get { return TranslationMethod.MachineTranslation; }
        }

        public Uri Uri
        {
            get { return Options.Uri; }
        }
        #endregion
    }
}