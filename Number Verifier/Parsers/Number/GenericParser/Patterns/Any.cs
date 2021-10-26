using System.Collections.Generic;
using System.Linq;
using Sdl.Community.NumberVerifier.Helpers;
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

			var noMatch = new NoMatch($"({text.Current})", string.Join(",", _pattern), text.CurrentIndex);
			var patterns = _pattern.ToList();
			patterns.Remove(Constants.NoSeparator);

			var charPatterns = patterns.Where(p => p.Length == 1);
			if (charPatterns.Contains(text.Current.ToString()))
			{
				var match = new Match(text.Current.ToString())
				{
					Name = Name
				};

				if (_keepConsistent && _pattern.Count > 1) _pattern.RemoveAll(el => el != match.Value);

				text.Advance();
				return match;
			}

			var stringPatterns = patterns.ToList();
			stringPatterns.RemoveAll(p => charPatterns.Contains(p));
			foreach (var sp in stringPatterns)
			{
				var currentSpLength = sp.Length;
				var isValidPositionInString = text.CurrentIndex + currentSpLength < text.Text.Length;

				if (isValidPositionInString && sp == text.Text.Substring(text.CurrentIndex, currentSpLength))
				{
					var match = new Match(sp)
					{
						Name = Name
					};

					if (_keepConsistent && _pattern.Count > 1) _pattern.RemoveAll(el => el != match.Value);

					text.Advance(currentSpLength);
					return match;
				}
			}

			if (_pattern.Contains(Constants.NoSeparator)) return new Match(string.Empty);

			return noMatch;
		}
	}
}