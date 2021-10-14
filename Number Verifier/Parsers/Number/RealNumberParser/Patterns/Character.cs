using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Matches;
using Match = Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Matches.Match;

namespace Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Patterns
{
	public class Character : AbstractPattern
	{
		public Character(char c)
		{
			Pattern = c;
		}

		public char Pattern { get; }

		public override IMatch Match(TextToParse text)
		{
			if (text.IsAtEnd())
				return (new NoMoreText(Pattern.ToString()));

			if (text.Current != Pattern)
				return (new NoMatch($"({text.Current})", Pattern.ToString(), text.CurrentIndex));

			var matchedChar = text.Current.ToString();
			text.CurrentIndex++;

			return new Match(matchedChar);
		}
	}
}