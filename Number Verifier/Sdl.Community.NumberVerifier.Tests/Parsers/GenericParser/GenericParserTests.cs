using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Patterns;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.Parsers.GenericParser
{
	public class GenericParserTests
	{
		[Theory]
		[InlineData("1 554,544,44 some word 1.23434343434343,5 another word -1 222,3", "1.554,5 test 1,234,5 another test word −1.222,3")]
		public void TestingTests(string source, string target)
		{
			var thousandSeparators = ", .";
			var decimalSeparators = ".,";

			var realNumber = new RealNumber(thousandSeparators, decimalSeparators);

			realNumber.MatchAll(new TextToParse(source));
			realNumber.MatchAll(new TextToParse(target));
			
		}
	}
}