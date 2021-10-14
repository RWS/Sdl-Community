using System.Collections.Generic;
using System.Linq;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Patterns
{
	public class IntegerNumber : AbstractPattern
    {
		private readonly List<string> _listOfThousandSeparators;
		private Sequence Integer { get; }

		public IntegerNumber(string thousandSeparators)
		{
			_listOfThousandSeparators = thousandSeparators.Select(s => s.ToString()).ToList();
			var thousandSeparatorsPattern = new Any(_listOfThousandSeparators)
			{
				Name = "ThousandSeparator"
			};

			var signs = "+-−".Select(s => s.ToString()).ToList();
			var signPattern = new Optional(new Any(signs))
			{
				Name = "Sign"
			};

			var thousandGroup = new Either(
				new Repeat(
					new Sequence(
						thousandSeparatorsPattern,
						new Repeat(new Range('0', '9'), 3, 3)), 1),
				new Repeat(new Range('0', '9')))
			{
				Name = "ThousandGroup"
			};

			Integer =
				new Sequence(
					signPattern,
					new Either(
						new Character('0'),
						new Sequence(
							new Range('1', '9'),
							new Optional(new Range('0', '9')),
							thousandGroup)));
		}

		public override IMatch Match(TextToParse text)
	    {
		    var integerMatch = Integer.Match(text);
		    integerMatch.Name = Name;

		    return integerMatch;
	    }
    }
}
