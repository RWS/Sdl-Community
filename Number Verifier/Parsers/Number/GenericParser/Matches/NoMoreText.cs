using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches
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
		public string Name { get; set; }
		public string Value => string.Empty;

		public override string ToString()
		{
			return "NoMoreText";
		}
	}
}