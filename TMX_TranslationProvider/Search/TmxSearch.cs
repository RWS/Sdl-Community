using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using TMX_TranslationProvider.Search.Result;
using TMX_TranslationProvider.Search.SearchSegment;
using TMX_TranslationProvider.TmxFormat;

namespace TMX_TranslationProvider.Search
{
	internal class TmxSearch : ISegmentPairSearch
	{
		private TmxParser _parser;

		public TmxSearch(TmxParser parser)
		{
			_parser = parser;
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

		public int SegmentPairCount() => _parser.TranslationUnits.Count;
		public SimpleResult TryTranslateExact(TextSegment sourceText, int segmentPairIndex, CultureInfo sourceLanguage, CultureInfo targetLaguage, int minScore)
		{
			if (segmentPairIndex < 0 || segmentPairIndex >= _parser.TranslationUnits.Count)
				throw new ArgumentException($"Invalid index {segmentPairIndex}, should be in [0,{SegmentPairCount()}] range");

			var unit = _parser.TranslationUnits[segmentPairIndex];
			if (!unit.HasLanguage(sourceLanguage) || !unit.HasLanguage(targetLaguage))
				return null;


			SimpleResult result = null;
			var tmxText = unit.Text(sourceLanguage);
			if (tmxText.OriginalText == sourceText.OriginalText)
			{
				result = unit.ToSimpleResult(sourceLanguage, targetLaguage);
				ApplyPenalties(sourceText, tmxText);
			}

			// care about penalties 
			// TranslateTime -> Change or Create time

			// care about full word misses - create 2 dictionaries for the sourceText and unit.sourcelanguage
			// + care about numbers
			// + care about punctuation
			return result;
		}

		private SimpleResult TryTranslateFuzzyFullWordDifferencesOnly(TextSegment sourceText, int segmentPairIndex, CultureInfo sourceLanguage, CultureInfo targetLaguage, int minScore)
		{
			if (segmentPairIndex < 0 || segmentPairIndex >= _parser.TranslationUnits.Count)
				throw new ArgumentException($"Invalid index {segmentPairIndex}, should be in [0,{SegmentPairCount()}] range");

			var unit = _parser.TranslationUnits[segmentPairIndex];
			if (!unit.HasLanguage(sourceLanguage) || !unit.HasLanguage(targetLaguage))
				return null;

			SimpleResult result = null;
			var tmxText = unit.Text(sourceLanguage);
			if (TextSegment.CompareScore(sourceText, tmxText) >= minScore)
			{
				result = unit.ToSimpleResult(sourceLanguage, targetLaguage);
				ApplyPenalties(sourceText, tmxText);
			}

			return result;
		}

		public SimpleResult TryTranslateFuzzy(TextSegment sourceText, int segmentPairIndex, CultureInfo sourceLanguage, CultureInfo targetLaguage, int minScore)
		{
			if (segmentPairIndex < 0 || segmentPairIndex >= _parser.TranslationUnits.Count)
				throw new ArgumentException($"Invalid index {segmentPairIndex}, should be in [0,{SegmentPairCount()}] range");

			var unit = _parser.TranslationUnits[segmentPairIndex];
			if (!unit.HasLanguage(sourceLanguage) || !unit.HasLanguage(targetLaguage))
				return null;

			return TryTranslateFuzzyFullWordDifferencesOnly(sourceText, segmentPairIndex, sourceLanguage, targetLaguage, minScore);
		}

		public SimpleResult TryTranslateConcordance(TextSegment sourceText, int segmentPairIndex, CultureInfo sourceLanguage, CultureInfo targetLaguage,
			bool sourceConcordance, int minScore)
		{
			if (segmentPairIndex < 0 || segmentPairIndex >= _parser.TranslationUnits.Count)
				throw new ArgumentException($"Invalid index {segmentPairIndex}, should be in [0,{SegmentPairCount()}] range");

			var unit = _parser.TranslationUnits[segmentPairIndex];
			if (!unit.HasLanguage(sourceLanguage) || !unit.HasLanguage(targetLaguage))
				return null;

			var tmxText = unit.Text(sourceLanguage);
			SimpleResult result = null;
			// care about penalties 
			// TranslateTime -> Change or Create time

			// care about full word misses - create 2 dictionaries for the sourceText and unit.sourcelanguage
			// + care about numbers
			// + care about punctuation
			if (result != null)
				ApplyPenalties(sourceText, tmxText);
			return result;
		}



	}
}
