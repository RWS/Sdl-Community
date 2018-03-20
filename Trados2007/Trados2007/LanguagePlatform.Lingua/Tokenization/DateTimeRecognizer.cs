using System;
using System.Collections.Generic;
using System.Text;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	internal class DateTimeRecognizer : Recognizer
	{
		private List<CalendarDateTimePatterns> _CalendarPatterns;

		public static Recognizer Create(Core.Resources.IResourceDataAccessor resourceDataAccessor, 
			System.Globalization.CultureInfo culture, 
			Core.Tokenization.DateTimePatternType types, int priority)
		{
			Recognizer result = new DateTimeRecognizer(priority,
				DateTimePatternComputer.GetPatterns(culture, resourceDataAccessor, types));
			result.OnlyIfFollowedByNonwordCharacter 
				= Core.CultureInfoExtensions.UseBlankAsWordSeparator(culture);
			return result;
		}

		public DateTimeRecognizer(int priority, List<CalendarDateTimePatterns> patterns)
			: base(TokenType.Date, priority, "DateTime", "DateTimeRecognizer")
		{
			if (patterns == null || patterns.Count == 0)
				throw new ArgumentNullException("patterns");
			_CalendarPatterns = patterns;
		}

		public override Core.Tokenization.Token Recognize(string s, int from, bool allowTokenBundles, ref int consumedLength)
		{
			int winningCalendar = -1;
			int winningPattern = -1;
			int winningLength = -1;
			DateTime winningValue = default(DateTime);

			for (int c = 0; c < _CalendarPatterns.Count; ++c)
			{
				for (int p = 0; p < _CalendarPatterns[c].Patterns.Count; ++p)
				{
					DateTime output;
					Match m = _CalendarPatterns[c].Patterns[p].Match(s, from, out output);
					if (m != null)
					{
						// don't check for winner if winning length too low
						if (m.Length < winningLength)
							continue;

						if (VerifyContextConstraints(s, m.Index + m.Length))
						{
							DateTime localValue;
							if (output != default(DateTime))
							{
								winningCalendar = c;
								winningLength = m.Length;
								winningPattern = p;
								winningValue = output;
							}
							else
							{
								if (DateTime.TryParse(s.Substring(from, m.Length),
									_CalendarPatterns[c].Culture,
									System.Globalization.DateTimeStyles.None, out localValue))
								{
									winningCalendar = c;
									winningLength = m.Length;
									winningPattern = p;
									winningValue = localValue;
								}
							}
						}
					}
				}
			}

			if (winningPattern >= 0)
			{
				consumedLength = winningLength;

				DateTimeToken t = new DateTimeToken(s.Substring(from, winningLength), winningValue,
					_CalendarPatterns[winningCalendar].Patterns[winningPattern].PatternType);
				return t;
			}
			else
				return null;
		}
	}
}
