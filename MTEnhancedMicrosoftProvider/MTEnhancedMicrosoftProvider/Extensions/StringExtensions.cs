using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MTEnhancedMicrosoftProvider.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		///     Returns a string array that contains the substrings in this instance that are delimited by specified indexes.
		/// </summary>
		/// <param name="source">The original string.</param>
		/// <param name="index">An index that delimits the substrings in this string.</param>
		/// <returns>An array whose elements contain the substrings in this instance that are delimited by one or more indexes.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="index" /> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">An <paramref name="index" /> is less than zero or greater than the length of this instance.</exception>
		public static string[] SplitAt(this string source, params int[] index)
		{
			index = index.Distinct().OrderBy(x => x).ToArray();
			var output = new string[index.Length + 1];
			var pos = 0;
			for (int i = 0; i < index.Length; pos = index[i++])
			{
				output[i] = source.Substring(pos, index[i] - pos);
			}

			output[index.Length] = source.Substring(pos);
			return output;
		}

		public static string ReplaceCharacters(this string textToTranslate, MatchCollection matches)
		{
			var indexes = new List<int>();
			foreach (Match match in matches)
			{
				if (match.Index.Equals(0))
				{
					indexes.Add(match.Length);
					continue;
				}

				indexes.Add(match.Index);
				var remainingText = textToTranslate.Substring(match.Index + match.Length);
				if (!string.IsNullOrEmpty(remainingText))
				{
					indexes.Add(match.Index + match.Length);
				}
			}

			var splitText = textToTranslate.SplitAt(indexes.ToArray()).ToList();
			var positions = new List<int>();
			for (var i = 0; i < splitText.Count; i++)
			{
				if (splitText[i].Contains("tg"))
				{
					continue;
				}

				positions.Add(i);
			}

			foreach (var position in positions)
			{
				var originalString = splitText[position];
				var start = Regex.Replace(originalString, "<", "&lt;");
				var finalString = Regex.Replace(start, ">", "&gt;");
				splitText[position] = finalString;
			}

			return splitText.Aggregate(string.Empty, (current, text) => current + text);
		}

	}
}
