using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	public class DateTimePattern : FSTRecognizer
	{
		private Core.Tokenization.DateTimePatternType _PatternType;
		private string _FormatString;

		public DateTimePattern(Core.Tokenization.DateTimePatternType patternType, System.Globalization.CultureInfo culture,
			string formatString, LanguagePlatform.Lingua.FST.FST fst)
			: base(fst, culture)
		{
			_PatternType = patternType;
			_FormatString = formatString;
		}

		public Core.Tokenization.DateTimePatternType PatternType
		{
			get { return _PatternType; }
		}

		public string FormatString
		{
			get { return _FormatString; }
		}

		/// <summary>
		/// Parses the output of the FST traversal as specified by the automaton. See <see cref="DateTimePatternComputer"/>
		/// for details.
		/// </summary>
		/// <param name="fstOutput">The FST's output as a string</param>
		/// <returns>A DateTime object, default(DateTime) on failure.</returns>
		private DateTime ParseOutput(string fstOutput)
		{
			// unfortunately we cannot set the value of the DateTime fields directly since
			//  if e.g. no year is given (as in "January 01"), calling new DateTime(0, 1, 1) will fail.
			//  Instead, we construct a date/time pattern which is understood by the DateTime parser
			//  and circumvent that issue.

			// TODO era, calendar, am/pm, etc.

			char specifier = '\0';
			int len = fstOutput.Length;
			int specifierLength = 0;
			bool literal = false;

			System.Text.StringBuilder parsePattern = new StringBuilder();

			for (int i = 0; i < len; ++i)
			{
				char c = fstOutput[i];

				if (literal)
				{
					if (c == '\'')
					{
						literal = false;
						// hopefully captures 't', 'tt', and 'gg'
						parsePattern.Append("'\\''");
						for (int p = 0; p < specifierLength; ++p)
							parsePattern.Append(specifier);
						parsePattern.Append("'\\''");
					}
					continue;
				}

				if (Char.IsLetter(c))
				{
					if (specifier == c)
						++specifierLength;
					else
						specifierLength = 1;
					specifier = c;
					parsePattern.Append('\'');
					parsePattern.Append(specifier);
					parsePattern.Append('\'');
				}
				else if (c == '\'')
				{
					literal = true;
				}
				else
				{
					System.Diagnostics.Debug.Assert(Char.IsDigit(c));
					parsePattern.Append(specifier);
				}
			}

			DateTime result;
			if (!DateTime.TryParseExact(fstOutput, parsePattern.ToString(),
				_Culture, System.Globalization.DateTimeStyles.None, out result))
			{
				result = default(DateTime);
			}
			return result;
		}

		public Match Match(string s, int startOffset, out DateTime output)
		{
			// #38572
			const bool ignoreCase = true;

			output = default(DateTime);

			List<FSTMatch> matches = ComputeMatches(s, startOffset, ignoreCase, 1, false);

			if (matches == null || matches.Count == 0)
				return null;

			System.Diagnostics.Debug.Assert(matches.Count == 1);

			// TODO maybe get/iterate through all matches if parsing match[0] failed?
			output = ParseOutput(matches[0].Output);
			return new Match(startOffset, matches[0].Length);
		}
	}
}
