using System;
using System.Collections.Generic;
using System.Text;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	internal class DefaultJAZHFallbackRecognizer : DefaultFallbackRecognizer
	{
		private static bool _JUSTONE = true;

		public DefaultJAZHFallbackRecognizer(Core.Tokenization.TokenType t, int priority, System.Globalization.CultureInfo culture, Core.Resources.IResourceDataAccessor dataAccessor)
			: base(t, priority, culture, dataAccessor, false)
		{
			_IsFallbackRecognizer = true;
		}

		public override Core.Tokenization.Token Recognize(string s, int from, bool allowTokenBundles, ref int consumedLength)
		{
			if (String.IsNullOrEmpty(s) || from >= s.Length)
				return null;

			consumedLength = 0;
			int originalStart = from;

			if (Core.CharacterProperties.IsCJKPunctuation(s[from]))
			{
				while (from < s.Length && Core.CharacterProperties.IsCJKPunctuation(s[from]))
				{
					++consumedLength;
					++from;
					if (_JUSTONE)
						break;
				}
				Token t = new SimpleToken(s.Substring(originalStart, consumedLength), TokenType.GeneralPunctuation);
				return t;
			}

			if (Core.CharacterProperties.IsCJKChar(s[from]))
			{
				while (from < s.Length && Core.CharacterProperties.IsCJKChar(s[from]))
				{
					++consumedLength;
					++from;
					if (_JUSTONE)
						break;
				}
				Token t = new SimpleToken(s.Substring(originalStart, consumedLength), TokenType.CharSequence);
				return t;
			}

			// TODO CJK punctuation etc.

			return base.Recognize(s, from, allowTokenBundles, ref consumedLength);
		}

	}
}
