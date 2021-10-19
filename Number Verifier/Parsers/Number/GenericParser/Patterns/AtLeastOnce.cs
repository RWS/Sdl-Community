using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Patterns
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

			var subPattern = new Repeat(_pattern, 1);

			return subPattern.Match(text);
		}
	}
}