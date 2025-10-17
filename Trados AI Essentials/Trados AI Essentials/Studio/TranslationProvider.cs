using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;

namespace Trados_AI_Essentials.Studio
{
    internal class TranslationProvider : ITranslationProvider
    {
        private ProviderStatusInfo _statusInfo;

        #region ITranslationProvider Members

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public ProviderStatusInfo StatusInfo
        {
            get => _statusInfo = new ProviderStatusInfo(true, "Status");
            set => _statusInfo = value;
        }

        public bool SupportsConcordanceSearch =>
            //TODO
            true;

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

        public bool SupportsSearchForTranslationUnits
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsSourceConcordanceSearch
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsStructureContext
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsTaggedInput
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsTargetConcordanceSearch
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsTranslation
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsUpdate
        {
            //TODO
            get { return true; }
        }

        public bool SupportsWordCounts
        {
            get { throw new NotImplementedException(); }
        }

        public TranslationMethod TranslationMethod => TranslationMethod.MachineTranslation;

        public Uri Uri => new Uri(PluginResources.ProviderScheme);

        public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
        {
            throw new NotImplementedException();
        }

        public void LoadState(string translationProviderState)
        {
            throw new NotImplementedException();
        }

        public void RefreshStatusInfo()
        {
            StatusInfo = new ProviderStatusInfo(true, "Refreshed");
        }

        public string SerializeState()
        {
            return "";
        }

        public bool SupportsLanguageDirection(LanguagePair languageDirection)
        {
            //TODO
            return true;
        }

        #endregion ITranslationProvider Members
    }
}