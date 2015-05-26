using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;

namespace Sdl.Community.ControlledMTProviders.Provider
{
    public class CascadeLanguageDirection : ITranslationProviderLanguageDirection
    {
        private readonly ControlledMtProvidersProvider _provider;
        private readonly LanguagePair _languagePair;
        private static bool? _disableMt;

        public static event EventHandler TranslationFinished;

        

        public static bool DisableMt
        {
            get
            {
                if (!_disableMt.HasValue) return false;
                return _disableMt.Value;
            }
            set
            {
                _disableMt = value;
            }
        }

        public CascadeLanguageDirection(ControlledMtProvidersProvider provider,LanguagePair languagePair)
        {
            _provider = provider;
            _languagePair = languagePair;
        }

        public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
        {
            return null;
        }

        public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
        {
            return null;

        }

        public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
        {
            return null;
        }

        public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
        {
            return null;
        }

        public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
        {
            return null;
        }

        public bool CanReverseLanguageDirection
        {
            get { return false; }
        }

        public SearchResults SearchSegment(SearchSettings settings, Segment segment)
        {
            var results = new SearchResults();
            if (DisableMt) return results;

            foreach (var provider in _provider.MtProviders)
            {
                if (SearchShouldExecute(provider, settings))
                {
                    ITranslationProviderLanguageDirection languageDirection = provider.GetLanguageDirection(_languagePair);
                    var innerSearchResult = languageDirection.SearchSegment(settings, segment);
                    SetResultInSearchResults(ref results, innerSearchResult);
                }
            }
            OnTranslationFinished();
            return results;
        }

        public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
        {
            if (segments == null)
                throw new ArgumentNullException("segments");
           
            var result
                = new SearchResults[segments.Length];

            if (DisableMt) return result;

            foreach (var provider in _provider.MtProviders)
            {
                if (SearchShouldExecute(provider, settings))
                {
                    ITranslationProviderLanguageDirection languageDirection = provider.GetLanguageDirection(_languagePair);
                    IList<SearchResults> internalResult = languageDirection.SearchSegments(settings, segments);

                    MergeProviderSearchResults(result, internalResult);
                }
            }
            OnTranslationFinished();
            return result;
        }

        public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
        {
            if (segments == null)
                throw new ArgumentNullException("segments");
            if (mask == null)
                throw new ArgumentNullException("mask");
            if (mask.Length != segments.Length)
                throw new ArgumentException("The length of the mask parameter should match the number segments.", "mask");


            var result
                = new SearchResults[segments.Length];

            if (DisableMt) return result;

            foreach (var provider in _provider.MtProviders)
            {
                if (SearchShouldExecute(provider, settings))
                {
                    var languageDirection = provider.GetLanguageDirection(_languagePair);
                    IList<SearchResults> internalResult = languageDirection.SearchSegmentsMasked(settings, segments, mask);

                    MergeProviderSearchResults(result, internalResult);
                }
            }
            OnTranslationFinished();
            return result;
        }

        public SearchResults SearchText(SearchSettings settings, string segment)
        {
            SearchResults results = null;
            if (DisableMt) return null;

            foreach (var provider in _provider.MtProviders)
            {
                if (SearchShouldExecute(provider, settings))
                {
                    ITranslationProviderLanguageDirection languageDirection = provider.GetLanguageDirection(_languagePair);
                    var innerSearchResult = languageDirection.SearchText(settings, segment);
                    SetResultInSearchResults(ref results, innerSearchResult);
                }
                
            }
            OnTranslationFinished();
            return results;
        }

        public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
        {
            SearchResults results = null;
            if (DisableMt) return null;

            foreach (var provider in _provider.MtProviders)
            {
                if (SearchShouldExecute(provider, settings))
                {
                    ITranslationProviderLanguageDirection languageDirection = provider.GetLanguageDirection(_languagePair);
                    var innerSearchResult = languageDirection.SearchTranslationUnit(settings, translationUnit);
                    SetResultInSearchResults(ref results, innerSearchResult);
                }
            }
            OnTranslationFinished();
            return results;
        }

        public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
        {
            if (translationUnits == null)
                throw new ArgumentNullException("translationUnits");
            
            var result
               = new SearchResults[translationUnits.Length];

            if (DisableMt) return result;

            foreach (var provider in _provider.MtProviders)
            {
                if (SearchShouldExecute(provider, settings))
                {
                    ITranslationProviderLanguageDirection languageDirection = provider.GetLanguageDirection(_languagePair);
                    IList<SearchResults> providerResult = languageDirection.SearchTranslationUnits(settings, translationUnits);

                    MergeProviderSearchResults(result, providerResult);
                }
            }
            OnTranslationFinished();

            return result;
        }

        public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
        {
            if (translationUnits == null)
                throw new ArgumentNullException("translationUnits");
            if (mask == null)
                throw new ArgumentNullException("mask");
            if (mask.Length != translationUnits.Length)
                throw new ArgumentException("The length of the mask parameter should match the number translation units.", "mask");

            var result
               = new SearchResults[translationUnits.Length];

            if (DisableMt) return result;

            foreach (var provider in _provider.MtProviders)
            {
                if (SearchShouldExecute(provider, settings))
                {
                    ITranslationProviderLanguageDirection languageDirection = provider.GetLanguageDirection(_languagePair);
                    IList<SearchResults> providerResult = languageDirection.SearchTranslationUnitsMasked(settings, translationUnits, mask);

                    MergeProviderSearchResults(result, providerResult);
                }
            }
           
            OnTranslationFinished();
            return result;

        }


        public System.Globalization.CultureInfo SourceLanguage
        {
            get { return _languagePair.SourceCulture; }
        }

        public System.Globalization.CultureInfo TargetLanguage
        {
            get { return _languagePair.TargetCulture; }
        }

        public ITranslationProvider TranslationProvider
        {
            get { return _provider; }
        }

        public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
        {
            return null;
        }

        public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
        {
            return null;
        }

        private bool SearchShouldExecute(ITranslationProvider translationProvider, SearchSettings searchSettings)
        {
            if (!searchSettings.IsConcordanceSearch && !translationProvider.SupportsTranslation)
            {
                return false;
            }
            if (searchSettings.IsConcordanceSearch && !translationProvider.SupportsConcordanceSearch)
            {
                return false;
            }
          
            if (searchSettings.Mode == SearchMode.ConcordanceSearch && !translationProvider.SupportsSourceConcordanceSearch)
            {
                return false;
            }
            if (searchSettings.Mode == SearchMode.TargetConcordanceSearch && !translationProvider.SupportsTargetConcordanceSearch)
            {
                return false;
            }

            return true;
        }

        private void MergeProviderSearchResults(SearchResults[] result, IList<SearchResults> providerResult)
        {
            for (int i = 0; i < providerResult.Count; ++i)
            {
                var tmpProviderResult = providerResult[i];
                if (tmpProviderResult == null) continue;
                SetResultInSearchResults(ref result[i],  tmpProviderResult);
            }
        }

        private void SetResultInSearchResults(ref SearchResults result, SearchResults tmpProviderResult)
        {
            if (result == null)
            {
                result = tmpProviderResult;
            }
            else
            {
                result.Results.AddRange(tmpProviderResult);
            }
        }

        private static void OnTranslationFinished()
        {
            var handler = TranslationFinished;
            if (handler != null) handler(null, EventArgs.Empty);
        }

    }
}
