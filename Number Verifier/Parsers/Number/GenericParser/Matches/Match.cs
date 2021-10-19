using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches
{
	public class Match : IMatch
	{
		public Match(string match)
		{
			Value = match;
		}

		public string Value { get; set; }
		public bool Success { get; set; } = true;
		public string Name { get; set; }

		public override string ToString()
		{
			return Value;
		}
	}
}