using System;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Patterns
{
    public class DigitRange : AbstractPattern
    {
	    readonly char _limOne;
	    readonly char _limTwo;
	    readonly char _fullwidthlimOne;
	    readonly char _fullwidthlimTwo;

        public DigitRange(int limOne, int limTwo)
        {
			var rangeDifference = '０' - '0';
			_limOne = Convert.ToChar(limOne);
            _limTwo = Convert.ToChar(limTwo);
	        _fullwidthlimOne = (char)(_limOne + rangeDifference);
	        _fullwidthlimTwo = (char)(_limTwo + rangeDifference);
        }

        public override IMatch Match(TextToParse text)
        {
            if (text.IsAtEnd())
                return new NoMoreText();

	        var currentChar = text.Current;
	        if (currentChar >= _limOne && currentChar <= _limTwo ||
	            currentChar >= _fullwidthlimOne && currentChar <= _fullwidthlimTwo)
	        {
		        var matchedChar = text.Current.ToString();
		        text.Advance();
				return (new Match(matchedChar));
	        }
			
		    return (new NoMatch($"({text.Current})", text.CurrentIndex));
        }

    }
}
