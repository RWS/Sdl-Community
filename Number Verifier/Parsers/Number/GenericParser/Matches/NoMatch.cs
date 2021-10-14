using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches
{
	public class NoMatch : IMatch
	{
		public NoMatch(string noMatch = "", int index = 0) : this(noMatch, "", index)
		{
		}

		public NoMatch(string noMatch, string expected, int index)
		{
			Value = noMatch;
			IndexOfNoMatch = index;
			Expected = expected;
		}

		public string Value { get; set; }
		
		public string Expected { get; }

		public int IndexOfNoMatch { get; }
		public bool Success { get; set; } = false;
		public string Name { get; set; }

		public override string ToString()
		{
			return $"{Value}<{IndexOfNoMatch}>";
		}
	}
}