using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;
using Sdl.Community.DeepLMTProvider.Helpers;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.DeepLMTProvider.Studio
{
	public class DeepLMtTranslationProviderLanguageDirection : ITranslationProviderLanguageDirection
    {
        private readonly DeepLMtTranslationProvider _deepLMtTranslationProvider;
        private readonly LanguagePair _languageDirection;
        private readonly Logger _logger = Log.GetLogger(nameof(DeepLMtTranslationProviderLanguageDirection));
        private readonly DeepLTranslationOptions _options;
        private readonly DeepLTranslationProviderConnecter _connecter;

        public DeepLMtTranslationProviderLanguageDirection(DeepLMtTranslationProvider deepLMtTranslationProvider, LanguagePair languageDirection, DeepLTranslationProviderConnecter connecter)
        {
            _deepLMtTranslationProvider = deepLMtTranslationProvider;
            _languageDirection = languageDirection;
            _options = deepLMtTranslationProvider.Options;
            _connecter = connecter;
        }

        public bool CanReverseLanguageDirection => throw new NotImplementedException();

        public ITranslationProvider TranslationProvider => _deepLMtTranslationProvider;

		CultureCode ITranslationProviderLanguageDirection.SourceLanguage => _languageDirection.SourceCulture;

		CultureCode ITranslationProviderLanguageDirection.TargetLanguage => _languageDirection.TargetCulture;

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

        public SearchResults SearchSegment(SearchSettings settings, Segment segment)
        {
            var translation = new Segment(_languageDirection.TargetCulture);
            var results = new SearchResults
            {
                SourceSegment = segment.Duplicate()
            };

            try
            {
                var newseg = segment.Duplicate();
                if (newseg.HasTags && !_options.SendPlainText)
                {
                    var tagPlacer = new DeepLTranslationProviderTagPlacer(newseg);
                    var translatedText = LookupDeepL(tagPlacer.PreparedSourceText);
                    if (!string.IsNullOrEmpty(translatedText))
                    {
                        translation = tagPlacer.GetTaggedSegment(translatedText);

                        results.Add(CreateSearchResult(newseg, translation));
                        return results;
                    }
                }
                else
                {
                    var sourcetext = newseg.ToPlain();

                    var translatedText = LookupDeepL(sourcetext);
                    if (!string.IsNullOrEmpty(translatedText))
                    {
                        translation.Add(translatedText);

                        results.Add(CreateSearchResult(newseg, translation));
                        return results;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error($"SearchSegment method: {e.Message}\n {e.StackTrace}");
                throw;
            }
            return results;
        }

        public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
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

        public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
        {
            //need to use the tu confirmation level in searchsegment method
            return SearchSegment(settings, translationUnit.SourceSegment);
        }

        public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
        {
            throw new NotImplementedException();
        }

        public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits,
            bool[] mask)
        {
            // bug LG-15128 where mask parameters are true for both CM and the actual TU to be updated which cause an unnecessary call for CM segment

            var noOfResults = mask.Length;

            var results = new List<SearchResults>(noOfResults);
            var preTranslateList = new List<PreTranslateSegment>(noOfResults);

            for (int i = 0; i < noOfResults; i++)
            {
                results.Add(null);
                preTranslateList.Add(null);
            }

            // plugin is called from pre-translate batch task
            //we receive the data in chunk of 10 segments
            if (translationUnits.Length > 2)
            {
                var i = 0;
                foreach (var tu in translationUnits)
                {
                    if (mask[i])
                    {
                        var preTranslate = new PreTranslateSegment
                        {
                            SearchSettings = settings,
                            TranslationUnit = tu
                        };
                        preTranslateList.RemoveAt(i);
                        preTranslateList.Insert(i, preTranslate);
                    }
                    i++;
                }
                if (preTranslateList.Count > 0)
                {
                    //Create temp file with translations
                    var translatedSegments = PrepareTempData(preTranslateList).Result;
                    var preTranslateSearchResults = GetPreTranslationSearchResults(translatedSegments);

                    foreach (var result in preTranslateSearchResults)
                    {
                        if (result != null)
                        {
                            var index = preTranslateSearchResults.IndexOf(result);
                            results.RemoveAt(index);
                            results.Insert(index, result);
                        }
                    }
                }
            }
            else
            {
                var i = 0;
                foreach (var tu in translationUnits)
                {
                    if (mask[i])
                    {
                        var result = SearchTranslationUnit(settings, tu);
                        results.RemoveAt(i);
                        results.Insert(i, result);
                    }
                    i++;
                }
            }
            return results.ToArray();
        }

        public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
        {
            throw new NotImplementedException();
        }

        public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
        {
            throw new NotImplementedException();
        }

        private SearchResult CreateSearchResult(Segment segment, Segment translation)
        {
			var tu = new TranslationUnit
			{
				SourceSegment = segment.Duplicate(),//this makes the original source segment, with tags, appear in the search window
				TargetSegment = translation
			};

			tu.ResourceId = new PersistentObjectToken(tu.GetHashCode(), Guid.Empty);

            //maybe this we need to add the score which Christine  requested
            //
            var score = 0; //score to 0...change if needed to support scoring
            tu.Origin = TranslationUnitOrigin.Nmt;
            var searchResult = new SearchResult(tu)
            {
                TranslationProposal = new TranslationUnit(tu),
                ScoringResult = new ScoringResult
                {
                    BaseScore = score
                }
            };
            tu.ConfirmationLevel = ConfirmationLevel.Draft;

            return searchResult;
        }

        private List<SearchResults> GetPreTranslationSearchResults(List<PreTranslateSegment> preTranslateList)
        {
            var resultsList = new List<SearchResults>(preTranslateList.Capacity);

            for (int i = 0; i < resultsList.Capacity; i++)
            {
                resultsList.Add(null);
            }

            foreach (var preTranslate in preTranslateList)
            {
                if (preTranslate != null)
                {
                    var translation = new Segment(_languageDirection.TargetCulture);
                    var newSeg = preTranslate.TranslationUnit.SourceSegment.Duplicate();
                    if (newSeg.HasTags && !_options.SendPlainText)
                    {
                        var tagPlacer = new DeepLTranslationProviderTagPlacer(newSeg);

                        translation = tagPlacer.GetTaggedSegment(preTranslate.PlainTranslation);
                        preTranslate.TranslationSegment = translation;
                    }
                    else
                    {
                        translation.Add(preTranslate.PlainTranslation);
                    }

                    var searchResult = CreateSearchResult(newSeg, translation);
                    var results = new SearchResults
                    {
                        SourceSegment = newSeg
                    };
                    results.Add(searchResult);

                    var index = preTranslateList.IndexOf(preTranslate);
                    resultsList.RemoveAt(index);
                    resultsList.Insert(index, results);
                }
            }
            return resultsList;
        }

        private string LookupDeepL(string sourceText)
        {
            return _connecter.Translate(_languageDirection, sourceText);
        }

        private async Task<List<PreTranslateSegment>> PrepareTempData(List<PreTranslateSegment> preTranslateSegments)
        {
            try
            {
                for (var i = 0; i < preTranslateSegments.Count; i++)
                {
                    if (preTranslateSegments[i] != null)
                    {
                        string sourceText;
                        var newSeg = preTranslateSegments[i].TranslationUnit.SourceSegment.Duplicate();

                        if (newSeg.HasTags)
                        {
                            var tagPlacer = new DeepLTranslationProviderTagPlacer(newSeg);
                            sourceText = tagPlacer.PreparedSourceText;
                        }
                        else
                        {
                            sourceText = newSeg.ToPlain();
                        }

                        preTranslateSegments[i].SourceText = sourceText;
                    }
                }

                await Task.Run(() => Parallel.ForEach(preTranslateSegments, segment =>
                {
                    if (segment != null)
                    {
                        segment.PlainTranslation = _connecter.Translate(_languageDirection, segment.SourceText);
                    }
                })).ConfigureAwait(true);

                return preTranslateSegments;
            }
            catch (Exception e)
            {
                _logger.Error($"{e.Message}\n {e.StackTrace}");
            }

            preTranslateSegments.ForEach(seg =>
            {
                if (seg.PlainTranslation == null)
                    seg.PlainTranslation = string.Empty;
            });
            return preTranslateSegments;
        }
    }
}