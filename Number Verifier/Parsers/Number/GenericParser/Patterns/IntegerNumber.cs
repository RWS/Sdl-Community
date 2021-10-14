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
			var thousandSeparatorsPattern = new Any(_listOfThousandSeparators);

			thousandSeparatorsPattern.Matched += SelectThousandSeparator;

			var signs = "+-−".Select(s => s.ToString()).ToList();
			Integer =
				new Sequence(
					new Optional(new Any(signs)),
					new Either(
						new Character('0'),
						new Sequence(
							new Range('1', '9'),
							new Optional(new Range('0', '9')),
							new Either(
								new Repeat(
									new Sequence(
										thousandSeparatorsPattern,
										new Repeat(new Range('0', '9'), 3, 3))),
								new Repeat(
									new Sequence(
										new Repeat(new Range('0', '9'))))))));
		}

		/// <summary>
		/// Once the first thousand separator has been found in the text,
		/// it is selected as the only possible thousand separator for this number
		/// </summary>
		/// <param name="matched">The matched thousand separator</param>
		private void SelectThousandSeparator(string matched)
		{
			_listOfThousandSeparators.RemoveAll(el => el != matched);
		}

		public override IMatch Match(TextToParse text)
	    {
		    var integerMatch = Integer.Match(text);
		    integerMatch.Name = Name;

		    return integerMatch;
	    }
    }
}
