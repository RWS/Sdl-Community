using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.Community.ControlledMTProviders.Provider
{
    public class CascadeLanguageDirection : ITranslationProviderLanguageDirection
    {
        private ControlledMTProvidersProvider _provider;
        private LanguagePair _languagePair;
        private static bool? _disableMT;
        public static bool DisableMT
        {
            get
            {
                if (!_disableMT.HasValue) return false;
                return _disableMT.Value;
            }
            set
            {
                _disableMT = value;
            }
        }

        public CascadeLanguageDirection(ControlledMTProvidersProvider provider,LanguagePair languagePair)
        {
            _provider = provider;
            _languagePair = languagePair;
        }

        public LanguagePlatform.TranslationMemory.ImportResult[] AddOrUpdateTranslationUnits(LanguagePlatform.TranslationMemory.TranslationUnit[] translationUnits, int[] previousTranslationHashes, LanguagePlatform.TranslationMemory.ImportSettings settings)
        {
            return null;
        }

        public LanguagePlatform.TranslationMemory.ImportResult[] AddOrUpdateTranslationUnitsMasked(LanguagePlatform.TranslationMemory.TranslationUnit[] translationUnits, int[] previousTranslationHashes, LanguagePlatform.TranslationMemory.ImportSettings settings, bool[] mask)
        {
            return null;

        }

        public LanguagePlatform.TranslationMemory.ImportResult AddTranslationUnit(LanguagePlatform.TranslationMemory.TranslationUnit translationUnit, LanguagePlatform.TranslationMemory.ImportSettings settings)
        {
            return null;
        }

        public LanguagePlatform.TranslationMemory.ImportResult[] AddTranslationUnits(LanguagePlatform.TranslationMemory.TranslationUnit[] translationUnits, LanguagePlatform.TranslationMemory.ImportSettings settings)
        {
            return null;
        }

        public LanguagePlatform.TranslationMemory.ImportResult[] AddTranslationUnitsMasked(LanguagePlatform.TranslationMemory.TranslationUnit[] translationUnits, LanguagePlatform.TranslationMemory.ImportSettings settings, bool[] mask)
        {
            return null;
        }

        public bool CanReverseLanguageDirection
        {
            get { return false; }
        }

        public LanguagePlatform.TranslationMemory.SearchResults SearchSegment(LanguagePlatform.TranslationMemory.SearchSettings settings, Segment segment)
        {
            SearchResults results = new SearchResults();
            if (DisableMT) return results;

            foreach (var provider in _provider.MTProviders)
            {
                if (SearchShouldExecute(provider, settings))
                {
                    ITranslationProviderLanguageDirection languageDirection = provider.GetLanguageDirection(_languagePair);
                    var innerSearchResult = languageDirection.SearchSegment(settings, segment);
                    SetResultInSearchResults(ref results, innerSearchResult);
                }
            }

            return results;
        }

        public LanguagePlatform.TranslationMemory.SearchResults[] SearchSegments(LanguagePlatform.TranslationMemory.SearchSettings settings, Segment[] segments)
        {
            if (segments == null)
                throw new ArgumentNullException("segments");
           
            Sdl.LanguagePlatform.TranslationMemory.SearchResults[] result
                = new SearchResults[segments.Length];

            if (DisableMT) return result;

            foreach (var provider in _provider.MTProviders)
            {
                if (SearchShouldExecute(provider, settings))
                {
                    ITranslationProviderLanguageDirection languageDirection = provider.GetLanguageDirection(_languagePair);
                    IList<Sdl.LanguagePlatform.TranslationMemory.SearchResults> internalResult = languageDirection.SearchSegments(settings, segments);

                    MergeProviderSearchResults(result, internalResult);
                }
            }

            return result;
        }

        public LanguagePlatform.TranslationMemory.SearchResults[] SearchSegmentsMasked(LanguagePlatform.TranslationMemory.SearchSettings settings, Segment[] segments, bool[] mask)
        {
            if (segments == null)
                throw new ArgumentNullException("segments");
            if (mask == null)
                throw new ArgumentNullException("mask");
            if (mask.Length != segments.Length)
                throw new ArgumentException("The length of the mask parameter should match the number segments.", "mask");


            Sdl.LanguagePlatform.TranslationMemory.SearchResults[] result
                = new SearchResults[segments.Length];

            if (DisableMT) return result;

            foreach (var provider in _provider.MTProviders)
            {
                if (SearchShouldExecute(provider, settings))
                {
                    ITranslationProviderLanguageDirection languageDirection = provider.GetLanguageDirection(_languagePair);
                    IList<Sdl.LanguagePlatform.TranslationMemory.SearchResults> internalResult = languageDirection.SearchSegmentsMasked(settings, segments, mask);

                    MergeProviderSearchResults(result, internalResult);
                }
            }

            return result;
        }

        public LanguagePlatform.TranslationMemory.SearchResults SearchText(LanguagePlatform.TranslationMemory.SearchSettings settings, string segment)
        {
            SearchResults results = null;
            if (DisableMT) return results;

            foreach (var provider in _provider.MTProviders)
            {
                if (SearchShouldExecute(provider, settings))
                {
                    ITranslationProviderLanguageDirection languageDirection = provider.GetLanguageDirection(_languagePair);
                    var innerSearchResult = languageDirection.SearchText(settings, segment);
                    SetResultInSearchResults(ref results, innerSearchResult);
                }
                
            }

            return results;
        }

        public LanguagePlatform.TranslationMemory.SearchResults SearchTranslationUnit(LanguagePlatform.TranslationMemory.SearchSettings settings, LanguagePlatform.TranslationMemory.TranslationUnit translationUnit)
        {
            SearchResults results = null;
            if (DisableMT) return results;

            foreach (var provider in _provider.MTProviders)
            {
                if (SearchShouldExecute(provider, settings))
                {
                    ITranslationProviderLanguageDirection languageDirection = provider.GetLanguageDirection(_languagePair);
                    var innerSearchResult = languageDirection.SearchTranslationUnit(settings, translationUnit);
                    SetResultInSearchResults(ref results, innerSearchResult);
                }
            }

            return results;
        }

        public LanguagePlatform.TranslationMemory.SearchResults[] SearchTranslationUnits(LanguagePlatform.TranslationMemory.SearchSettings settings, LanguagePlatform.TranslationMemory.TranslationUnit[] translationUnits)
        {
            if (translationUnits == null)
                throw new ArgumentNullException("translationUnits");
            
            Sdl.LanguagePlatform.TranslationMemory.SearchResults[] result
               = new SearchResults[translationUnits.Length];

            if (DisableMT) return result;

            foreach (var provider in _provider.MTProviders)
            {
                if (SearchShouldExecute(provider, settings))
                {
                    ITranslationProviderLanguageDirection languageDirection = provider.GetLanguageDirection(_languagePair);
                    IList<Sdl.LanguagePlatform.TranslationMemory.SearchResults> providerResult = languageDirection.SearchTranslationUnits(settings, translationUnits);

                    MergeProviderSearchResults(result, providerResult);
                }
            }

            return result;
        }

        public LanguagePlatform.TranslationMemory.SearchResults[] SearchTranslationUnitsMasked(LanguagePlatform.TranslationMemory.SearchSettings settings, LanguagePlatform.TranslationMemory.TranslationUnit[] translationUnits, bool[] mask)
        {
            if (translationUnits == null)
                throw new ArgumentNullException("translationUnits");
            if (mask == null)
                throw new ArgumentNullException("mask");
            if (mask.Length != translationUnits.Length)
                throw new ArgumentException("The length of the mask parameter should match the number translation units.", "mask");

            Sdl.LanguagePlatform.TranslationMemory.SearchResults[] result
               = new SearchResults[translationUnits.Length];

            if (DisableMT) return result;

            foreach (var provider in _provider.MTProviders)
            {
                if (SearchShouldExecute(provider, settings))
                {
                    ITranslationProviderLanguageDirection languageDirection = provider.GetLanguageDirection(_languagePair);
                    IList<Sdl.LanguagePlatform.TranslationMemory.SearchResults> providerResult = languageDirection.SearchTranslationUnitsMasked(settings, translationUnits, mask);

                    MergeProviderSearchResults(result, providerResult);
                }
            }
           
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

        public LanguagePlatform.TranslationMemory.ImportResult UpdateTranslationUnit(LanguagePlatform.TranslationMemory.TranslationUnit translationUnit)
        {
            return null;
        }

        public LanguagePlatform.TranslationMemory.ImportResult[] UpdateTranslationUnits(LanguagePlatform.TranslationMemory.TranslationUnit[] translationUnits)
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

        private bool AddShouldExecute(ITranslationProvider provider, ImportSettings settings)
        {
            if (provider.IsReadOnly)
            {
                return false;
            }
            if (!provider.SupportsUpdate)
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

    }
}
