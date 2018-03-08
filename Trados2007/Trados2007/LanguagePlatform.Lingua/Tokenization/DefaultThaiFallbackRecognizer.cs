using System;
using System.Collections.Generic;
using System.Text;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	internal class DefaultThaiFallbackRecognizer : DefaultFallbackRecognizer
	{
		private static bool _JUSTONE = false;

		public DefaultThaiFallbackRecognizer(Core.Tokenization.TokenType t, int priority, System.Globalization.CultureInfo culture, Core.Resources.IResourceDataAccessor dataAccessor)
			: base(t, priority, culture, dataAccessor, false)
		{
			// also used for Khmer now
			// System.Diagnostics.Debug.Assert(culture.TwoLetterISOLanguageName.Equals("th", StringComparison.OrdinalIgnoreCase));
			_IsFallbackRecognizer = true;
		}

		public override Core.Tokenization.Token Recognize(string s, int from, bool allowTokenBundles, ref int consumedLength)
		{
			if (String.IsNullOrEmpty(s) || from >= s.Length)
				return null;

			consumedLength = 0;
			int originalStart = from;

			// splitting off all punctuation may exaggerate a bit - wait for user feedback
			if (System.Char.IsPunctuation(s[from]))
			{
				while (from < s.Length && System.Char.IsPunctuation(s[from]))
				{
					++consumedLength;
					++from;
					if (_JUSTONE)
						break;
				}
				Token t = new SimpleToken(s.Substring(originalStart, consumedLength), TokenType.GeneralPunctuation);
				return t;
			}

			if (Core.CharacterProperties.IsInBlock(s[from], Core.UnicodeBlock.Thai))
			{
				while (from < s.Length && Core.CharacterProperties.IsInBlock(s[from], Core.UnicodeBlock.Thai))
				{
					++consumedLength;
					++from;
					if (_JUSTONE)
						break;
				}
				Token t = new SimpleToken(s.Substring(originalStart, consumedLength), TokenType.CharSequence);
				return t;
			}

			return base.Recognize(s, from, allowTokenBundles, ref consumedLength);
		}

	}
}
