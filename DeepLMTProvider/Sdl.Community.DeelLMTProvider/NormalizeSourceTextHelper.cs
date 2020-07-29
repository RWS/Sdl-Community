﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Sdl.Community.DeepLMTProvider
{
	public class NormalizeSourceTextHelper
	{
		public string NormalizeText(string sourceText, bool removeTabs = true)
		{
			var rgx = new Regex("(\\<\\w+[üäåëöøßşÿÄÅÆĞ]*[^\\d\\W\\\\/\\\\]+\\>)");
			var words = rgx.Matches(sourceText);

			//For german and < > characters
			if (words.Count > 0)
			{
				var matchesIndexes = GetMatchesIndexes(sourceText, words);
				sourceText = ReplaceCharacters(matchesIndexes, sourceText);
			}

			// for < words >
			var shouldEncodeBrackets = ShouldEncodeBrackets(sourceText);
			if (shouldEncodeBrackets)
			{
				sourceText = EncodeBracket(sourceText);
			}

			//search for spaces
			if (!removeTabs)
			{
				var spacesCollection = ShouldNormalizeSpaces(sourceText);
				if (spacesCollection.Count > 0)
				{
					var matchesIndexes = GetMatchesIndexes(sourceText, spacesCollection);
					sourceText = NormalizeSpaces(matchesIndexes, sourceText);
				}
			}
			else
			{
				sourceText = Regex.Replace(sourceText, @"\s{2,}|\s", " ");
			}

			return Uri.EscapeDataString(sourceText);
		}

		private string EncodeBracket(string sourceText)
		{
			return HttpUtility.HtmlEncode(sourceText);
		}

		private int[] GetMatchesIndexes(string sourcetext, MatchCollection matches)
		{
			var indexes = new List<int>();
			foreach (Match match in matches)
			{
				if (match.Index.Equals(0))
				{
					indexes.Add(match.Length);
				}
				else
				{
					var remainingText = sourcetext.Substring(match.Index + match.Length);
					if (!string.IsNullOrEmpty(remainingText))
					{
						indexes.Add(match.Index);
						indexes.Add(match.Index + match.Length);
					}
					else
					{
						indexes.Add(match.Index);
					}
				}
			}
			return indexes.ToArray();
		}

		private string NormalizeSpaces(int[] matchesIndexes, string sourceText)
		{
			var spaceRgx = new Regex(@"\s{2,}");
			var finalText = new StringBuilder();
			var splitedText = sourceText.SplitAt(matchesIndexes).ToList();

			foreach (var text in splitedText)
			{
				var isSpace = spaceRgx.IsMatch(text);
				var containsTab = text.Contains('\t');
				if (isSpace)
				{
					var space = spaceRgx.Replace(text, containsTab ? "\t" : " ");

					finalText.Append(space);
				}
				else
				{
					finalText.Append(text);
				}
			}
			return finalText.ToString();
		}

		private string ReplaceCharacters(int[] indexes, string sourceText)
		{
			var splitedText = sourceText.SplitAt(indexes).ToList();
			var positions = new List<int>();
			for (var i = 0; i < splitedText.Count; i++)
			{
				if (!splitedText[i].Contains("tg"))
				{
					positions.Add(i);
				}
			}

			foreach (var position in positions)
			{
				var originalString = splitedText[position];
				var start = Regex.Replace(originalString, "<", "&lt;");
				var finalString = Regex.Replace(start, ">", "&gt;");
				splitedText[position] = finalString;
			}
			var finalText = string.Empty;
			foreach (var text in splitedText)
			{
				finalText += text;
			}

			return finalText;
		}

		private bool ShouldEncodeBrackets(string sourceText)
		{
			var isMatch = sourceText.Contains('<');
			if (isMatch)
			{
				var isTagSymbol = sourceText.Contains("tg");
				return !isTagSymbol;
			}
			return false;
		}

		private MatchCollection ShouldNormalizeSpaces(string sourceText)
		{
			var spaceRgx = new Regex(@"\s{2,}");
			return spaceRgx.Matches(sourceText);
		}
	}
}