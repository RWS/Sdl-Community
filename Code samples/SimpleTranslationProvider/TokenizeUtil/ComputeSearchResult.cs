using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTranslationProvider.TokenizeUtil
{
    public class ComputeSearchResult
    {
        private const double TOLERANCE = 0.00001;

        private TokenizeText _tokenizeText = new TokenizeText();
        private ComputeEditDistance _computeEditDistance = new ComputeEditDistance();

        public Segment CreateSourceSegment(string text) => _tokenizeText.CreateTokenizedSegment(text);

        // originalText - the source segment text
        // resultingText - what we actually translated (in the source language). This can be the same as the original text.
        // translatedText - what we translated, in the target language
        public SearchResult CreateSearchResult(string originalText, string resultingText, string translatedText, CultureInfo targetLanguage)
        {
            var sourceSegment = _tokenizeText.CreateTokenizedSegment(resultingText);

            var translation = new Segment(targetLanguage);
            translation.Add(translatedText);
            var tu = new TranslationUnit
            {
                SourceSegment = sourceSegment,
                TargetSegment = translation ,
            };

            tu.ResourceId = new PersistentObjectToken(tu.GetHashCode(), Guid.Empty);

            tu.Origin = TranslationUnitOrigin.Nmt;
            var distance = _computeEditDistance.Compute(originalText, resultingText);
            var searchResult = new SearchResult(tu)
            {
                ScoringResult = new ScoringResult
                {
                    BaseScore = (int)(distance.Score * 100),
                    EditDistance = distance,
                },
                TranslationProposal = tu,
            };

            tu.ConfirmationLevel = Math.Abs(distance.Score - 1) < TOLERANCE ? ConfirmationLevel.Translated : ConfirmationLevel.Draft;
            return searchResult;
        }
    }
}
