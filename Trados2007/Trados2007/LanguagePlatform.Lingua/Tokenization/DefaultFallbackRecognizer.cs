using System;
using System.Collections.Generic;
using System.Text;
using Sdl.LanguagePlatform.Core.Tokenization;

// disable warnings about missing XML comments for internal classes:
// #pragma warning disable 1591

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	internal class DefaultFallbackRecognizer : Recognizer
	{
		// TODO do we need to know the culture we're in, e.g. to create specific date/time patterns in the FB tokenizer?
		protected System.Globalization.CultureInfo _Culture;
		private Trie<char, int> _LeadingClitics = null;
		private Resources.LanguageResources _Resources;

		public DefaultFallbackRecognizer(Core.Tokenization.TokenType t, int priority, System.Globalization.CultureInfo culture, Core.Resources.IResourceDataAccessor dataAccessor, bool separateClitics)
			: base(t, priority, null, "DefaultFallbackRecognizer")
		{
			// NOTE the token type is ignored in the implementation as the fallback recognizer will deliver multiple token types
			_Culture = culture;
			if (dataAccessor != null)
				_Resources = new Sdl.LanguagePlatform.Lingua.Resources.LanguageResources(_Culture, dataAccessor);

			_IsFallbackRecognizer = true;

			// TODO test performance of building up the trie and matching instead fo using StartsWith() on 
			//  the list of clitics
			HashSet<string> leadingClitics = Core.CultureInfoExtensions.GetLeadingClitics(_Culture);
			if (leadingClitics != null)
			{
				int p = 0;
				_LeadingClitics = new Trie<char, int>();
				foreach (string s in leadingClitics)
					_LeadingClitics.Add(s.ToCharArray(), p++);
			}
		}

		public override Core.Tokenization.Token Recognize(string s, int from, bool allowTokenBundles, ref int consumedLength)
		{
			/*
			 * TODO handle some special cases, e.g.
			 * 
			 * "--" in en-US
			 * "...." etc. in mid-words
			 * "l-xxx" in Maltese (leading and trailing clitics)
			 * ta' in Maltese (word ends in non-sep punct)
			 * */

			consumedLength = 0;

			if (String.IsNullOrEmpty(s))
				return null;

			int len = s.Length;
			int pos = from;

			// check for leading whitespace
			while (pos < len && (System.Char.IsWhiteSpace(s, pos) || System.Char.IsSeparator(s, pos)))
				++pos;

			if (pos > from)
			{
				// found a whitespace token
				consumedLength = pos - from;
				Token t = new SimpleToken(s.Substring(from, consumedLength), TokenType.Whitespace);
				return t;
			}

			// initial hard token terminators: treat as punctuation token
			if (IsHardTokenTerminator(s, pos))
			{
				consumedLength = 1;
				Token t = new SimpleToken(s.Substring(from, consumedLength), TokenType.GeneralPunctuation);
				return t;
			}

			// clitics, if defined by the culture, are always separated
			if (_LeadingClitics != null)
			{
				TrieIterator<char, int> iter
					= _LeadingClitics.GetIterator();
				int cliticLength = 0;

				while (iter != null && pos + cliticLength < len && !iter.IsFinal)
				{
					if (!iter.Traverse(s[pos + cliticLength]))
						break;
					++cliticLength;
				}
				if (iter != null && iter.IsValid && iter.IsFinal)
				{
					// found a clitic
					consumedLength = cliticLength;
					Token t = new SimpleToken(s.Substring(from, cliticLength), TokenType.Word);
					return t;
				}
			}

			char c = s[pos];
			bool lastIsCJK = Core.CharacterProperties.IsCJKChar(c);
			while (pos < len 
				&& !(System.Char.IsWhiteSpace(c) || System.Char.IsSeparator(c) || IsHardTokenTerminator(s, pos)))
			{
				// don't step over critical script changes
				// NOTE default fallback tokenizer will return CJK sequences as one token while
				//  FE fallback tokenizer will split them into single-char char sequences.
				bool currentIsCJK = Core.CharacterProperties.IsCJKChar(c);
				if (currentIsCJK != lastIsCJK)
					break;

				++pos;
				if (pos < len)
				{
					c = s[pos];
					lastIsCJK = currentIsCJK;
				}
			}

			int upto = pos;

			// [from, upto[ is now the longest non-whitespace chain. Start separating leading punctuation, 
			// including full stops

			// TODO this will put ")." into one token. We may want to split it into two.

			for (pos = from; pos < upto && (IsSeparablePunct(s, pos) || s[pos] == '.'); ++pos)
				;

			if (pos > from)
			{
				// found a sequence of separable punctuation
				consumedLength = pos - from;
				Token t = new SimpleToken(s.Substring(from, consumedLength), TokenType.GeneralPunctuation);
				return t;
			}

			// token does not start with separable punctuation - remove separable punctuation from the end
			// and take care of trailing full stop and abbreviations

			// We need to catch situations like "...test)." - here, after the full stop is removed, we need to
			//  check for trailing closing punctuation again, and vice versa as in "test...)." and similar cases.
			bool separated;
			bool isAbbreviation = false;
			do
			{
				separated = false;

				// take care of trailing closing punctuation
				while (upto - 1 > pos && IsSeparablePunct(s, upto - 1))
				{
					--upto;
					separated = true;
				}

				// take care of full stop separation
				int trailingFullStops = 0;
				while (upto - 1 - trailingFullStops > pos && s[upto - 1 - trailingFullStops] == '.')
				{
					++trailingFullStops;
				}
				if (trailingFullStops > 1)
				{
					// ellipsis
					upto -= trailingFullStops;
					separated = true;
				}
				else if (trailingFullStops == 1)
				{
					// single trailing full stop - separate if we aren't looking at a known abbreviation.
					// TODO add abbreviation heuristics
					// TODO use specific token type for abbreviations?
					if (_Resources == null || !_Resources.IsAbbreviation(s.Substring(from, upto - from)))
					{
						--upto;
						separated = true;
					}
					else
						isAbbreviation = true;
				}
			} while (separated);

			// treat the remainder as a word
			consumedLength = upto - from;
			Token token = new SimpleToken(s.Substring(from, consumedLength), 
				isAbbreviation ? TokenType.Abbreviation : TokenType.Word);
			return token;
		}

		private bool IsSeparablePunct(string s, int pos)
		{
			// NOTE full stops '.' are UnicodeCategory.OtherPunctuation but we need special
			//  treatment as they may or may not be separated from a char sequence, depending
			//  on whether we see an abbreviation or not.

			System.Globalization.UnicodeCategory cat = System.Char.GetUnicodeCategory(s, pos);
			bool isPunct = (cat == System.Globalization.UnicodeCategory.OpenPunctuation
				|| cat == System.Globalization.UnicodeCategory.ClosePunctuation
				|| cat == System.Globalization.UnicodeCategory.FinalQuotePunctuation
				|| cat == System.Globalization.UnicodeCategory.InitialQuotePunctuation
				|| cat == System.Globalization.UnicodeCategory.MathSymbol
				|| (cat == System.Globalization.UnicodeCategory.OtherPunctuation && s[pos] != '.'));
			return isPunct;
		}

		/// <summary>
		/// Returns true iff the char at s[p] is not allowed inside a (simple) token
		/// </summary>
		private static bool IsHardTokenTerminator(string s, int p)
		{
			System.Globalization.UnicodeCategory cat
				= Char.GetUnicodeCategory(s, p);

			if (cat == System.Globalization.UnicodeCategory.SpaceSeparator
				|| cat == System.Globalization.UnicodeCategory.ParagraphSeparator
				|| cat == System.Globalization.UnicodeCategory.LineSeparator
				|| cat == System.Globalization.UnicodeCategory.OpenPunctuation
				|| cat == System.Globalization.UnicodeCategory.ClosePunctuation
				|| cat == System.Globalization.UnicodeCategory.FinalQuotePunctuation
				|| cat == System.Globalization.UnicodeCategory.InitialQuotePunctuation
				|| cat == System.Globalization.UnicodeCategory.MathSymbol
				)
				return true;

			// TODO switch to respective Unicode Properties?
			// Core.CharacterProperties.IsApostrophe, Core.CharacterProperties.IsColon, Core.CharacterProperties.IsSemicolon,
			char c = s[p];
			if (c == '/' || c == '\\' 
				|| Core.CharacterProperties.IsColon(c)
				|| Core.CharacterProperties.IsSemicolon(c))
				return true;

			return false;
		}

	}
}
