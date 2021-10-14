using System.Linq;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Matches;

namespace Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Patterns
{
	public class Sequence : AbstractPattern
	{
		private readonly IPattern[] _pattern;

		public Sequence(params IPattern[] pattern)
		{
			_pattern = pattern;
		}

		public override IMatch Match(TextToParse text)
		{
			var matchesArray = new MatchArray();

			foreach (var el in _pattern)
			{
				var match = el.Match(text);

				if (!match.Success) break;

				matchesArray.AddPattern(match);
			}

			if (matchesArray.Matches.Any() && matchesArray.Matches.All(el => el.Success)) matchesArray.Success = true;

			return matchesArray;
		}
	}
}