using System;
using System.Collections.Generic;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace SampleTranslationProvider
{
    public class SampleTranslationProviderLanguageDirection : ITranslationProviderLanguageDirection
    {
        private readonly SampleTranslationProvider _provider;
        private readonly LanguagePair _languageDirection;

        public SampleTranslationProviderLanguageDirection(SampleTranslationProvider provider, LanguagePair languages)
        {
            _provider = provider;
            _languageDirection = languages;
        }

        public CultureCode SourceLanguage => _languageDirection.SourceCulture;

        public CultureCode TargetLanguage => _languageDirection.TargetCulture;

        public ITranslationProvider TranslationProvider => _provider;

        public SearchResults GetFakeSearchResults(SearchSettings settings, TranslationUnit tu)
        {
            var sourceDuplicate = tu.SourceSegment.Duplicate();
            SearchResults results = new SearchResults
            {
                SourceSegment = sourceDuplicate
            };


            if (settings.Mode == SearchMode.NormalSearch)
            {
                Segment translation = new Segment(_languageDirection.TargetCulture);

                foreach (var element in sourceDuplicate.Elements)
                {
                    translation.Add(element);
                }

                translation.Elements.Add(new Text(" (Translated)"));

                results.Add(CreateSearchResult(translation, sourceDuplicate.HasTags, tu));

            }

            return results;
        }

        private SearchResult CreateSearchResult(Segment translation,
            bool formattingPenalty, TranslationUnit translationUnit)
        {
            TranslationUnit tu = new TranslationUnit();

            tu.SourceSegment = translationUnit.SourceSegment.Duplicate();
            tu.TargetSegment = translation;

            // ===========================================================================================
            // IMPORTANT NOTE - DEMONSTRATION OF A KNOWN ISSUE (DET-725)
            // ===========================================================================================
            //
            // WHERE THE BUG OCCURS:
            // Assigning the incoming TranslationUnit's 'DocumentSegmentPair' onto the TranslationUnit
            // returned inside a SearchResult corrupts the source and target tags of the segment.
            // Issue reported here: https://rws-dev.atlassian.net/browse/DET-725
            //
            // The line below is the code that TRIGGERS the corruption. It is left commented out on
            // purpose - uncommenting it reproduces the tag corruption in Trados Studio:
            //
            //          tu.DocumentSegmentPair = translationUnit?.DocumentSegmentPair;   // <-- DO NOT DO THIS
            //
            // WHY DEVELOPERS REACH FOR IT:
            // A common reason to set 'DocumentSegmentPair' is to carry metadata back to the paragraph
            // unit (e.g. via 'DocumentSegmentPair.Properties.TranslationOrigin.MetaData').
            //
            // HOW TO AVOID IT (the workaround):
            // Do NOT set 'DocumentSegmentPair' on the returned TranslationUnit. If you need to surface
            // metadata, set it directly on the SearchResult instead. See below,
            // where 'searchResult.MetaData' is populated - that is the recommended, corruption-free approach.
            // ===========================================================================================


            tu.ResourceId = new PersistentObjectToken(tu.GetHashCode(), Guid.Empty);

            int score = 100;
            tu.Origin = TranslationUnitOrigin.TM;


            SearchResult searchResult = new SearchResult(tu)
            {
                ScoringResult = new ScoringResult()
            };
            searchResult.ScoringResult.BaseScore = score;

            if (formattingPenalty)
            {

                tu.ConfirmationLevel = ConfirmationLevel.Draft;

                Penalty penalty = new Penalty(PenaltyType.TagMismatch, 1);
                searchResult.ScoringResult.ApplyPenalty(penalty);

            }
            else
            {
                tu.ConfirmationLevel = ConfirmationLevel.Translated;
            }

            // RECOMMENDED APPROACH (see the note above, re: DET-725):
            // Rather than carrying metadata via the TranslationUnit's 'DocumentSegmentPair' - which
            // corrupts the source and target tags - set any metadata you need directly on the
            // SearchResult. This is safe and does not affect the segment's tags.
            searchResult.MetaData.Add("MyMetaData", "MyValue");


            return searchResult;
        }

        public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
        {
            List<SearchResults> results = new List<SearchResults>();

            int i = 0;
            foreach (var tu in translationUnits)
            {
                if (mask == null || mask[i])
                {
                    var result = GetFakeSearchResults(settings, tu);
                    results.Add(result);
                }
                else
                {
                    results.Add(null);
                }
                i++;
            }

            return results.ToArray();
        }

        public SearchResults SearchSegment(SearchSettings settings, Segment segment)
        {
            throw new NotImplementedException();
        }

        public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
        {
            throw new NotImplementedException();
        }

        public SearchResults SearchText(SearchSettings settings, string segment)
        {
            throw new NotImplementedException();
        }

        public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
        {
            throw new NotImplementedException();
        }

        public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
        {
            throw new NotImplementedException();
        }

        public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
        {
            throw new NotImplementedException();
        }
    }
}
