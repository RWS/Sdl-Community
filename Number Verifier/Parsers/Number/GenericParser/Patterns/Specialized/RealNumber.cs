using System.Collections.Generic;
using System.Linq;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Patterns.Specialized
{
	public class RealNumber : AbstractPattern
	{
		private readonly string _decimalSeparators;
		private readonly string _thousandSeparators;
		private Optional FractionalNumber { get; set; }

		public RealNumber(string thousandSeparators, string decimalSeparators)
		{
			_thousandSeparators = thousandSeparators;
			_decimalSeparators = decimalSeparators;
		}

		private IntegerNumber IntegerNumber { get; set; }
		private Sequence Number { get; set; }

		public List<string> ToNumberList()
		{
			var numberList = new List<string>();
			foreach (var match in MatchArray)
			{
				var realNumberMatch = (MatchArray)match;
				var realNumberString = $"{realNumberMatch["Integer"].Value}{realNumberMatch["Fraction"]?.Value}";

				numberList.Add(realNumberString);
			}

			return numberList;
		}
		
		public List<string> ToNormalizedNumberList()
		{
			var numberList = new List<string>();
			foreach (var match in MatchArray)
			{
				var realNumberMatch = (MatchArray)match;
				var thousandSeparator = realNumberMatch["ThousandSeparator"]?.Value;
				var decimalSeparator = realNumberMatch["DecimalSeparator"]?.Value;

				var integerPart = realNumberMatch["Integer"]?.Value;
				var signPart = realNumberMatch["Sign"]?.Value;
				string normalizedIntegerPart = null;
				if (integerPart is not null)
				{
					normalizedIntegerPart = signPart is null
						? integerPart
						: integerPart.Replace(signPart, "s");

					normalizedIntegerPart = thousandSeparator is null
						? normalizedIntegerPart
						: normalizedIntegerPart.Replace(thousandSeparator, "t");
				}

				var fractionPart = realNumberMatch["Fraction"]?.Value;
				string normalizedFractionPart = null;
				if (fractionPart is not null)
				{
					normalizedFractionPart = decimalSeparator is null
						? fractionPart
						: fractionPart.Replace(decimalSeparator, "d");
				}

				var realNumberString = $"{normalizedIntegerPart}{normalizedFractionPart}";

				numberList.Add(realNumberString);
			}

			return numberList;
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
			IntegerNumber = new IntegerNumber(_thousandSeparators)
			{
				Name = "Integer",
			};
			var decimalSeparatorsPattern = new Any(_decimalSeparators.Select(s => s.ToString()).ToList())
			{
				Name = "DecimalSeparator"
			};

			//decimalSeparatorsPattern.Matched += EnsureUnambiguousDecimalSeparator;

			FractionalNumber = new Optional(
				new Sequence(
					decimalSeparatorsPattern,
					new AtLeastOnce(
						new Range('0', '9')
						)
					)
				)
			{
				Name = "Fraction"
			};

			Number =
				new Sequence(
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