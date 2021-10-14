using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Matches;

namespace Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Patterns
{
	public class AtLeastOnce : AbstractPattern
	{
		private readonly IPattern _pattern;

		public AtLeastOnce(IPattern pattern)
		{
			_pattern = pattern;
		}

		public override IMatch Match(TextToParse text)
		{
			if (text.IsAtEnd())
				return new NoMoreText();

			var subPattern = new Many(_pattern, 1);

			return subPattern.Match(text);
		}
	}
}