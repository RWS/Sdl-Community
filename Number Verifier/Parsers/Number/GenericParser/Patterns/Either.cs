using System.Collections.Generic;
using System.Linq;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Patterns
{
	public class Either : AbstractPattern
	{
		private readonly List<IPattern> _pattern;

		public Either(params IPattern[] pattern)
		{
			_pattern = pattern.ToList();
		}

		public override IMatch Match(TextToParse text)
		{
			foreach (var el in _pattern)
			{
				if (text.IsAtEnd())
					return new NoMoreText();

				var match = el.Match(text);
				match.Name = Name;

				if (match.Success)
				{
					return match;
				}
			}

			return new NoMatch();
		}
	}
}