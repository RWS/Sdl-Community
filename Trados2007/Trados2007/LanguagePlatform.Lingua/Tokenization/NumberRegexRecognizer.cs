using System;
using System.Collections.Generic;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System.Linq;

// disable warnings about missing XML comments for internal classes:
// #pragma warning disable 1591

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	internal class NumberRegexRecognizer : RegexRecognizer
	{
		private System.Globalization.NumberFormatInfo _NumberFormat;

		public static Recognizer Create(System.Globalization.CultureInfo culture, int priority)
		{
			try
			{
				NumberRegexRecognizer recog = new NumberRegexRecognizer(100, "DEFAULT_NUMBER_RECOGNIZER", culture.NumberFormat);

				CharacterSet first = null;
				foreach (string p in ComputeRXPatterns(culture, NumberSeparatorMode.CultureDefault, out first))
					recog.Add(p, first, 2);

				SeparatorCombination defaultSc = new SeparatorCombination(culture, false);
				if (defaultSc.IsSwappable())
				{
					foreach (string p in ComputeRXPatterns(culture, NumberSeparatorMode.Swapped, out first))
						recog.Add(p, first, 1);
				}
				if (NumberPatternComputer.DoAddENUSSeparators(culture))
				{
					foreach (string p in ComputeRXPatterns(culture, NumberSeparatorMode.EnUS, out first))
						recog.Add(p, first, 1);
				}

				recog.OnlyIfFollowedByNonwordCharacter
					= CultureInfoExtensions.UseBlankAsWordSeparator(culture);
				recog.AdditionalTerminators = new CharacterSet();
				recog.AdditionalTerminators.Add('-'); // TODO other math symbols?
				recog.OverrideFallbackRecognizer = true;

				return recog;
			}
			catch // (System.Exception e)
			{
				return null;
			}
		}

		public NumberRegexRecognizer(int priority, string recognizerName, System.Globalization.NumberFormatInfo numberFormat)
			: base(TokenType.Number, priority, "NUMBER", recognizerName)
		{
			// TODO set up special context rules to avoid "5000|:000"
			_NumberFormat = numberFormat;
		}

		protected override Core.Tokenization.Token CreateToken(string s,
			System.Text.RegularExpressions.GroupCollection groups)
		{
			string decimalPart = groups["sdec"].Value;
			if (String.IsNullOrEmpty(decimalPart))
				decimalPart = groups["gdec"].Value;

			NumberToken value = new NumberToken(s,
				groups["sign"].Value, decimalPart, groups["frac"].Value, _NumberFormat);

			return value;
		}

		internal static List<string> ComputeRXPatterns(System.Globalization.CultureInfo culture,
			NumberSeparatorMode separatorMode,
			out CharacterSet first)
		{
			first = null;

			if (culture == null)
				throw new ArgumentNullException("culture");

			if (culture.IsNeutralCulture)
			{
				System.Diagnostics.Debug.Assert(false, "Cannot compute number pattern for neutral cultures");
				return null;
			}

			IList<string> digits = culture.NumberFormat.NativeDigits;

			string decSep = culture.NumberFormat.NumberDecimalSeparator;
			string grpSep = culture.NumberFormat.NumberGroupSeparator;

			switch (separatorMode)
			{
			case NumberSeparatorMode.CultureDefault:
				// nop
				break;
			case NumberSeparatorMode.Swapped:
				{
					string tmp = decSep;
					decSep = grpSep;
					grpSep = tmp;
				}
				break;
			case NumberSeparatorMode.EnUS:
				decSep = ".";
				grpSep = ",";
				break;
			default:
				throw new Exception("Unexpected");
			}

			List<string> result = ComputeRXPatterns(grpSep, decSep, digits, out first);
			if (result == null)
				return null;

			// add full-width variant digits for jp, zh, ko
			if (NumberPatternComputer.DoAddFullwidthVariantDigits(culture))
			{
				Core.CharacterSet alternateFirst;
				digits = "\uFF10|\uFF11|\uFF12|\uFF13|\uFF14|\uFF15|\uFF16|\uFF17|\uFF18|\uFF19".Split('|');
				List<string> alternate = ComputeRXPatterns(grpSep, decSep, digits, out alternateFirst);
				if (alternate != null)
				{
					result.AddRange(alternate);
					first.Add(alternateFirst);
				}
			}

			// add patters with default digits
			if (!CultureInfoExtensions.UsesDefaultDigits(culture))
			{
				Core.CharacterSet alternateFirst;
				digits = "0|1|2|3|4|5|6|7|8|9".Split('|');
				List<string> alternate = ComputeRXPatterns(grpSep, decSep, digits, out alternateFirst);
				if (alternate != null)
				{
					result.AddRange(alternate);
					first.Add(alternateFirst);
				}
			}

			return result;
		}

		private static List<string> ComputeRXPatterns(string grpSep, string decSep, IList<string> digits,
			out CharacterSet first)
		{
			first = null;

			if (decSep == null || decSep.Length != 1
				|| grpSep == null || grpSep.Length != 1
				|| digits == null || digits.Count != 10)
			{
				return null;
			}

			if (digits.Any(s => s.Length != 1))
				return null;

			// TODO escape digit's special symbols (unlikely)
			string dig = System.String.Format("[{0}-{1}]", digits[0],
				digits[9]);

			// haven't yet seen other signs in any of the supported cultures
			string optSign = "(?<sign>[-+\x2013\x2212\xFF0B\xFF0D])?";

			// FIRST: all digits plus the signs
			first = new CharacterSet();
			first.Add(digits[0][0], digits[9][0]);
			first.Add('-');
			first.Add('+');
			first.Add('\x2013');
			first.Add('\x2212');
			first.Add('\xFF0B');
			first.Add('\xFF0D');

			// ---------- digit sequence (preceding decimal point)

			string simpleDecPart = System.String.Format("(?<sdec>{0}+)", dig);

			// TODO non-breaking space as grp sep

			string groupedDecPart = System.String.Format("(?<gdec>{0}{{1,3}}(\\{1}{2}{{3}})*)",
				dig, grpSep, dig);

			// ---------- fractional part

			// do NOT recognize .25 (no optional decimal part) 
			string optFracPart = System.String.Format("(?<frac>\\{0}{1}+)?", decSep, dig);

			List<string> result = new List<string>();

			result.Add(optSign + simpleDecPart + optFracPart);
			result.Add(optSign + groupedDecPart + optFracPart);

			return result;
		}
	}
}
