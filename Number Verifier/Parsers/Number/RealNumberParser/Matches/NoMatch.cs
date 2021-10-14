using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Interface;

namespace Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Matches
{
	public class NoMatch : IMatch
	{
		public NoMatch(string noMatch = "", int index = 0) : this(noMatch, "", index)
		{
		}

		public NoMatch(string noMatch, string expected, int index)
		{
			Current = noMatch;
			IndexOfNoMatch = index;
			Expected = expected;
		}

		public string Current { get; set; }
		public string Expected { get; }

		public int IndexOfNoMatch { get; }
		public bool Success { get; set; } = false;

		public override string ToString()
		{
			return $"{Current}<{IndexOfNoMatch}>";
		}
	}
}