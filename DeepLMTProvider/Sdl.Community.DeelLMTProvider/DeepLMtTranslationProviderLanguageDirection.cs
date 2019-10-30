using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System.Globalization;
using System.Threading.Tasks;
using System.Web;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Core.Globalization;
using Sdl.Community.DeepLMTProvider.WPF.Model;

namespace Sdl.Community.DeepLMTProvider
{
    public class DeepLMtTranslationProviderLanguageDirection : ITranslationProviderLanguageDirection
    {
		private readonly DeepLMtTranslationProvider _deepLMtTranslationProvider;
		private readonly DeepLTranslationOptions _options;
		private readonly LanguagePair _languageDirection;
		private TranslationUnit _inputTu;
		private DeepLTranslationProviderConnecter _deeplConnect;
	    private readonly NormalizeSourceTextHelper _normalizeSourceTextHelper;


		public DeepLMtTranslationProviderLanguageDirection(DeepLMtTranslationProvider deepLMtTranslationProvider, LanguagePair languageDirection)
		{
			_deepLMtTranslationProvider = deepLMtTranslationProvider;
			_languageDirection = languageDirection;
			_options = deepLMtTranslationProvider.Options;
			_normalizeSourceTextHelper = new NormalizeSourceTextHelper();
		}

		public ITranslationProvider TranslationProvider => _deepLMtTranslationProvider;

        public CultureInfo SourceLanguage => _languageDirection.SourceCulture;

        public CultureInfo TargetLanguage => _languageDirection.TargetCulture;

        public bool CanReverseLanguageDirection => throw new NotImplementedException();

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

		    // if there are match in tm the provider will not search the segment
		    #region "Confirmation Level"

		    if (!_options.ResendDrafts && _inputTu.ConfirmationLevel != ConfirmationLevel.Unspecified)
		    {
			    translation.Add(PluginResources.TranslationLookupDraftNotResentMessage);
			    //later get these strings from resource file
			    results.Add(CreateSearchResult(segment, translation));
			    return results;
		    }
		    var newseg = segment.Duplicate();
		    if (newseg.HasTags)
		    {
			    var tagPlacer = new DeepLTranslationProviderTagPlacer(newseg);
			    var translatedText = LookupDeepl(tagPlacer.PreparedSourceText);
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
			    var translatedText = LookupDeepl(sourcetext);
			    if (!string.IsNullOrEmpty(translatedText))
			    {
				    translation.Add(translatedText);

				    results.Add(CreateSearchResult(newseg, translation));
				    return results;
			    }

		    }
		    #endregion

		    return results;
	    }

	    private string LookupDeepl(string sourcetext)
		{
			if (_deeplConnect == null)
			{
				_deeplConnect = new DeepLTranslationProviderConnecter(_options.ApiKey, _options.Identifier);
			}
			else
			{
				_deeplConnect.ApiKey = _options.ApiKey;
			}

			var translatedText = _deeplConnect.Translate(_languageDirection, sourcetext);
			return translatedText;
		}

		private SearchResult CreateSearchResult(Segment segment, Segment translation)
		{
			#region "TranslationUnit"
			var tu = new TranslationUnit
			{
				SourceSegment = segment.Duplicate(),//this makes the original source segment, with tags, appear in the search window
				TargetSegment = translation
			};
			#endregion

			tu.ResourceId = new PersistentObjectToken(tu.GetHashCode(), Guid.Empty);
			
			//maybe this we need to add the score which Christine  requested
			//
			var score = 0; //score to 0...change if needed to support scoring
			tu.Origin = TranslationUnitOrigin.MachineTranslation;
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
			_inputTu = translationUnit;
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
					if (mask == null || mask[i])
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
					if (mask == null || mask[i])
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
					if (newSeg.HasTags)
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

	    private async Task<List<PreTranslateSegment>> PrepareTempData(List<PreTranslateSegment> preTranslatesegments)
	    {
			try
			{
				for (var i = 0; i < preTranslatesegments.Count; i++)
				{
					if (preTranslatesegments[i] != null)
					{
						string sourceText;
						var newseg = preTranslatesegments[i].TranslationUnit.SourceSegment.Duplicate();

						if (newseg.HasTags)
						{
							var tagPlacer = new DeepLTranslationProviderTagPlacer(newseg);
							sourceText = tagPlacer.PreparedSourceText;
						}
						else
						{
							sourceText = newseg.ToPlain();
						}

						preTranslatesegments[i].SourceText = sourceText;
					}
				}


			  var translator = new DeepLTranslationProviderConnecter(_options.ApiKey, _options.Identifier);

				await Task.Run(() => Parallel.ForEach(preTranslatesegments, segment =>
				{
					if (segment != null)
					{
						segment.PlainTranslation = translator.Translate(_languageDirection, segment.SourceText);
					}
				})).ConfigureAwait(true);

			    return preTranslatesegments;
		    }
		    catch (Exception e)
		    {
			    Console.WriteLine(e);
		    }
		    return preTranslatesegments;
		}

	    public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
        {
            throw new NotImplementedException();
        }

        public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
        {
            throw new NotImplementedException();
        }
    }
}
