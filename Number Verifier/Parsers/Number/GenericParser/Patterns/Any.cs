using System.Collections.Generic;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Patterns
{
	public class Any : AbstractPattern
	{
		private readonly bool _keepConsistent;
		private readonly List<string> _pattern;

		/// <summary>
		/// Match any string in list
		/// </summary>
		/// <param name="list"></param>
		/// <param name="keepConsistent">
		/// <see langword="true"/>The first string found will be used consistently (all others will be removed from list
		/// <see langword="false"/> Every string in the list will be used at every possible match location</param>
		public Any(List<string> list, bool keepConsistent = false)
		{
			_pattern = list;
			_keepConsistent = keepConsistent;
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

				if (_keepConsistent && _pattern.Count > 1) _pattern.RemoveAll(el => el != match.Value);

				text.Advance();
				return match;
			}

			return new NoMatch($"({text.Current})", string.Join(",", _pattern), text.CurrentIndex);
		}
	}
}