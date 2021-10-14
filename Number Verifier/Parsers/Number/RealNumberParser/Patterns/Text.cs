using System.Linq;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Interface;

namespace Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Patterns
{
	public class Text : AbstractPattern
	{
		private readonly Sequence _pattern;

		public Text(string pattern)
		{
			_pattern = new Sequence(pattern.Select(character => new Character(character)).ToArray());
		}

		public override IMatch Match(TextToParse text)
		{
			return _pattern.Match(text);
		}
	}
}