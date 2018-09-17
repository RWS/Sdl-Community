using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System.Globalization;
using System.Windows.Forms;
using Sdl.Core.Globalization;
using Sdl.Community.DeelLMTProvider;
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

		public DeepLMtTranslationProviderLanguageDirection(DeepLMtTranslationProvider deepLMtTranslationProvider, LanguagePair languageDirection)
		{
			_deepLMtTranslationProvider = deepLMtTranslationProvider;
			_languageDirection = languageDirection;
			_options = deepLMtTranslationProvider.Options;
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
			var results = new SearchResults()
			{
				SourceSegment = segment.Duplicate()
			};

			// if there are match in tm the provider will not search the segment
			#region "Confirmation Level"
			if ( !_options.ResendDrafts &&_inputTu.ConfirmationLevel != ConfirmationLevel.Unspecified) 
			{
				translation.Add(PluginResources.TranslationLookupDraftNotResentMessage);
				//later get these strings from resource file
				results.Add(CreateSearchResult(segment, translation, segment.ToString()));
				return results;
			}
	        var newseg = segment.Duplicate();
	        if (newseg.HasTags)
	        {
		        var tagPlacer = new DeepLTranslationProviderTagPlacer(newseg);
		        var translatedText = LookupDeepl(tagPlacer.PreparedSourceText);
		         translation = tagPlacer.GetTaggedSegment(translatedText);

		        results.Add(CreateSearchResult(newseg, translation, newseg.ToPlain()));
		        return results;
			}
	        else
	        {

		        var sourcetext = newseg.ToPlain();

		        var translatedText = LookupDeepl(sourcetext);
		        translation.Add(translatedText);

		        results.Add(CreateSearchResult(newseg, translation, newseg.ToPlain()));
		        return results;
	        }

	        #endregion
		}

		private string LookupDeepl(string sourcetext)
		{
			if (_deeplConnect == null)
			{
				_deeplConnect = new DeepLTranslationProviderConnecter(_options.ApiKey);
			}
			else
			{
				_deeplConnect.ApiKey = _options.ApiKey;
			}

			var translatedText = _deeplConnect.Translate(_languageDirection, sourcetext);
			return translatedText;
		}

		private SearchResult CreateSearchResult(Segment segment, Segment translation, string v)
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

        public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
        {
			// Bug LG-15128 where mask parameters are true for both CM and the actual TU to be updated which cause an unnecessary call for CM segment
	        var results = new List<SearchResults>();
	     
				var i = 0;
	        foreach (var tu in translationUnits)
	        {
		        if (mask == null || mask[i])
		        {
			        var result = SearchTranslationUnit(settings, tu);
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
