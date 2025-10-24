using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using Trados_AI_Essentials.Model;

namespace Trados_AI_Essentials.Studio
{
    public class TranslationProvider : ITranslationProvider
    {
        private ProviderStatusInfo _statusInfo;

        private TranslationProvider()
        {
        }

        public static TranslationProvider CreateFromSettings(Settings settings) => new() { Settings = settings };

        public static TranslationProvider CreateFromState(string state)
        {
            var translationProvider = new TranslationProvider();
            translationProvider.LoadState(state);
            return translationProvider;
        }

        #region ITranslationProvider Members

        public bool IsReadOnly => true;

        public string Name => PluginResources.Plugin_Name;

        public Settings Settings { get; set; }

        public ProviderStatusInfo StatusInfo
        {
            get => _statusInfo = new ProviderStatusInfo(true, "Status");
            set => _statusInfo = value;
        }

        public bool SupportsConcordanceSearch => false;

        public bool SupportsDocumentSearches
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsFilters
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsFuzzySearch
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsMultipleResults
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsPenalties
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsPlaceables
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsScoring
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsSearchForTranslationUnits => true;

        public bool SupportsSourceConcordanceSearch
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsStructureContext => false;

        public bool SupportsTaggedInput => false;

        public bool SupportsTargetConcordanceSearch => false;

        public bool SupportsTranslation => true;

        public bool SupportsUpdate
        {
            //TODO - implement supportsUpdate functionality
            get { return true; }
        }

        public bool SupportsWordCounts
        {
            get { throw new NotImplementedException(); }
        }

        public TranslationMethod TranslationMethod => TranslationMethod.MachineTranslation;

        public Uri Uri => new Uri(PluginResources.ProviderScheme);

        public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection) =>
            StudioIntegration.GetTranslationProviderLanguageDirection(languageDirection, this);

        public void LoadState(string translationProviderState) =>
            Settings = JsonConvert.DeserializeObject<Settings>(translationProviderState);

        public void RefreshStatusInfo()
        {
            StatusInfo = new ProviderStatusInfo(true, "Refreshed");
        }

        public string SerializeState()
        {
            return JsonConvert.SerializeObject(Settings);
        }

        public bool SupportsLanguageDirection(LanguagePair languageDirection)
        {
            //TODO - check supported languages from settings
            return true;
        }

        #endregion ITranslationProvider Members
    }
}