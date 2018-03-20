using System;
using System.Collections.Generic;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	internal class DefaultChineseFallbackRecognizer : DefaultFallbackRecognizer
	{
		private static string _DEFAULT_PUNC_CS = "[\u3000-\u303F\u3200-\u32FF\uFF01-\uFF0F\uFF1A-\uFF20\uFF3B-\uFF3D\uFF5B-\uFF64]";
		private CharacterSet _DefaultPunctCharset;

		public DefaultChineseFallbackRecognizer(Core.Tokenization.TokenType t, int priority, System.Globalization.CultureInfo culture, Core.Resources.IResourceDataAccessor dataAccessor)
			: base(t, priority, culture, dataAccessor, false)
		{
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

			if (s[from] >= 0x4e00 && s[from] <= 0x9fff)
			{
				while (from < s.Length && s[from] >= 0x4e00 && s[from] <= 0x9fff)
				{
					++consumedLength;
					++from;
				}
				Token t = new SimpleToken(s.Substring(originalStart, consumedLength), TokenType.CharSequence);
				return t;
			}

			// TODO CJK punctuation etc.

			return base.Recognize(s, from, allowTokenBundles, ref consumedLength);
		}

	}
}
