using System;
using System.Collections.Generic;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	internal class DefaultJapaneseFallbackRecognizer : DefaultFallbackRecognizer
	{
		private static string _DEFAULT_WORD_RX = "[\u30A0-\u30FF\uFF65-\uFF9F]+|[\u3040-\u3091\u3093-\u309F]+|[\u3092]|[\u4E00-\u9FFF]+|[\uFF21-\uFF3A\uFF41-\uFF5A]+";
		private static string _DEFAULT_PUNC_CS = "[\u3000-\u303F\u3200-\u32FF\uFF01-\uFF0F\uFF1A-\uFF20\uFF3B-\uFF3D\uFF5B-\uFF64]";

		private System.Text.RegularExpressions.Regex _DefaultWordRegex;
		private CharacterSet _DefaultPunctCharset;

		public DefaultJapaneseFallbackRecognizer(Core.Tokenization.TokenType t, int priority, System.Globalization.CultureInfo culture, Core.Resources.IResourceDataAccessor dataAccessor)
			: base(t, priority, culture, dataAccessor, false)
        {
			// TODO outsource the pattern into resource assembly, or make externally configurable?
			_DefaultWordRegex = new System.Text.RegularExpressions.Regex(_DEFAULT_WORD_RX, System.Text.RegularExpressions.RegexOptions.ExplicitCapture);
			int i = 0;
			_IsFallbackRecognizer = true;
			_DefaultPunctCharset = CharacterSetParser.Parse(_DEFAULT_PUNC_CS, ref i);
        }

		public override Core.Tokenization.Token Recognize(string s, int from, bool allowTokenBundles, ref int consumedLength)
		{
			if (String.IsNullOrEmpty(s) || from >= s.Length)
				return null;

			consumedLength = 0;
			int originalStart = from;

			if (_DefaultPunctCharset.Contains(s[from]))
			{
				while (from < s.Length && _DefaultPunctCharset.Contains(s[from]))
				{
					++consumedLength;
					++from;
				}
				Token t = new SimpleToken(s.Substring(originalStart, consumedLength), TokenType.GeneralPunctuation);
				return t;
			}

			System.Text.RegularExpressions.Match m = _DefaultWordRegex.Match(s, from);
			if (m != null && m.Success && m.Index == from)
			{
				consumedLength = m.Length;
				Token t = new SimpleToken(m.Value, TokenType.Word);
				return t;
			}
			
			/*
				AUTOMATON PUNCT [U+3000-U+303FU+3200-U+32FFU+FF01-U+FF0FU+FF1A-U+FF20U+FF3B-U+FF3DU+FF5B-U+FF64]
				NFA WORD [U+30A0-U+30FFU+FF65-U+FF9F]+
				NFA WORD [U+3040-U+3091U+3093-U+309F]+
				NFA WORD [U+3092]
				NFA WORD [U+4E00-U+9FFF]+
				NFA WORD [U+FF21-U+FF3AU+FF41-U+FF5A]+
			*/

			return base.Recognize(s, from, allowTokenBundles, ref consumedLength);
		}
	}
}

