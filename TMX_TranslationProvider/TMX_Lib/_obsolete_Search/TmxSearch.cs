using System.Globalization;
using TMX_Lib._obsolete_Search.Result;
using TMX_Lib._obsolete_Search.SearchSegment;
using TMX_Lib.TmxFormat;

namespace TMX_Lib._obsolete_Search
{
	internal class TmxSearch : ISegmentPairSearch
	{
		private TmxParser _parser;

		private const int MAX_WORD_COUNT_FUZZY_SEARCH = 50000;
		private bool _canDoFuzzySearch;

		public TmxSearch(TmxParser parser)
		{
			_parser = parser;
			//_canDoFuzzySearch = _parser.TranslationUnits.Count <= MAX_WORD_COUNT_FUZZY_SEARCH;
		}

		public bool SupportsSourceLanguage(CultureInfo language)
		{
			// FIXME
			return true;
		}

		public bool SupportsTargetLanguage(CultureInfo language)
		{
			// FIXME
			return true;
		}

		// searchedText - the text I'm searching for
		// tmxText - the text I have internally
		private void ApplyPenalties(TextSegment searchedText, TextSegment tmxText)
		{
			// FIXME
		}

		public int SegmentPairCount() => 0;// _parser.TranslationUnits.Count;
		public SimpleResult TryTranslateExact(TextSegment sourceText, int segmentPairIndex, CultureInfo sourceLanguage, CultureInfo targetLaguage, int minScore)
		{
			return null;
			//if (segmentPairIndex < 0 || segmentPairIndex >= _parser.TranslationUnits.Count)
			//	throw new ArgumentException($"Invalid index {segmentPairIndex}, should be in [0,{SegmentPairCount()}] range");

			//var unit = _parser.TranslationUnits[segmentPairIndex];
			//if (!unit.HasLanguage(sourceLanguage) || !unit.HasLanguage(targetLaguage))
			//	return null;


			//SimpleResult result = null;
			//var tmxText = unit.Text(sourceLanguage);
			//if (tmxText.OriginalText == sourceText.OriginalText)
			//{
			//	result = unit.ToSimpleResult(sourceLanguage, targetLaguage);
			//	ApplyPenalties(sourceText, tmxText);
			//}

			// care about penalties 
			// TranslateTime -> Change or Create time

			//return result;
		}

		public SimpleResult TryTranslateFuzzy(TextSegment sourceText, int segmentPairIndex, CultureInfo sourceLanguage, CultureInfo targetLaguage, int minScore)
		{
			return null;
			//if (!_canDoFuzzySearch)
			//	return null;

			//if (segmentPairIndex < 0 || segmentPairIndex >= _parser.TranslationUnits.Count)
			//	throw new ArgumentException($"Invalid index {segmentPairIndex}, should be in [0,{SegmentPairCount()}] range");

			//var unit = _parser.TranslationUnits[segmentPairIndex];
			//if (!unit.HasLanguage(sourceLanguage) || !unit.HasLanguage(targetLaguage))
			//	return null;

			//SimpleResult result = null;
			//var tmxText = unit.Text(sourceLanguage);
			//if (TextSegment.CompareScore(sourceText, tmxText, minScore) >= minScore)
			//{
			//	result = unit.ToSimpleResult(sourceLanguage, targetLaguage);
			//	ApplyPenalties(sourceText, tmxText);
			//}

			//return result;
		}

		public SimpleResult TryTranslateConcordance(TextSegment sourceText, int segmentPairIndex, CultureInfo sourceLanguage, CultureInfo targetLaguage,
			bool sourceConcordance, int minScore)
		{
			return null;
			//if (segmentPairIndex < 0 || segmentPairIndex >= _parser.TranslationUnits.Count)
			//	throw new ArgumentException($"Invalid index {segmentPairIndex}, should be in [0,{SegmentPairCount()}] range");

			//var unit = _parser.TranslationUnits[segmentPairIndex];
			//if (!unit.HasLanguage(sourceLanguage) || !unit.HasLanguage(targetLaguage))
			//	return null;

			//var tmxText = unit.Text(sourceConcordance ? sourceLanguage : targetLaguage);
			//SimpleResult result = null;
			//if (sourceText.ConcordanceSearchMatch(tmxText, minScore) >= minScore)
			//{
			//	result = unit.ToSimpleResult(sourceLanguage, targetLaguage);
			//	ApplyPenalties(sourceText, tmxText);
			//}
			//return result;
		}



	}
}
