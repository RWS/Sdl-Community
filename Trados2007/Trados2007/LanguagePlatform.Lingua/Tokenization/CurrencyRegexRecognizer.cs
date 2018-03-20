using System;
using System.Collections.Generic;
using System.Text;

// disable warnings about missing XML comments for internal classes:
// #pragma warning disable 1591

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	internal class CurrencyRegexRecognizer : RegexRecognizer
	{
		private System.Globalization.CultureInfo _Culture;

		public static Recognizer Create(System.Globalization.CultureInfo culture,
			Core.Wordlist currencySymbols,
			int priority)
		{
			try
			{
				// TODO support non-blank languages for unit separation

				Core.CharacterSet curFirst = null;
				string currenciesRx = currencySymbols.GetRegularExpression(out curFirst);

				int currencyPattern = culture.NumberFormat.CurrencyPositivePattern;

				bool currencyPrecedesNumber = (currencyPattern % 2) == 0;
				bool currencyIsSeparated = (currencyPattern >= 2);

				CurrencyRegexRecognizer result = new CurrencyRegexRecognizer(100, "DEFAULT_CURRENCY_RECOGNIZER", culture);

				Core.CharacterSet first = null;
				// augmentation doesn't change FIRST()
				// TODO use currency pattern instead of number pattern?
				List<string> patterns = NumberRegexRecognizer.ComputeRXPatterns(culture, NumberSeparatorMode.CultureDefault, out first);

				if (currencyPrecedesNumber)
				{
					first.Add(curFirst);
				}

				AugmentPatterns(patterns, currenciesRx, culture);

				foreach (string p in patterns)
				{
					// use the same first for all patterns (the number regex pattern computer returns just one pattern anyway)
					result.Add(p, first, 2);
				}

				/*
				 * Be strict for currencies (only flexible for measurements and numbers)
				 * 
				if (NumberRegexRecognizer.CanSwapSeparators(culture))
				{
					patterns = NumberRegexRecognizer.ComputeRXPatterns(culture, NumberSeparatorMode.Swapped, out first);
					AugmentPatterns(patterns, currenciesRx, culture);

					foreach (string p in patterns)
					{
						result.Add(p, first, 1);
					}
				}
				if (NumberRegexRecognizer.AddENUSSeparators(culture))
				{
					patterns = NumberRegexRecognizer.ComputeRXPatterns(culture, NumberSeparatorMode.EnUS, out first);
					AugmentPatterns(patterns, currenciesRx, culture);

					foreach (string p in patterns)
					{
						result.Add(p, first, 0);
					}
				}
				 */

				result.OnlyIfFollowedByNonwordCharacter
					= Core.CultureInfoExtensions.UseBlankAsWordSeparator(culture);

				return result;
			}
			catch // (System.Exception e)
			{
				return null;
			}
		}

		private static void AugmentPatterns(List<string> numberPatterns,
			string currenciesRx,
			System.Globalization.CultureInfo culture)
		{
			string optSep = @"[ \u00A0]?";

			int currencyPattern = culture.NumberFormat.CurrencyPositivePattern;

			string fmtString = null;

			switch (currencyPattern)
			{
			case 0: // $n 
			default:
				fmtString = "(?<cur>{2})(?<num>{0})";
				break;
			case 1: // n$ 
				fmtString = "(?<num>{0})(?<cur>{2})";
				break;
			case 2: // $ n
				fmtString = "(?<cur>{2})(?<sep>{1})(?<num>{0})";
				break;
			case 3: // n $
				fmtString = "(?<num>{0})(?<sep>{1})(?<cur>{2})";
				break;
			}

			for (int i = 0; i < numberPatterns.Count; ++i)
			{
				numberPatterns[i] = String.Format(fmtString, numberPatterns[i], optSep, currenciesRx);
			}
		}

		private CurrencyRegexRecognizer(int priority, string recognizerName, System.Globalization.CultureInfo culture)
			: base(Core.Tokenization.TokenType.Measurement, priority, "CURRENCY", recognizerName)
		{
			// TODO set up special context rules for validation
			_Culture = culture;
		}

		protected override Core.Tokenization.Token CreateToken(string s,
			System.Text.RegularExpressions.GroupCollection groups)
		{
			string cur = groups["cur"].Value;
			string num = groups["num"].Value;
			string sep = groups["sep"].Value;

			string decimalPart = groups["sdec"].Value;
			if (String.IsNullOrEmpty(decimalPart))
				decimalPart = groups["gdec"].Value;

			char separator = (sep.Length > 0) ? sep[0] : '\0';

			Core.Tokenization.MeasureToken value = new Core.Tokenization.MeasureToken(s,
				groups["sign"].Value, decimalPart, 
				groups["frac"].Value, 
				Core.Tokenization.Unit.Currency, cur, separator, _Culture.NumberFormat);

			value.Unit = Core.Tokenization.Unit.Currency;

			return value;
		}
	}
}
