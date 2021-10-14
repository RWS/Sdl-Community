using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Patterns
{
	public class Optional : AbstractPattern
	{
		private readonly IPattern _optionalPattern;

		public Optional(IPattern pattern) => _optionalPattern = pattern;
		public bool Flatten { get; set; }

		public override IMatch Match(TextToParse text)
		{
			var subPattern = new Repeat(_optionalPattern, 0, 1);
			var match = subPattern.Match(text);

			//if (Flatten && match is MatchArray matchArray && matchArray.Matches.Count == 1) match = matchArray.Matches[0]; 

			match.Success = true;
			match.Name = Name;

			return match;
		}
	}
}