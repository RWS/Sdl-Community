using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using dtSearch.Engine;
using Sdl.Community.DtSearch4Studio.Provider.Helpers;
using Sdl.Community.DtSearch4Studio.Provider.Model;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.DtSearch4Studio.Provider.Service
{
	public class SearchService
	{
		#region Private 
		private CultureInfo _sourceLanguage;
		private CultureInfo _targetLanguage;

		#endregion
		#region Public Properties
		public static readonly Log Log = Log.Instance;
		#endregion

		#region Constructors
		public SearchService(CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			_sourceLanguage = sourceLanguage;
			_targetLanguage = targetLanguage;
		}
		#endregion

		#region Public Methods		
		//public void GetResults(string indexPath, string segment)
		//{
		//	using (var searchJob = new SearchJob())
		//	{
		//		using (var searchResults = new SearchResults())
		//		{
		//			searchJob.Request = segment;
		//			searchJob.IndexesToSearch.Add(indexPath);
		//			searchJob.MaxFilesToRetrieve = 100;
		//			searchJob.SearchFlags = SearchFlags.dtsSearchTypeAnyWords;
		//			searchJob.Execute(searchResults);

		//			if (searchJob.Errors.Count > 0)
		//			{
		//				for (int i = 0; i <= searchJob.Errors.Count; i++)
		//				{
		//					Log.Logger.Error($"{Constants.GetResults}{i}: {searchJob.Errors.Message(i)}");
		//				}
		//			}
		//			else
		//			{
		//				var res = GetResultsList(searchResults);					
		//			}
		//		}
		//	}
		//}

		//public List<ResultsItem> GetResultsList(SearchResults searchResults)
		//{
		//	var items = new List<ResultsItem>();
		//	for (int i = 0; i < searchResults.Count; ++i)
		//	{
		//		searchResults.GetNthDoc(i);
		//		var item = new ResultsItem(searchResults.CurrentItem);
		//		item.OrdinalInSearchResults = i;
		//		items.Add(item);
		//	}
		//	return items;
		//}
		
		public List<WordItem> GetResults(string indexPath, string segment)
		{
			var wordListBuilder = new WordListBuilder();
			wordListBuilder.OpenIndex(indexPath);
			wordListBuilder.ListWords(segment, 4);

			var words = new List<WordItem>();
			for (int i = 0; i < wordListBuilder.Count; ++i)
			{
				var item = new WordItem();
				item.MakeFromWordListBuilder(wordListBuilder, i);
				if (item.HitCount > 0)
				{
					words.Add(item);
				}
			}
			return words;
		}

		public SearchResult CreateSearchResult(WordItem word, string sourceSegmentText)
		{
			var searchSegment = new Segment(_sourceLanguage);
			searchSegment.Add(sourceSegmentText);
			var translationSegment = new Segment(_targetLanguage);			
			translationSegment.Add(word.Word);

			var unit = new TranslationUnit
			{
				SourceSegment = searchSegment,
				TargetSegment = translationSegment,
				ConfirmationLevel = ConfirmationLevel.Translated,
				Origin = TranslationUnitOrigin.MachineTranslation
			};
			unit.ResourceId = new PersistentObjectToken(unit.GetHashCode(), Guid.Empty);
			var searchResult = new SearchResult(unit);

			// We do not currently support scoring, so always say that we're 25% sure on this translation.
			searchResult.ScoringResult = new ScoringResult() { BaseScore = 25 };

			return searchResult;		
		}

		#endregion
	}
}