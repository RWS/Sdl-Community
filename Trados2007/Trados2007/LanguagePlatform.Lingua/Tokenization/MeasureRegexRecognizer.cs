using System;
using System.Collections.Generic;
using System.Text;

// disable warnings about missing XML comments for internal classes:
// #pragma warning disable 1591

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	internal enum NumberSeparatorMode
	{
		/// <summary>
		/// use culture's default separators
		/// </summary>
		CultureDefault,
		/// <summary>
		/// Use culture's swapped default separators
		/// </summary>
		Swapped,
		/// <summary>
		/// Use separators of en-US ('.' decimal, ',' group separator)
		/// </summary>
		EnUS
	}

	internal class MeasureRegexRecognizer : RegexRecognizer
	{
		private System.Globalization.CultureInfo _Culture;

		public static Recognizer Create(System.Globalization.CultureInfo culture, int priority)
		{
			try
			{
				// TODO support non-blank languages for unit separation

				MeasureRegexRecognizer result = new MeasureRegexRecognizer(100, "DEFAULT_MEASURE_RECOGNIZER", culture);

				Core.CharacterSet first = null;
				// augmentation doesn't change FIRST()
				List<string> patterns = NumberRegexRecognizer.ComputeRXPatterns(culture, NumberSeparatorMode.CultureDefault, out first);
				AugmentPatterns(patterns, culture);

				foreach (string p in patterns)
				{
					// use the same first for all patterns (the number regex pattern computer returns just one pattern anyway)
					result.Add(p, first, 2);
				}

				SeparatorCombination defaultSc = new SeparatorCombination(culture, false);
				if (defaultSc.IsSwappable())
				{
					patterns = NumberRegexRecognizer.ComputeRXPatterns(culture, NumberSeparatorMode.Swapped, out first);
					AugmentPatterns(patterns, culture);

					foreach (string p in patterns)
					{
						result.Add(p, first, 1);
					}
				}
				if (NumberPatternComputer.DoAddENUSSeparators(culture))
				{
					patterns = NumberRegexRecognizer.ComputeRXPatterns(culture, NumberSeparatorMode.EnUS, out first);
					AugmentPatterns(patterns, culture);

					foreach (string p in patterns)
					{
						result.Add(p, first, 0);
					}
				}

				result.OnlyIfFollowedByNonwordCharacter
					= true; // otherwise "123 ABC" will be recognized as "123 A" "BC" in Japanese
				return result;
			}
			catch // (System.Exception e)
			{
				return null;
			}
		}

		private static void AugmentPatterns(List<string> numberPatterns,
			System.Globalization.CultureInfo culture)
		{
			string unitRx = Core.Tokenization.PhysicalUnit.GetUnitsRX(culture, false);
			string optSep = @"[ \u00A0]?";

			// NOTE if any units may precede the numeric part, you need to add them to the FIRST set for the returned
			//  pattern and adjust the code in the constructor accordingly

			for (int i = 0; i < numberPatterns.Count; ++i)
				numberPatterns[i] = String.Format("(?<num>{0})(?<sep>{1})(?<unit>{2})", numberPatterns[i], optSep, unitRx);
		}

		private MeasureRegexRecognizer(int priority, string recognizerName, System.Globalization.CultureInfo culture)
			: base(Core.Tokenization.TokenType.Measurement, priority, "MEASURE", recognizerName)
		{
			// TODO set up special context rules for validation
			_Culture = culture;
		}

		protected override Core.Tokenization.Token CreateToken(string s,
			System.Text.RegularExpressions.GroupCollection groups)
		{
			string unit = groups["unit"].Value;
			string num = groups["num"].Value;
			string sep = groups["sep"].Value;

			string decimalPart = groups["sdec"].Value;
			if (String.IsNullOrEmpty(decimalPart))
				decimalPart = groups["gdec"].Value;

			char separator = (sep.Length > 0) ? sep[0] : '\0';

			Core.Tokenization.MeasureToken value;

			Core.Tokenization.Unit u = Core.Tokenization.PhysicalUnit.Find(unit, _Culture);

			value = new Core.Tokenization.MeasureToken(s,
				groups["sign"].Value, decimalPart, groups["frac"].Value,
				u, unit, separator, _Culture.NumberFormat);

			return value;
		}
	}
}
