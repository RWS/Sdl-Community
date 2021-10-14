using System.Linq;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Patterns
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
			var originalIndex = text.CurrentIndex;

			var matchesArray = new MatchArray();
			foreach (var el in _pattern)
			{
				var match = el.Match(text);
				matchesArray.AddPattern(match);

				if (match.Success) continue;

				text.CurrentIndex = originalIndex;
				break;
			}

			if (matchesArray.Matches.Any() && matchesArray.Matches.All(el => el.Success)) matchesArray.Success = true;

			matchesArray.Name = Name;
			return matchesArray;
		}
	}
}