using System.Collections.Generic;
using System.Linq;
using Sdl.Community.NumberVerifier.Helpers;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Patterns.Specialized
{
	public class RealNumber : AbstractPattern
	{
		private readonly List<string> _decimalSeparators;
		private readonly bool _omitZero;
		private readonly List<string> _thousandSeparators;
		private Optional FractionalNumber { get; set; }

		public RealNumber(List<string> thousandSeparators, List<string> decimalSeparators, bool omitZero = false)
		{
			_thousandSeparators = thousandSeparators;
			_decimalSeparators = decimalSeparators;
			_omitZero = omitZero;
		}

		private AbstractPattern IntegerNumber { get; set; }
		private Sequence Number { get; set; }

		public List<string> ToNumberList()
		{
			if (MatchArray is null) return null;

			var numberList = new List<string>();
			foreach (var match in MatchArray)
			{
				var realNumberMatch = (MatchArray)match;
				var realNumberString = $"{realNumberMatch["Sign"]?.Value}{realNumberMatch["Integer"]?.Value}{realNumberMatch["Fraction"]?.Value}";

				if (_omitZero && LeadingZeroOmitted(realNumberMatch, realNumberString))
				{
					var decimalSeparator = realNumberMatch["DecimalSeparator"]?.Value;
					var firstZeroIndex = decimalSeparator is not null ? realNumberString.IndexOf(decimalSeparator) : 0;
					realNumberString = realNumberString.Insert(firstZeroIndex, "0");
				}

				numberList.Add(realNumberString);
			}

			return numberList;
		}

		private static bool LeadingZeroOmitted(MatchArray realNumberMatch, string realNumberString, bool normalized = false)
		{
			var firstSymbol = realNumberString[0].ToString();
			var firstTwoSymbolsAreNotDigits = realNumberString.Substring(0, 2).All(s => !char.IsDigit(s));
			var decimalSeparator =  normalized ? "d" : realNumberMatch["DecimalSeparator"]?.Value;
			
			return firstSymbol == decimalSeparator
			       || firstTwoSymbolsAreNotDigits;
		}

		public List<string> ToNormalizedNumberList()
		{
			if (MatchArray is null) return null;
			var numberList = new List<string>();
			foreach (var match in MatchArray)
			{
				var realNumberMatch = (MatchArray)match;
				var thousandSeparator = realNumberMatch["ThousandSeparator"]?.Value;
				var decimalSeparator = realNumberMatch["DecimalSeparator"]?.Value;

				var integerPart = realNumberMatch["Integer"]?.Value;

				var signPart = realNumberMatch["Sign"]?.Value;
				var signed = signPart is not null;
				if (signed) signPart = "s";

				string normalizedIntegerPart = null;
				if (integerPart is not null)
				{
					normalizedIntegerPart = thousandSeparator is null
						? _thousandSeparators.Contains(Constants.NoSeparator) ? GetNumberWithThousandSeparatorsAdded(integerPart, signed) : integerPart
						: integerPart.Replace(thousandSeparator, "t");
				}

				var fractionPart = realNumberMatch["Fraction"]?.Value;
				string normalizedFractionPart = null;
				var isDecimal = decimalSeparator is not null;
				if (fractionPart is not null)
				{
					normalizedFractionPart = isDecimal
						? fractionPart.Replace(decimalSeparator, "d")
						: fractionPart;
				}

				var realNumberString = $"{signPart}{normalizedIntegerPart}{normalizedFractionPart}";

				if (_omitZero && LeadingZeroOmitted(realNumberMatch, realNumberString, true))
				{
					var firstZeroIndex = isDecimal ? realNumberString.IndexOf("d") : 0;
					realNumberString = realNumberString.Insert(firstZeroIndex, "0");
				}

				numberList.Add(realNumberString);
			}

			return numberList;
		}

		private string GetNumberWithThousandSeparatorsAdded(string normalizedIntegerPart, bool signed)
		{
			var noStart = signed ? 1 : 0;
			for (var i = normalizedIntegerPart.Length -3; i > noStart; i -= 3)
			{
				normalizedIntegerPart = normalizedIntegerPart.Insert(i, "t");
			}

			return normalizedIntegerPart;
		}

		public override IMatch Match(TextToParse text)
		{
			ResetNumber();
			while (true)
			{
				var match = Number.Match(text);

				if (match.Success || text.IsAtEnd())
				{
					EnsureUnambiguousDecimalSeparator(match as MatchArray);

					return (match as MatchArray)?.FlattenByNamedGroups();
				}

				text.Advance();
			}
		}

		/// <summary>
		/// Allows to have different thousand separators for each number in the same text
		/// </summary>
		private void ResetNumber()
		{
			var signs = "+-−".Select(s => s.ToString()).ToList();
			var signPattern = new Optional(new Any(signs))
			{
				Name = "Sign"
			};

			IntegerNumber = _omitZero
				? new Optional(new IntegerNumber(_thousandSeparators.ToList()))
				: new IntegerNumber(_thousandSeparators.ToList());
			var decimalSeparatorsPattern = new Any(_decimalSeparators)
			{
				Name = "DecimalSeparator"
			};

			IntegerNumber.Name = "Integer";

			//decimalSeparatorsPattern.Matched += EnsureUnambiguousDecimalSeparator;

			FractionalNumber = new Optional(
				new Sequence(
					decimalSeparatorsPattern,
					new AtLeastOnce(
						new DigitRange('0', '9')
						)
					)
				)
			{
				Name = "Fraction"
			};

			Number =
				new Sequence(
					signPattern,
					IntegerNumber,
					FractionalNumber)
				{
					Name = "RealNumber"
				};
		}

		private void EnsureUnambiguousDecimalSeparator(MatchArray match)
		{
			
			//if (match["Fraction"].)

		}
	}
}