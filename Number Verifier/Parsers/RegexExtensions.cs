using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sdl.Community.NumberVerifier.Helpers;

namespace Sdl.Community.NumberVerifier.Parsers
{
	public static class RegexExtensions
	{
		public static string Normalize(this Match realNumberMatch, List<string> thousandSeparators, List<string> decimalSeparators, bool omitZero)
		{
			if (realNumberMatch is null) return null;

			var thousandSeparator = realNumberMatch.Groups["ThousandSeparators"].Captures.Cast<Capture>().FirstOrDefault()?.Value;
			var decimalSeparator = realNumberMatch.Groups["DecimalSeparators"].Captures.Cast<Capture>().FirstOrDefault()?.Value;

			var integerPart = realNumberMatch.Groups["Integer"].Captures.Cast<Capture>().FirstOrDefault()?.Value;

			string normalizedIntegerPart = null;
			if (integerPart is not null)
			{
				normalizedIntegerPart = string.IsNullOrEmpty(thousandSeparator)
					? thousandSeparators.Contains(Constants.NoSeparator)
						? GetNumberWithThousandSeparatorsAdded(integerPart)
						: integerPart
					: integerPart.Replace(thousandSeparator, "t");
			}

			var fractionPart = realNumberMatch.Groups["Fraction"].Captures.Cast<Capture>().FirstOrDefault()?.Value;
			string normalizedFractionPart = null;
			var isDecimal = !string.IsNullOrEmpty(decimalSeparator);
			if (fractionPart is not null)
			{
				normalizedFractionPart = isDecimal
					? fractionPart.Replace(decimalSeparator, "d")
					: fractionPart;
			}

			var realNumberString = $"{normalizedIntegerPart}{normalizedFractionPart}";

			if (omitZero && LeadingZeroOmitted(realNumberMatch, realNumberString, true))
			{
				var firstZeroIndex = isDecimal ? realNumberString.IndexOf("d") : 0;
				realNumberString = realNumberString.Insert(firstZeroIndex, "0");
			}

			var ambiguous = thousandSeparators.Intersect(decimalSeparators);

			//RealNumberPattern is biased towards thousand separators, therefore if something has been determined to be thousand, it could've been decimal, but not the other way around
			if (!isDecimal && CountStringOccurrences(realNumberString, thousandSeparator) == 1)
				if (ambiguous.Contains(thousandSeparator)) realNumberString = realNumberString.Replace(thousandSeparator, "u");

			return realNumberString.Normalize(NormalizationForm.FormKC);
		}

		public static string ToClassOfCharactersString(this List<string> toBeJoined)
		{
			if (toBeJoined.Count <= 0) return null;

			var joinedListString = string.Join("", toBeJoined);
			joinedListString = $"[{joinedListString}]";

			return joinedListString;
		}

		public static int CountStringOccurrences(string text, string pattern)
		{
			if (text is null || pattern is null) return 0;
			// Loop through all instances of the string 'text'.
			var count = 0;
			var i = 0;
			while ((i = text.IndexOf(pattern, i)) != -1)
			{
				i += pattern.Length;
				count++;
			}
			return count;
		}

		private static bool LeadingZeroOmitted(Match realNumberMatch, string realNumberString, bool normalized = false)
		{
			var firstSymbol = realNumberString[0].ToString();
			var firstTwoSymbolsAreNotDigits = realNumberString.Substring(0, 2).All(s => !char.IsDigit(s));
			var decimalSeparator = normalized ? "d" : realNumberMatch.Groups["DecimalSeparator"].Captures.Cast<Capture>().FirstOrDefault()?.Value;

			return firstSymbol == decimalSeparator
			       || firstTwoSymbolsAreNotDigits;
		}

		private static string GetNumberWithThousandSeparatorsAdded(string normalizedIntegerPart)
		{
			for (var i = normalizedIntegerPart.Length - 3; i > 0; i -= 3)
			{
				normalizedIntegerPart = normalizedIntegerPart.Insert(i, "t");
			}

			return normalizedIntegerPart;
		}
	}
}
