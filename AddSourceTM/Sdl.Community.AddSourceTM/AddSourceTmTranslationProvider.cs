using System;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.AddSourceTM
{
    public class AddSourceTmTranslationProvider : ITranslationProvider
    {
        internal const string ProviderUriScheme = "sdlcommunityastp";
        internal const string ProviderUri = ProviderUriScheme + "://";

        
        public ITranslationProvider FileBasedTranslationProvider
        {
            get { return _fileBasedTranslationProvider; }
        }

        private readonly ITranslationProvider _fileBasedTranslationProvider;
        public AddSourceTmTranslationProvider(ITranslationProvider provider)
        {
            _fileBasedTranslationProvider = provider;
        }

        #region ITranslationProvider Members

        public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languagePair)
        {
            var languageDirection = _fileBasedTranslationProvider.GetLanguageDirection(languagePair);
            return new AddSourceTmLanguageDirection(languageDirection);
        }

        public bool IsReadOnly
        {
            get { return _fileBasedTranslationProvider.IsReadOnly; }
        }

        public void LoadState(string translationProviderState)
        {
            _fileBasedTranslationProvider.LoadState(translationProviderState);
        }

        public string Name
        {
            get { return _fileBasedTranslationProvider.Name; }
        }

        public void RefreshStatusInfo()
        {
            _fileBasedTranslationProvider.RefreshStatusInfo();
        }

        public string SerializeState()
        {
            return _fileBasedTranslationProvider.SerializeState();
        }

        public ProviderStatusInfo StatusInfo
        {
            get { return _fileBasedTranslationProvider.StatusInfo; }
        }

        public bool SupportsConcordanceSearch
        {
            get { return _fileBasedTranslationProvider.SupportsConcordanceSearch; }
        }

        public bool SupportsDocumentSearches
        {
            get { return _fileBasedTranslationProvider.SupportsDocumentSearches; }
        }

        public bool SupportsFilters
        {
            get { return _fileBasedTranslationProvider.SupportsFilters; }
        }

        public bool SupportsFuzzySearch
        {
            get { return _fileBasedTranslationProvider.SupportsFuzzySearch; }
        }

        public bool SupportsLanguageDirection(LanguagePair languageDirection)
        {
            return _fileBasedTranslationProvider.SupportsLanguageDirection(languageDirection);
        }

        public bool SupportsMultipleResults
        {
            get { return _fileBasedTranslationProvider.SupportsMultipleResults; }
        }

        public bool SupportsPenalties
        {
            get { return _fileBasedTranslationProvider.SupportsPenalties; }
        }

        public bool SupportsPlaceables
        {
            get { return _fileBasedTranslationProvider.SupportsPlaceables; }
        }

        public bool SupportsScoring
        {
            get { return _fileBasedTranslationProvider.SupportsScoring; }
        }

        public bool SupportsSearchForTranslationUnits
        {
            get { return _fileBasedTranslationProvider.SupportsSearchForTranslationUnits; }
        }

        public bool SupportsSourceConcordanceSearch
        {
            get { return _fileBasedTranslationProvider.SupportsSourceConcordanceSearch; }
        }

        public bool SupportsStructureContext
        {
            get { return _fileBasedTranslationProvider.SupportsStructureContext; }
        }

        public bool SupportsTaggedInput
        {
            get { return _fileBasedTranslationProvider.SupportsTaggedInput; }
        }

        public bool SupportsTargetConcordanceSearch
        {
            get { return _fileBasedTranslationProvider.SupportsTargetConcordanceSearch; }
        }

        public bool SupportsTranslation
        {
            get { return _fileBasedTranslationProvider.SupportsTranslation; }
        }

        public bool SupportsUpdate
        {
            get { return _fileBasedTranslationProvider.SupportsUpdate; }
        }

        public bool SupportsWordCounts
        {
            get { return _fileBasedTranslationProvider.SupportsWordCounts; }
        }

        public TranslationMethod TranslationMethod
        {
            get { return _fileBasedTranslationProvider.TranslationMethod; }
        }

        public Uri Uri
        {
            get
            {
                return new Uri(ProviderUri + FileBasedTranslationProvider.Uri.AbsolutePath);
            }
        }

        #endregion
    }
}

