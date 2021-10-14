using System.Collections.Generic;
using System.Linq;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches
{
	public class MatchArray : IMatch
	{
		public string Value { get; set; }

		public void Flatten()
		{
			if (Matches == null || Matches.Count == 0) return;

			Matches?.ForEach(m => (m as MatchArray)?.Flatten());

			if (Matches.Count == 1 && Matches[0] is MatchArray matchArray) Matches = matchArray.Matches;

		}

		public List<IMatch> Matches { get; set; } = new List<IMatch>();

		public bool Success { get; set; }
		public string Name { get; set; }

		public void AddPattern(IMatch match)
		{
			var matchValue = match.ToString();
			if (matchValue.Length == 0) return;

			Value += matchValue;
			Matches.Add(match);
		}

		public IMatch this[string key] => Matches.FirstOrDefault(m => m.Name == key);

		public override string ToString()
		{
			var result = Matches.Aggregate(string.Empty, (current, match) => current + match);
			return result;
		}
	}
}