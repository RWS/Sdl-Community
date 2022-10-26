using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleTranslationProvider
{
    internal class MyTranslationProvider : ITranslationProvider
    {
        #region ITranslationProvider Members

        public MyTranslationProvider()
        {
            Uri = new Uri($"{MyTranslationProviderWinFormsUI.TRANSLATION_URI}://");
        }

        private MyTranslationProviderLanguageDirection _languageDirection ;
        public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair pair)
        {
            if (_languageDirection == null)
                _languageDirection = new MyTranslationProviderLanguageDirection(pair, this);

            if (_languageDirection.SourceLanguage.Name != pair.SourceCulture.Name || _languageDirection.TargetLanguage.Name != pair.TargetCulture.Name)
                _languageDirection = new MyTranslationProviderLanguageDirection(pair, this);

            return _languageDirection;
        }

        public bool IsReadOnly => true;

        public void LoadState(string translationProviderState)
        {
        }

        public string Name => MyTranslationProviderWinFormsUI.TRANSLATION_URI;

        public void RefreshStatusInfo()
        {
        }

        public string SerializeState()
        {
            return "";
        }

        public ProviderStatusInfo StatusInfo => new ProviderStatusInfo(true, MyTranslationProviderWinFormsUI.TRANSLATION_URI);

        public bool SupportsConcordanceSearch => false;

        public bool SupportsDocumentSearches => false;

        public bool SupportsFilters => false;

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

        public bool SupportsUpdate => false;

        public bool SupportsWordCounts => false;


        public bool SupportsLanguageDirection(LanguagePair languageDirection)
        {
            return true;
        }


        public TranslationMethod TranslationMethod => TranslationMethod.MachineTranslation;

        public Uri Uri
        {
            get;
        }

        #endregion
    }
}

