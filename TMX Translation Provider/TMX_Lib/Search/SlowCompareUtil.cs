using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMX_Lib.Utils;

namespace TMX_Lib.Search
{
	internal class SlowCompareUtil
	{
		public static (string noPunct, string punct) RemovePunctuation(string a)
		{
			var punct = "";
			var noPunct = new string(a.Where(ch =>
			{
				if (Char.IsPunctuation(ch))
				{
					punct += ch;
					return false;
				}

				return true;
			}).ToArray());
			return (noPunct, punct);
		}

		public static Dictionary<string, int> StrToDictioary(string a)
		{
			var dic = new Dictionary<string, int>();
			foreach (var word in a.Split(Constants.WORD_DELIMITERS, StringSplitOptions.RemoveEmptyEntries))
			{
				if (dic.ContainsKey(word))
					dic[word]++;
				else
					dic.Add(word, 1);
			}

			return dic;
		}

		// returns how many words are contained in 'test'
		public static int StrDictionaryCount(Dictionary<string, int> test, Dictionary<string, int> words)
		{
			var count = 0;
			foreach (var word in words.Keys)
				if (test.TryGetValue(word, out var wordCount))
					count += wordCount;
			return count;
		}

		// returns a word penalty, based on the order of the words - if they don't match
		//
		// note: we won't return anything higher than twoConsecutiveMismatchesPenalty
		public static double OrderPenalty(string a, string b, double orderMismatchPenalty, double twoConsecutiveMismatchesPenalty)
		{
			// simple for now:
			// - 3% for each order mismatch (word missing or extra word or different word)
			// - 12% if two consecutive mismatches ("this is an interesting idea" vs "this interesting idea")
			var aWords = a.Split(Constants.WORD_DELIMITERS, StringSplitOptions.RemoveEmptyEntries);
			var bWords = b.Split(Constants.WORD_DELIMITERS, StringSplitOptions.RemoveEmptyEntries);
			var aIdx = 0;
			var bIdx = 0;
			var penalty = 0d;
			while (aIdx < aWords.Length && bIdx < bWords.Length)
			{
				if (a[aIdx] == b[bIdx])
				{
					aIdx++;
					bIdx++;
					continue;
				}

				penalty += orderMismatchPenalty;
				if (aIdx + 1 >= aWords.Length || bIdx + 1 >= bWords.Length)
					// can't compare against next word
					break;

				if (a[aIdx + 1] == b[bIdx + 1])
				{
					// word difference
					aIdx++;
					bIdx++;
					continue;
				}

				if (a[aIdx + 1] == b[bIdx])
				{
					aIdx++;
					continue;
				}

				if (a[aIdx] == b[bIdx + 1])
				{
					bIdx++;
					continue;
				}

				// two consecutive differences
				penalty = twoConsecutiveMismatchesPenalty;
				break;
			}

			return penalty == 0d ? penalty : Math.Max(penalty, twoConsecutiveMismatchesPenalty) ;
		}
	}
}
