using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Patterns;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.Parsers.RealNumbers
{
	public class RealNumbersParserTests
	{
		[Theory]
		[InlineData("1 554,5 some word 1.234,5 another word -1 222,3", "1.554,5 test 1,234,5 another test word −1.222,3")]
		public void TestingTests(string source, string target)
		{
			var realNumber = new RealNumber();
			var match = realNumber.MatchAll(new TextToParse(source));
		}
	}
}