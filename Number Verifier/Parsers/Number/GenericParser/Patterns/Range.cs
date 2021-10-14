using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Patterns
{
    public class Range : AbstractPattern
    {
	    readonly char _limOne;
	    readonly char _limTwo;

        public Range(char limOne, char limTwo)
        {
            _limOne = limOne;
            _limTwo = limTwo;
        }

        public override IMatch Match(TextToParse text)
        {
            if (text.IsAtEnd())
                return new NoMoreText();


	        if (text.Current < _limOne || text.Current > _limTwo)
		        return (new NoMatch($"({text.Current})", text.CurrentIndex));

	        var matchedChar = text.Current.ToString();
	        text.CurrentIndex++;

	        return (new Match(matchedChar));
        }

    }
}
