using System;
using System.Collections.Generic;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;

// disable warnings about missing XML comments for internal classes:
// #pragma warning disable 1591

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	internal class RegexRecognizerPattern
	{
		private System.Text.RegularExpressions.Regex _Regex;
		private CharacterSet _First;
		private int _Priority;

		public RegexRecognizerPattern(System.Text.RegularExpressions.Regex rx)
			: this(rx, null, 0)
		{
		}

		public RegexRecognizerPattern(System.Text.RegularExpressions.Regex rx,
			CharacterSet first)
			: this(rx, first, 0)
		{
		}

		public RegexRecognizerPattern(System.Text.RegularExpressions.Regex rx,
			CharacterSet first, int priority)
		{
			_Regex = rx;
			_First = first;
			_Priority = priority;
		}

		public System.Text.RegularExpressions.Regex Regex
		{
			get { return _Regex; }
			set { _Regex = value; }
		}

		public CharacterSet First
		{
			get { return _First; }
			set { _First = value; }
		}

		public int Priority
		{
			get { return _Priority; }
			set { _Priority = value; }
		}

		public string Pattern
		{
			get { return _Regex == null ? null : _Regex.ToString(); }
		}
	}

	internal class RegexRecognizer : Recognizer
	{
		protected List<RegexRecognizerPattern> _Patterns;

		public RegexRecognizer(Core.Tokenization.TokenType t, int priority, string tokenClassName, string recognizerName)
			: this(t, priority, tokenClassName, recognizerName, false)
		{
		}

		public RegexRecognizer(Core.Tokenization.TokenType t, int priority, string tokenClassName, string recognizerName, bool autoSubstitutable)
			: base(t, priority, tokenClassName, recognizerName, autoSubstitutable)
		{
			// TODO allow passing regex options?
			_Patterns = new List<RegexRecognizerPattern>();
		}

		public void Add(string rxPattern, CharacterSet first)
		{
			Add(rxPattern, first, 0);
		}

		public void Add(string rxPattern, CharacterSet first, bool caseInsensitive)
		{
			Add(rxPattern, first, 0, caseInsensitive);
		}

		public void Add(string rxPattern, CharacterSet first, int priority)
		{
			Add(rxPattern, first, priority, false);
		}

		public void Add(string rxPattern, CharacterSet first, int priority, bool caseInsensitive)
		{
			System.Text.RegularExpressions.RegexOptions options
				= System.Text.RegularExpressions.RegexOptions.CultureInvariant
					| System.Text.RegularExpressions.RegexOptions.ExplicitCapture;

			if (caseInsensitive)
				options |= System.Text.RegularExpressions.RegexOptions.IgnoreCase;

			if (String.IsNullOrEmpty(rxPattern))
				throw new ArgumentNullException("rxPattern");
			Add(new System.Text.RegularExpressions.Regex(rxPattern, options), first, priority);
		}

		public void Add(System.Text.RegularExpressions.Regex rx, CharacterSet first, int priority)
		{
			if (rx == null)
				throw new ArgumentNullException("rx");

			// avoid identical duplicates:
			string p = rx.ToString();
			foreach (RegexRecognizerPattern rxp in _Patterns)
			{
				if (String.Equals(p, rxp.Pattern, StringComparison.Ordinal))
					return;
			}

			RegexRecognizerPattern pattern = new RegexRecognizerPattern(rx, first, priority);
			_Patterns.Add(pattern);
		}

		protected virtual Core.Tokenization.Token CreateToken(string s,
			System.Text.RegularExpressions.GroupCollection groups)
		{
			Token t = null;

			if (_Type == TokenType.OtherTextPlaceable)
			{
				t = new GenericPlaceableToken(s, TokenClassName, _AutoSubstitutable);
			}
			else
			{
				t = new SimpleToken(s, _Type);
			}
			return t;
		}

		public override Core.Tokenization.Token Recognize(string s, int from, bool allowTokenBundles, ref int consumedLength)
		{
			// TODO this could test whether the match is followed by whitespace or non-word characters (punctuation etc) - 
			//  that would be simpler than inlcuding the respective constraints in the RX

			Token winner = null;
			int winningLength = 0;
			int winningPriority = 0;

			for (int p = 0; p < _Patterns.Count; ++p)
			{
				System.Text.RegularExpressions.Regex rx = _Patterns[p].Regex;
				CharacterSet first = _Patterns[p].First;

				// NOTE if the requirement that m.Index == from is dropped, we cannot use FIRST any more
				if (first != null && from < s.Length && !first.Contains(s[from]))
					continue;

				System.Text.RegularExpressions.Match m = rx.Match(s, from);
				if (m != null && m.Success && m.Index == from)
				{
					if (VerifyContextConstraints(s, m.Index + m.Value.Length))
					{
						Token t = CreateToken(m.Value, m.Groups);
						// TODO set other token values?
						if (t != null && m.Length > 0)
						{
							// longest wins, if two matches are found with equal length, the one with
							//  higher prio wins, or if both have same prio, first match wins
							if ((m.Length > winningLength)
								|| winner == null 
								|| (m.Length == winningLength && _Patterns[p].Priority > winningPriority && !allowTokenBundles))
							{
								winningLength = m.Length;
								winner = t;
								winningPriority = _Patterns[p].Priority;
							}
							else if (allowTokenBundles && m.Length == winningLength)
							{
								if (!(winner is TokenBundle))
								{
									winner = new TokenBundle(winner, winningPriority);
								}

								((TokenBundle)winner).Add(t, _Patterns[p].Priority);
								winningPriority = Math.Max(winningPriority, _Patterns[p].Priority);
							}
						}
					}
				}
			}

			if (winner != null)
			{
				consumedLength = winningLength;
				return winner;
			}
			else
				return null;
		}
	}
}
