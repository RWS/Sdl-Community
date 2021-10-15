using System.Linq;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Patterns.Specialized
{
	public class IntegerNumber : AbstractPattern
    {
		private Sequence Integer { get; }

		public IntegerNumber(string thousandSeparators)
		{
			var listOfThousandSeparators = thousandSeparators.Select(s => s.ToString()).ToList();
			var thousandSeparatorsPattern = new Any(listOfThousandSeparators, true)
			{
				Name = "ThousandSeparator"
			};

			var signs = "+-−".Select(s => s.ToString()).ToList();
			var signPattern = new Optional(new Any(signs))
			{
				Name = "Sign"
			};

			Integer =
				new Sequence(
					signPattern,
					new Either(
						new Character('0'),
						new Sequence(
							new Range('1', '9'),
							new Optional(new Range('0', '9')),
							new Either(
								new Repeat(
									new Sequence(
										thousandSeparatorsPattern,
										new Repeat(new Range('0', '9'), 3, 3)), 1),
								new Repeat(new Range('0', '9'))))));
		}

		public override IMatch Match(TextToParse text)
	    {
		    var integerMatch = Integer.Match(text);
		    integerMatch.Name = Name;

		    return integerMatch;
	    }
    }
}
