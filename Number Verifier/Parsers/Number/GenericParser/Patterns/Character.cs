using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;
using Match = Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches.Match;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Patterns
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