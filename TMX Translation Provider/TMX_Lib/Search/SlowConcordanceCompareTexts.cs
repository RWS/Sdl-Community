using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMX_Lib.Utils;

namespace TMX_Lib.Search
{
	public class SlowConcordanceCompareTexts
	{
		private static bool EnoughWords(int searchForWordCount, int foundTextContainsWordCount)
		{
			if (searchForWordCount >= 15)
				return foundTextContainsWordCount >= searchForWordCount - 4;
			if (searchForWordCount >= 8)
				return foundTextContainsWordCount >= searchForWordCount - 3;
			if (searchForWordCount >= 5)
				return foundTextContainsWordCount >= searchForWordCount - 2;
			if (searchForWordCount >= 3)
				return foundTextContainsWordCount >= searchForWordCount - 1;
			return searchForWordCount == foundTextContainsWordCount;
		}

		// care about:
		// order of all words in searchFor
		// how close words are to each other
		private static double OrderPenalty(string searchFor, string foundText)
		{
			var searchForWords = searchFor.Split(Constants.WORD_DELIMITERS, StringSplitOptions.RemoveEmptyEntries);
			var foundTextWords = foundText.Split(Constants.WORD_DELIMITERS, StringSplitOptions.RemoveEmptyEntries).ToList();

			/* Penalties
				- case insensitive penalty 
					- if found word doesn't contain the exact case
				- order penalty
					- for each word that isn't found in the correct order
				- closeness-of-words penalty
					- for each word that is too far from the previous word
				- word missing
			 */
			const double CaseInsensitivePenalty = 0.005;
			const double OrderPenalty = 0.04;
			const double FarPenalty = 0.01;
			const double TooFarPenalty = 0.03;
			const double WordMissingPenalty = 0.08;

			var penalty = 0d;
			var lastIndex = 0;
			foreach (var word in searchForWords)
			{
				var foundAnywhereIdx = foundTextWords.FindIndex(w => w.Equals(word, StringComparison.InvariantCultureIgnoreCase));
				if (foundAnywhereIdx < 0)
				{
					penalty += WordMissingPenalty;
					continue;
				}

				var skipIdx = 0;
				var foundIdx = foundTextWords.FindIndex(w => skipIdx++ >= lastIndex && w.Equals(word, StringComparison.InvariantCultureIgnoreCase));
				if (foundIdx < 0)
				{
					penalty += OrderPenalty;
					lastIndex = foundAnywhereIdx + 1;
					continue;
				}

				var closenessPenalty = foundIdx > lastIndex ? (foundIdx - lastIndex > 2 ? TooFarPenalty : FarPenalty) : 0d;
				penalty += closenessPenalty;
				if (!foundTextWords[foundIdx].Equals(word, StringComparison.InvariantCulture))
					penalty += CaseInsensitivePenalty;
				lastIndex = foundIdx + 1;
			}

			return penalty;
		}


		// returns a 0-1 score (0-least , 1-best)
		public static double Compare(string searchFor, string foundText)
		{
			string searchForPunct, foundTextPunct;
			(searchFor, searchForPunct) = SlowCompareUtil. RemovePunctuation(searchFor);
			(foundText, foundTextPunct) = SlowCompareUtil.RemovePunctuation(foundText);

			var searchForWords = SlowCompareUtil.StrToDictioary(searchFor);
			var foundTextWords = SlowCompareUtil.StrToDictioary(foundText);

			var searchForWordCount = searchForWords.Sum(w => w.Value);
			var foundTextContainsWordCount = SlowCompareUtil.StrDictionaryCount(foundTextWords, searchForWords);
			if (!EnoughWords(searchForWordCount, foundTextContainsWordCount))
				return 0;

			var result = 1d;
			if (searchForPunct != foundTextPunct)
				// if not same punctuation -> remove 4%
				result = Math.Max(result - 0.04, 0);

			result = Math.Max(result - OrderPenalty(searchFor, foundText), 0);
			return result;
		}
	}
}
