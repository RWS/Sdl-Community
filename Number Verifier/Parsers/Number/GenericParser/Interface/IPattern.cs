using System.Collections.Generic;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface
{
	public interface IPattern
	{
		IMatch Match(TextToParse text);
		List<IMatch> MatchAll(TextToParse text);
		string Name{ get; set; }
	}
}