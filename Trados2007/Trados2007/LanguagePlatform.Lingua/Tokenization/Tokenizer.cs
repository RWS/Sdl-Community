using System;
using System.Collections.Generic;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System.Linq;

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	/// <summary>
	/// An engine which splits a stream of characters into a stream of tokens (usually words).
	/// </summary>
	public class Tokenizer
	{
		private TokenizerParameters _Parameters;

		public Tokenizer(TokenizerParameters parameters)
		{
			if (parameters == null)
				throw new ArgumentNullException("parameters");
			_Parameters = parameters;
		}

		public Tokenizer(TokenizerSetup setup)
			: this(new TokenizerParameters(setup, null))
		{
		}

		public Tokenizer(TokenizerSetup setup, Core.Resources.IResourceDataAccessor resourceDataAccessor)
			: this(new TokenizerParameters(setup, resourceDataAccessor))
		{
		}

		public TokenizerParameters Parameters
		{
			get { return _Parameters; }
		}

		public List<Core.Tokenization.Token> Tokenize(string s)
		{
			List<Core.Tokenization.Token> result
				= TokenizeInternal(s, 0, _Parameters.CreateWhitespaceTokens, false);

			ReclassifyAcronyms(result);

			return result;
		}

		public List<Core.Tokenization.Token> Tokenize(Core.Segment s)
		{
			return Tokenize(s, false);
		}

		public List<Core.Tokenization.Token> Tokenize(Core.Segment s, bool allowTokenBundles)
		{
			// TODO check whether segment culture is compatible with tokenizer parameter's culture? Or accept junk-in-junk-out?

			List<Token> result = new List<Token>();

			int run = -1;

			foreach (SegmentElement se in s.Elements)
			{
				++run;

				if (se == null)
				{
					System.Diagnostics.Debug.Assert(false, "empty segment run!");
					continue;
				}

				Text txtR = se as Text;
				Token tokR = se as Token;
				Tag tagR = se as Tag;

				if (tagR != null)
				{
					// TODO rather have a "markup token" type?
					Token t = new TagToken(tagR);
					t.Span = new SegmentRange(run, 0, 0);
					result.Add(t);
				}
				else if (tokR != null)
				{
					// partially pretokenized input
					// TODO duplicate token/deep copy?
					tokR.Span = new SegmentRange(run, 0, 0);
					result.Add(tokR);
				}
				else if (txtR != null)
				{
					List<Token> tokenized = TokenizeInternal(txtR.Value, run, _Parameters.CreateWhitespaceTokens, allowTokenBundles);
					if (tokenized != null && tokenized.Count > 0)
						result.AddRange(tokenized);
				}
				else
					System.Diagnostics.Debug.Assert(false, "Unknown segment run type");
			}

			ReclassifyAcronyms(result);

			return result;
		}

		private static bool CanBundle(TokenType t)
		{
			return t == TokenType.Date
				|| t == TokenType.Measurement
				|| t == TokenType.Number
				|| t == TokenType.Time;
		}

		private bool ReclassifyAcronyms(List<Core.Tokenization.Token> tokens)
		{
			if (!_Parameters.ReclassifyAcronyms)
				return false;

			bool hasAcroyms = tokens.Any(t => t.Type == TokenType.Acronym);
			if (!hasAcroyms)
				return false;

			int ucLetters = 0;
			int lcLetters = 0;
			int ncLetters = 0;
			int noLetters = 0;

			int words = 0;
			int abbreviations = 0;
			int acronyms = 0;
			int charSequences = 0;
			int wordlikeTokens = 0;

			foreach (Core.Tokenization.Token t in tokens)
			{
				switch (t.Type)
				{
				case TokenType.Word:
					++words;
					++wordlikeTokens;
					CountLetters(t.Text, ref ucLetters, ref lcLetters, ref ncLetters, ref noLetters);
					break;
				case TokenType.Abbreviation:
					++abbreviations;
					++wordlikeTokens;
					CountLetters(t.Text, ref ucLetters, ref lcLetters, ref ncLetters, ref noLetters);
					break;
				case TokenType.CharSequence:
					++charSequences;
					++wordlikeTokens;
					CountLetters(t.Text, ref ucLetters, ref lcLetters, ref ncLetters, ref noLetters);
					break;
				case TokenType.Acronym:
					++acronyms;
					++wordlikeTokens;
					CountLetters(t.Text, ref ucLetters, ref lcLetters, ref ncLetters, ref noLetters);
					break;
				default:
					break;
				}
			}

			int totalLetters = ucLetters + lcLetters + ncLetters;
			bool result = false;

			// reclassification criterion - don't reclassify acronym-only segment
			if (wordlikeTokens > 1 && ucLetters > (lcLetters * 2))
			{
				foreach (Core.Tokenization.Token t in tokens)
				{
					if (t.Type == TokenType.Acronym 
                        && (t is Core.Tokenization.SimpleToken) )
					{
					    t.Type = TokenType.Word;
					}
				}
			}

			return result;
		}

		private static void CountLetters(string s, ref int upperCase, ref int lowerCase, ref int noCase, ref int noChar)
		{
			for (int p = 0; p < s.Length; ++p)
			{
				char c = s[p];
				if (Char.IsUpper(c))
					++upperCase;
				else if (Char.IsLower(c))
					++lowerCase;
				else if (Char.IsLetter(c))
					++noCase;
				else
					++noChar;
			}
		}

		private List<Core.Tokenization.Token> TokenizeInternal(string s,
			int currentRun,
			bool createWhitespaceTokens,
			bool allowTokenBundles)
		{
			List<Token> result = new List<Token>();

			int p = 0;
			int sLen = s.Length;
			while (p < sLen)
			{
				int start = p;

				while (p < sLen && System.Char.IsWhiteSpace(s, p))
					++p;

				if (p > start)
				{
					if (createWhitespaceTokens)
					{
						Token t = new SimpleToken(s.Substring(start, p - start), TokenType.Whitespace);
						t.Span = new SegmentRange(currentRun, start, p - 1);
						result.Add(t);
					}
					start = p;
				}
				if (p >= sLen)
					break;

				// test which recognizer claims the longest prefix

				Recognizer winningRecognizer = null;
				int winningLength = 0;
				Token winningToken = null;

				const bool allowBundlesOfDifferentType = false;

				for (int r = 0; r < _Parameters.Count; ++r)
				{
					Recognizer rec = _Parameters[r];
					int consumedLength = 0;
					Token t = rec.Recognize(s, start, allowTokenBundles, ref consumedLength);

					if (t != null)
					{
						if (winningRecognizer == null
							|| (winningLength < consumedLength && !(winningRecognizer.OverrideFallbackRecognizer && rec.IsFallbackRecognizer)))
						{
							winningToken = t;
							winningRecognizer = rec;
							winningLength = consumedLength;
							p = start + consumedLength;
						}
						else if (allowTokenBundles && allowBundlesOfDifferentType)
						{
							Core.Tokenization.TokenBundle winningBundle
								= winningToken as Core.Tokenization.TokenBundle;

							if (winningBundle == null)
							{
								winningBundle = new TokenBundle(winningToken, winningRecognizer.Priority);
								winningToken = winningBundle;
							}
							else
								winningBundle.Add(t, winningRecognizer.Priority);

							System.Diagnostics.Debug.Assert(winningLength == consumedLength);
							System.Diagnostics.Debug.Assert(p == start + consumedLength);
						}
						else if (winningRecognizer.Priority < rec.Priority)
						{
							// same length, but lower priority - highest prio wins
							winningToken = t;
							winningRecognizer = rec;
							winningLength = consumedLength;
							p = start + consumedLength;
						}
					}
				}

				if (winningToken == null)
				{
					// none of the recognizers claimed any input, or there were no recognizers set up.
					// ultimate fallback required: group by same Unicode category
					// TODO scanning on just the category is too fine - we may want to group coarser categories together
					System.Globalization.UnicodeCategory cat = System.Char.GetUnicodeCategory(s, start);
					while (p < sLen && System.Char.GetUnicodeCategory(s, p) == cat)
						++p;
					winningLength = p - start;
					// TODO distinguish result token type depending on the category
					winningToken = new SimpleToken(s.Substring(start, p - start), TokenType.Word);
					winningRecognizer = null;
				}
				else if (winningToken is TokenBundle)
				{
					// convert single-element token bundles to single tokens
					TokenBundle tb = winningToken as TokenBundle;
					if (tb.Count == 1)
						winningToken = tb[0].Token;
				}

				System.Diagnostics.Debug.Assert(winningLength > 0);
				System.Diagnostics.Debug.Assert(winningToken != null);

				winningToken.Span = new SegmentRange(currentRun, start, p - 1);

				result.Add(winningToken);
			}

			return result;
		}
	}
}
