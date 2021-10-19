using System.Collections.Generic;
using System.Linq;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Patterns.Specialized
{
	public class IntegerNumber : AbstractPattern
    {
		private Sequence Integer { get; }

		public IntegerNumber(List<string> thousandSeparators)
		{
			var listOfThousandSeparators = thousandSeparators;
			var thousandSeparatorsPattern = new Any(listOfThousandSeparators, true)
			{
				Name = "ThousandSeparator"
			};

			Integer =
				new Sequence(
					new Either(
						new Character('0'),
						new Sequence(
							new DigitRange('1', '9'),
							new Optional(new DigitRange('0', '9')),
							new Optional(
								new Either(
									new Repeat(
										new Sequence(
											thousandSeparatorsPattern,
											new Repeat(new DigitRange('0', '9'), 3, 3)), 1),
									new Repeat(new DigitRange('0', '9')))))));
		}

		public override IMatch Match(TextToParse text)
	    {
		    var integerMatch = Integer.Match(text);
		    integerMatch.Name = Name;

		    return integerMatch;
	    }
    }
}
