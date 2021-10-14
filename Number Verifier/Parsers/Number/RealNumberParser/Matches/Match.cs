using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Interface;

namespace Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Matches
{
	public class Match : IMatch
	{
		public Match(string match)
		{
			Current = match;
		}

		public string Current { get; set; }
		public bool Success { get; set; } = true;

		public override string ToString()
		{
			return Current;
		}
	}
}