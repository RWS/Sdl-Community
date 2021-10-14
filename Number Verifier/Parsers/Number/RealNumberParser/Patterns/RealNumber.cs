using System;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Matches;

namespace Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Patterns
{
	public class RealNumber : AbstractPattern
	{
		private Sequence Number { get; } = 
			new Sequence(
				new IntegerNumber(),
				new Optional(
					new Sequence(
						new Character('.'),
						new AtLeastOnce(
							new Range('0', '9')
							)
						)
					)
				);

		public override IMatch Match(TextToParse text)
		{
			while (true)
			{
				var match = Number.Match(text);
				if (match.Success || text.IsAtEnd()) return (match);

				text.Advance();
			}
		}
	}
}
