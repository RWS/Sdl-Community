using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Matches;

namespace Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Patterns
{
    public class Many : AbstractPattern
    {
	    private readonly IPattern _pattern;
		private readonly int _start;
	    private readonly int _end;

        public Many(IPattern pattern, int start = 0, int end = 0)
        {
            _pattern = pattern;
            _start = start;
            _end = end;
        }

        public override IMatch Match(TextToParse text)
        {
            var matchedText = "";
	        var matchesArray = new MatchArray { Success = false };
	        var originalText = text;

            var match = _pattern.Match(text);
            
            var count = 0;

            while (match.Success)
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
            return (new NoMatch(matchedText + noMatch.Current, noMatch.Expected ,matchedText.Length + noMatch.Current.Length-3));
        }
    }
}
