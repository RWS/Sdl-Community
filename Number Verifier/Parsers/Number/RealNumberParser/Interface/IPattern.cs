using System.Collections.Generic;

namespace Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Interface
{
	public interface IPattern
	{
		IMatch Match(TextToParse text);
		List<IMatch> MatchAll(TextToParse text);
	}
}