using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface
{
	public interface IPattern
	{
		string Name { get; set; }

		IMatch Match(TextToParse text);

		void MatchAll(TextToParse text);
	}
}