using System.Collections.Generic;
using System.Linq;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Matches;

namespace Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Patterns
{
	public class Choice : AbstractPattern
	{
		private readonly List<IPattern> _pattern;

		public Choice(params IPattern[] pattern)
		{
			_pattern = pattern.ToList();
		}

		public override IMatch Match(TextToParse text)
		{
			var partialMatch = new NoMatch("");

			foreach (var el in _pattern)
			{
				if (text.IsAtEnd())
					return new NoMoreText();

				var match = el.Match(text);

				if (match.Success)
					return match;

				if (!(match is NoMatch noMatch)) continue;

				if (noMatch.Current.Length - 2 > partialMatch.Current.Length) partialMatch = noMatch;
			}

			return partialMatch;
		}
	}
}