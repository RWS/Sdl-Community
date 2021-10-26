using System.Collections.Generic;
using System.Linq;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches
{
	public class MatchArray : IMatch
	{
		public string Value { get; set; }

		public IMatch FlattenByNamedGroups()
		{
			var newMatches = Matches.Select(item => item is MatchArray matchArray ? matchArray.FlattenByNamedGroups() : item).ToList();

			newMatches.RemoveAll(m => m?.Name == null);
			RemoveDuplicates(newMatches);

			if (Name == null)
			{
				switch (newMatches.Count)
				{
					case 0:
						return null;
					case 1:
						return newMatches[0];
				}
			}

			if (newMatches.Count == 0) return new Match(Value) { Name = Name };

			Matches = newMatches;

			Name ??= "Matches";

			return this;
		}

		private static void RemoveDuplicates(List<IMatch> newMatches)
		{
			var duplicatesIndexes = new List<int>();
			for (var i = 0; i < newMatches.Count - 1 && !duplicatesIndexes.Contains(i); i++)
				for (var j = 1; j < newMatches.Count && j != i; j++)
				{
					if (newMatches[i].Name == newMatches[j].Name) duplicatesIndexes.Add(j);
				}
			duplicatesIndexes.ForEach(newMatches.RemoveAt);
		}

		public List<IMatch> Matches { get; set; } = new();

		public bool Success { get; set; }
		public string Name { get; set; }

		public void AddPattern(IMatch match)
		{
			var matchValue = match.ToString();
			if (matchValue.Length == 0) return;

			Value += matchValue;
			Matches.Add(match);
		}

		public IMatch this[string key]
		{
			get
			{
				var matchfound = Matches.FirstOrDefault(m => m.Name == key);
				return matchfound ??
				       Matches.Where(m => m is MatchArray).Cast<MatchArray>().Select(matchArray => matchArray[key]).FirstOrDefault(
					       subArray => subArray is not null);
			}
		}

		public override string ToString()
		{
			var result = Matches.Aggregate(string.Empty, (current, match) => current + match);
			return result;
		}
	}
}