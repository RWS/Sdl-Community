using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Trados_AI_Essentials.Interface;
using Trados_AI_Essentials.Model;

namespace Trados_AI_Essentials.Service
{
    public class TranslationService
    {
        public ILCClient LCClient => Dependencies.LCClient;

        public SearchResults[] Translate(TranslationUnit[] translationUnits, bool[] mask, string sourceLanguage, string targetLanguage)
        {
            var results = new SearchResults[translationUnits.Length];

            for (int i = 0; i < translationUnits.Length; i++)
            {
                if (!mask[i])
                {
                    var tu = translationUnits[i];
                    var lcResult = LCClient.TranslateAsync(new TranslationRequest
                    {
                        IncludeUserResources = true,
                        SourceLanguage = sourceLanguage,
                        TargetLanguage = targetLanguage,
                        Source = tu.SourceSegment.ToPlain(),
                        TranslationProfileId = "68dbe0506169197b147e0531",
                        UserPrompt = "Provide a translation of the following 'Source' text to 'Italian' maintaining all of the formatting tags in the translation."
                    }).Result;
                    var searchResults = new SearchResults();

                    var targetSegment = new Segment(targetLanguage);
                    targetSegment.Add(lcResult.Translation);

                    var translationProposal = new TranslationUnit
                    {
                        SourceSegment = tu.SourceSegment.Duplicate(),
                        TargetSegment = targetSegment,
                        Origin = TranslationUnitOrigin.Nmt,
                        ConfirmationLevel = ConfirmationLevel.Draft,
                        DocumentSegmentPair = tu.DocumentSegmentPair
                    };

                    var searchResult = new SearchResult(translationProposal)
                    {
                        ScoringResult = new ScoringResult
                        {
                            BaseScore = 0
                        },
                        TranslationProposal = translationProposal
                    };

                    searchResults.Results.Add(searchResult);
                    results[i] = searchResults;
                }
                else
                {
                    results[i] = new SearchResults();
                }
            }

            return results;
        }
    }
}