using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.LanguagePlatform.Core.Resources;
using System.Reflection;
using Sdl.LanguagePlatform.Core.EditDistance;
using SimpleTranslationProvider.TokenizeUtil;

namespace SimpleTranslationProvider
{
    /*
     * Sample of creating a simple translation provider, and be able to show differences between TUs
     *
     * Easiest way to test:
     * - Open the sample project from Trados Studio 2022.
     * - Open the SecondSample.docx.sdlxliff file
     * - play with the 1st, 2nd, and 4th segments
     * - do note that in our simple sample, you should ignore the translation itself. Please focus on what needs to be done on the source segments
     *
     * Important classes:
     * - ComputeSearchResult
     *   - you can use this class and be done with everything. It exposes CreateSourceSegment and CreateSearchResult, and that's all you need.
     *     You can look at the SearchSegments() function and see an example of usage
     * - ComputeEditDistance
     *   - computes the edit distance between 2 texts
     * - TokenizeText
     *   - converts a text into a Segment, along with Tokens (needed to be able to show differences between TUs)
     *
     */
    internal class MyTranslationProviderLanguageDirection : ITranslationProviderLanguageDirection
    {
        #region ITranslationProviderLanguageDirection Members

        private LanguagePair _languagePair;
        private MyTranslationProvider _translationProvider;

        private ComputeSearchResult _computeSearchResult = new ComputeSearchResult();

        public MyTranslationProviderLanguageDirection(LanguagePair languagePair, MyTranslationProvider translationProvider)
        {
            _languagePair = languagePair;
            _translationProvider = translationProvider;
        }

        public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
        {
            throw new NotImplementedException();
        }

        public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
        {
            throw new NotImplementedException();
        }

        public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
        {
            throw new NotImplementedException();
        }

        public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
        {
            throw new NotImplementedException();
        }

        public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
        {
            throw new NotImplementedException();
        }

        public bool CanReverseLanguageDirection
        {
            get { throw new NotImplementedException(); }
        }


        public SearchResults SearchSegment(SearchSettings settings, Segment segment)
        {
            var search = SearchSegments(settings, new[] { segment });
            return search.Length > 0 ? search[0] : null;
        }


        private string DummyTranslateText(string text)
        {
            var dummy = new StringBuilder();
            foreach (var ch in text)
                dummy.Append($"{ch}-");
            return dummy.ToString();
        }

        public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
        {
            var results = new SearchResults[segments.Length];
            for (var i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];
                if (segment == null)
                    segment = new Segment();

                results[i] = new SearchResults();
                
                var originalText = segment.ToPlain();
                var resultingText = originalText.Replace("National", "Nini").Replace("Safety", "Coolness")
                    .Replace("primary school teachers to participate", "teachers to incredibly partake");
                var translatedText = DummyTranslateText(originalText);

                var source = _computeSearchResult.CreateSourceSegment(originalText);
                results[i].SourceSegment = source;
                results[i].Add(_computeSearchResult.CreateSearchResult(originalText, resultingText, translatedText, TargetLanguage));
            }
            return results;
        }

        public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
        {
            return SearchSegments(settings, segments);
        }

        public SearchResults SearchText(SearchSettings settings, string segment)
        {
            throw new NotImplementedException();
        }

        public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
        {
            return SearchSegment(settings, translationUnit.SourceSegment);
        }

        public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
        {
            return SearchSegments(settings, translationUnits.Select(tu => tu.SourceSegment).ToArray());
        }

        public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
        {
            return SearchSegments(settings, translationUnits.Select(tu => tu.SourceSegment).ToArray());
        }

        public System.Globalization.CultureInfo SourceLanguage => _languagePair.SourceCulture;
        public System.Globalization.CultureInfo TargetLanguage => _languagePair.TargetCulture;
        public ITranslationProvider TranslationProvider => _translationProvider;

        public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
        {
            throw new NotImplementedException();
        }

        public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
