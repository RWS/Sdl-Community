using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Interface;

namespace Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Patterns
{
	public class Optional : AbstractPattern
	{
		private readonly IPattern _optionalPattern;

		public Optional(IPattern pattern) => _optionalPattern = pattern;

		public override IMatch Match(TextToParse text)
		{
			var subPattern = new Many(_optionalPattern, 0, 1);

			var match = subPattern.Match(text);
			match.Success = true;

			return match;
		}
	}
}