using System.Collections.Generic;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;

namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface
{
	public abstract class AbstractPattern : IPattern
	{
		public List<IMatch> Matches { get; set; }
		public abstract IMatch Match(TextToParse text);

		public void Reset() => Matches = new List<IMatch>();

		public void MatchAll(string text)
		{
			Reset();
			if (text is null) return;
				MatchAll(new TextToParse(text));
		}

		private void MatchAll(TextToParse text)
		{
			while (!text.IsAtEnd())
			{
				var currentMatch = Match(text);
				if (currentMatch.Value != "NoMoreText" && (currentMatch?.Success ?? false)) Matches.Add(currentMatch);
				else text.Advance();
			}
		}

		public string Name { get; set; }
	}
}