using System.Linq;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Patterns
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

		public override IMatch Match(TextToParse text)
		{
			ResetNumber();
			while (true)
			{
				var match = Number.Match(text);

				if (match.Success || text.IsAtEnd())
				{
					EnsureUnambiguousDecimalSeparator(match as MatchArray);

					(match as MatchArray)?.Flatten();
					return match;
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
				Name = "Fraction",
				Flatten = true
			};

			Number =
				new Sequence(
					IntegerNumber,
					FractionalNumber
					);
		}

		private void EnsureUnambiguousDecimalSeparator(MatchArray match)
		{
			
			//if (match["Fraction"].)

		}
	}
}