using System.Collections.Generic;

namespace Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Interface
{
	public abstract class AbstractPattern : IPattern
	{
		public abstract IMatch Match(TextToParse text);

		public List<IMatch> MatchAll(TextToParse text)
		{
			var matchArray = new List<IMatch>();
			while (!text.IsAtEnd())
			{
				var currentMatch = Match(text);
				if (currentMatch.Success) matchArray.Add(currentMatch);
				else text.Advance();
			}

			return matchArray;
		}
	}
}