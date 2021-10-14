using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Patterns
{
    public class Repeat : AbstractPattern
    {
	    private readonly IPattern _pattern;
		private readonly int _start;
	    private readonly int _end;

        public Repeat(IPattern pattern, int start = 0, int end = 0)
        {
            _pattern = pattern;
            _start = start;
            _end = end;
        }

        public override IMatch Match(TextToParse text)
        {
            var matchedText = "";
	        var originalText = text;
			var isExactNumberOfMatches = _start == _end && _start != 0;

            var match = _pattern.Match(text);
            
            var count = 0;

	        var matchesArray = new MatchArray { Success = false };
            while (match.Success && (!isExactNumberOfMatches || count != _end))
            {
                count++;

                matchesArray.AddPattern(match);
                match = _pattern.Match(text);
            }

            if (count >= _start && (_end == 0 || count <= _end))
            {
	            matchesArray.Success = true;
	            return matchesArray;
            }

            if ((count < _start && count > 0) || count > _end)
                return (new NoMatch($"Wrong number of <{_pattern}> objects", _pattern.ToString(), originalText.CurrentIndex));

            var noMatch = match as NoMatch;
            return (new NoMatch(matchedText + noMatch.Value, noMatch.Expected ,matchedText.Length + noMatch.Value.Length-3));
        }
    }
}
