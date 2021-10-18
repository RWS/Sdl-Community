using System.Collections.Generic;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface
{
	public abstract class AbstractPattern : IPattern
	{
		public List<IMatch> MatchArray { get; set; }
		public abstract IMatch Match(TextToParse text);

		public void MatchAll(TextToParse text)
		{
			MatchArray = new List<IMatch>();
			while (!text.IsAtEnd())
			{
				var currentMatch = Match(text);
				if (currentMatch?.Success ?? false) MatchArray.Add(currentMatch);
				else text.Advance();
			}
		}

		public string Name { get; set; }
	}
}