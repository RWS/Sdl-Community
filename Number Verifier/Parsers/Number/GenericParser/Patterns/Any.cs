using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Events;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Patterns
{
	public class Any : AbstractPattern
	{
		private readonly List<string> _pattern;
		public event MatchedEventHandler Matched;

		public Any(List<string> text)
		{
			_pattern = text;
		}

		public override IMatch Match(TextToParse text)
		{
			if (text.IsAtEnd())
				return new NoMoreText();

			if (_pattern.Contains(text.Current.ToString()))
			{
				var matchedChar = text.Current.ToString();
				var match = new Match(matchedChar)
				{
					Name = Name
				};

				Matched?.Invoke(match.Value);

				text.Advance();
				return match;
			}

			return new NoMatch($"({text.Current})", string.Join(",", _pattern), text.CurrentIndex);
		}
	}
}