using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMX_TranslationProvider.Search.Compare;
using TMX_TranslationProvider.Utils;

namespace TMX_TranslationProvider.Search.SearchSegment
{
	// represents the text we're searching for, optimized for comparison
	public class TextSegment
	{
		private const int SMALL_WORD_MAX_LEN = 3;
		

		private string _originalText;
		public string OriginalText => _originalText;

		// FIXME later -> formatting

		private IReadOnlyList<string> _words;
		private string _punctuation;

		// note: later : maybe care for numbers?

		// the idea - we care less about small words diffs than by big word diffs
		private HashSet<int> _wordsHashes ;
		private HashSet<int> _smallWordsHashes;

		private HashSet<int> _punctuationHashes;
		private HashSet<int> _placeablesHashes;

		public TextSegment(string originalText)
		{
			_originalText = originalText;
			ParseText();
		}

		private static string RemovePunctuation(string word, StringBuilder punctuation, List<string> placeablesList)
		{
			if (word.All(ch => char.IsPunctuation(ch) || char.IsDigit(ch) || char.IsSeparator(ch)))
			{
				// no letters in this word
				placeablesList.Add(word);
				return "";
			}

			StringBuilder noPunct = new StringBuilder();
			foreach (var ch in word)
				if (Char.IsPunctuation(ch))
					punctuation.Append(ch);
				else
					noPunct.Append(ch);
			return noPunct.ToString();
		}
		private void ParseText()
		{
			var punctuation = new StringBuilder();
			var placeablesList = new List<string>();
			_words = _originalText.Split(Constants.WORD_DELIMITERS, StringSplitOptions.RemoveEmptyEntries)
				.Select(w => RemovePunctuation(w, punctuation, placeablesList))
				.Where(w => w != "")
				.ToList();
			_punctuation = punctuation.ToString();

			_wordsHashes = new HashSet<int>(_words.Where(w => w.Length <= SMALL_WORD_MAX_LEN).Select(w => w.GetHashCode()));
			_smallWordsHashes = new HashSet<int>(_words.Where(w => w.Length > SMALL_WORD_MAX_LEN).Select(w => w.GetHashCode()));

			_punctuationHashes = new HashSet<int>(_punctuation.Select(Convert.ToInt32));
			_placeablesHashes = new HashSet<int>(placeablesList.Select(w => w.GetHashCode()));
		}

		// return a value 0 - 1
		// 0 - no match , 1 - full match
		private static double CompareHashes(HashSet<int> aSet, HashSet<int> bSet)
		{
			if (aSet.Equals(bSet))
				return 1;
			double max = Math.Max(aSet.Count, bSet.Count);
			if (aSet.Count == 0 || bSet.Count == 0)
			{
				// one is empty, one is not
				switch (max)
				{
					case 1: return 0.95;
					case 2: return 0.9;
					case 3: return 0.8;
					case 4: return 0.6;
				}
				return 0;
			}

			int inANotB = 0, inBNotA = 0;
			foreach (var a in aSet)
				if (!bSet.Contains(a))
					++inANotB;

			if (inANotB >= max / 2 && inANotB >= 2)
				return 0; // too many differences

			foreach(var b in bSet)
				if (!aSet.Contains(b))
					++inBNotA;

			if (inBNotA >= max / 2 && inBNotA >= 2)
				return 0; // too many differences

			var diff = Math.Min(((max - inANotB - inBNotA) * 1.2 / max), 0.9);
			return diff;
		}

		// in case of not 100% match...
		//
		// minPenalty -- the min penalty we can apply
		// maxPenalty -- the max penalty we can apply
		private static void CompareBigWordHashes(HashSet<int> a, HashSet<int> b, ref double minPenalty)
		{
			// min penalty - already initialized, we add to it
			var hasPenalty = 1 - CompareHashes(a, b);
			if (hasPenalty != 0)
				minPenalty += hasPenalty * 0.7;
		}

		// in case of not 100% match...
		//
		// minPenalty -- the min penalty we can apply
		// maxPenalty -- the max penalty we can apply
		private static void CompareSmallWordHashes(HashSet<int> a, HashSet<int> b, ref double minPenalty)
		{
			// min penalty - already initialized, we add to it
			var hasPenalty = 1 - CompareHashes(a, b);
			if (hasPenalty != 0)
				minPenalty += hasPenalty * 0.4;
		}
		// in case of not 100% match...
		//
		// minPenalty -- the min penalty we can apply
		// maxPenalty -- the max penalty we can apply
		private static void ComparePunctuationHashes(HashSet<int> a, HashSet<int> b, ref double minPenalty)
		{
			// min penalty - already initialized, we add to it
			var hasPenalty = 1 - CompareHashes(a, b);
			if (hasPenalty != 0)
				minPenalty += hasPenalty * 0.2;
		}
		// in case of not 100% match...
		//
		// minPenalty -- the min penalty we can apply
		// maxPenalty -- the max penalty we can apply
		private static void ComparePlaceablesHashes(HashSet<int> a, HashSet<int> b, ref double minPenalty)
		{
			// min penalty - already initialized, we add to it
			var hasPenalty = 1 - CompareHashes(a, b);
			if (hasPenalty != 0)
				minPenalty += hasPenalty * 0.1;
		}

		// returns a number 0-100 , 100=exact, 0= none
		public static int CompareScore(TextSegment a, TextSegment b, int minScore)
		{
			if (a.OriginalText == b.OriginalText)
				return 100;

			double minPenalty = 0;
			CompareBigWordHashes(a._wordsHashes, b._wordsHashes, ref minPenalty);
			CompareSmallWordHashes(a._smallWordsHashes, b._smallWordsHashes, ref minPenalty);
			ComparePunctuationHashes(a._punctuationHashes, b._punctuationHashes, ref minPenalty);
			ComparePlaceablesHashes(a._placeablesHashes, b._placeablesHashes, ref minPenalty);

			minPenalty *= 100;
			if (100 - minPenalty < minScore)
				// here, min penalty already higher than what user wants, no point in further processing
				return 0;

			// here, we could actually have a match, do full processing
			var score = SlowCompareTexts.Compare(a.OriginalText, b.OriginalText);
			return (int)(score * 100d);
		}

		public int ConcordanceSearchMatch(TextSegment fullText, int minScore)
		{
			// care about full word misses - create 2 dictionaries for the sourceText and unit.sourcelanguage
			// + care about numbers
			// + care about punctuation
			return 0;
		}
	}
}
