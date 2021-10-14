using System.Collections.Generic;
using System.Linq;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Interface;

namespace Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Matches
{
	public class MatchArray : IMatch
	{
		public string Current { get; set; }
		public List<IMatch> Matches { get; set; } = new List<IMatch>();

		public bool Success { get; set; }

		public void AddPattern(IMatch match)
		{
			var matchValue = match.ToString();
			if (matchValue.Length == 0) return;

			Current += matchValue;
			Matches.Add(match);
		}

		public override string ToString()
		{
			var result = Matches.Aggregate(string.Empty, (current, match) => current + match);
			return result;
		}
	}
}