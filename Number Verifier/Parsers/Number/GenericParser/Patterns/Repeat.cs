using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Patterns
{
	public class Repeat : AbstractPattern
	{
		private readonly int _end;
		private readonly IPattern _pattern;
		private readonly int _start;
		private int _count;

		public Repeat(IPattern pattern, int start = 0, int end = 0)
		{
			_pattern = pattern;
			_start = start;
			_end = end;
		}

		private bool InRange => _count >= _start && (!IsLimited || _count <= _end);

		private bool IsLimited => _end != 0;

		private bool ReachedEnd => _count == _end;

		public override IMatch Match(TextToParse text)
		{
			_count = 0;
			var matchesArray = new MatchArray { Success = false };
			while (IsLimited && !ReachedEnd || !IsLimited)
			{
				var match = _pattern.Match(text);
				if (!match.Success) break;

				_count++;
				matchesArray.AddPattern(match);
			}

			if (InRange)
			{
				matchesArray.Success = true;
				return matchesArray;
			}

			return (new NoMatch());
		}
	}
}