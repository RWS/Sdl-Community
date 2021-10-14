using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Interface;

namespace Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Matches
{
	public class NoMoreText : IMatch
	{
		public NoMoreText() : this("")
		{ }

		public NoMoreText(string expected)
		{
			Expected = expected;
		}

		public string Expected { get; }

		public bool Success { get; set; } = false;

		public override string ToString()
		{
			return "NoMoreText";
		}
	}
}