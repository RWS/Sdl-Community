using System.Linq;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Matches;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Patterns.Specialized;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.Parsers.GenericParser
{
	public class GenericParserTests
	{
		[Theory]
		[InlineData("1 554,544,44 some word 1.23434343434343,5 another word -1 222,3")]
		public void RealNumberPattern_MatchTest(string source)
		{
			var thousandSeparators = ", .".Select(c => c.ToString()).ToList();
			var decimalSeparators = ".,".Select(c => c.ToString()).ToList();

			var realNumberPattern = new RealNumber(thousandSeparators, decimalSeparators);

			realNumberPattern.MatchAll(source);
			Assert.Collection(realNumberPattern.MatchArray,
				item =>
				{
					Assert.Equal(expected: "1 554,544", item.Value);
					Assert.Collection(((MatchArray)item).Matches,
						subItem =>
						{
							Assert.Equal(expected: "Integer", subItem.Name);
							Assert.Equal(expected: "1 554", subItem.Value);
							Assert.Collection(((MatchArray)subItem).Matches,
								thousandSeparator => Assert.Equal(expected: " ", thousandSeparator.Value));
						},
						subItem =>
						{
							Assert.Equal(expected: "Fraction", subItem.Name);
							Assert.Equal(expected: ",544", subItem.Value);
							Assert.Collection(((MatchArray)subItem).Matches,
								decimalSeparator => Assert.Equal(expected: ",", decimalSeparator.Value));
						});
				},
				item =>
				{
					Assert.Equal(expected: "44", item.Value);
					Assert.Collection(((MatchArray)item).Matches,
						subItem =>
						{
							Assert.Equal(expected: "Integer", subItem.Name);
							Assert.Equal(expected: "44", subItem.Value);
						});
				},
				item =>
				{
					Assert.Equal(expected: "1.234", item.Value);
					Assert.Collection(((MatchArray)item).Matches,
						subItem =>
						{
							Assert.Equal(expected: "Integer", subItem.Name);
							Assert.Equal(expected: "1.234", subItem.Value);
							Assert.Collection(((MatchArray)subItem).Matches,
								thousandSeparator => Assert.Equal(expected: ".", thousandSeparator.Value));
						});
				},
				item =>
				{
					Assert.Equal(expected: "34343434343,5", item.Value);
					Assert.Collection(((MatchArray)item).Matches,
						subItem =>
						{
							Assert.Equal(expected: "Integer", subItem.Name);
							Assert.Equal(expected: "34343434343", subItem.Value);
							Assert.True(subItem is Match);
						},
						subItem =>
						{
							Assert.Equal(expected: "Fraction", subItem.Name);
							Assert.Equal(expected: ",5", subItem.Value);
							Assert.Collection(((MatchArray)subItem).Matches,
								decimalSeparator => Assert.Equal(expected: ",", decimalSeparator.Value));
						});
				},
				item =>
				{
					Assert.Equal(expected: "-1 222,3", item.Value);
					Assert.Collection(((MatchArray)item).Matches,
						subItem =>
						{
							Assert.Equal(expected: "Sign", subItem.Name);
							Assert.Equal(expected: "-", subItem.Value);
						},
						subItem =>
						{
							Assert.Equal(expected: "Integer", subItem.Name);
							Assert.Equal(expected: "1 222", subItem.Value);

							var integerParts = ((MatchArray)subItem).Matches;
							Assert.Collection(integerParts,
								
								ip =>
								{
									Assert.Equal(expected: "ThousandSeparator", ip.Name);
									Assert.Equal(expected: " ", ip.Value);
								});
						},
						subItem =>
						{
							Assert.Equal(expected: "Fraction", subItem.Name);
							Assert.Equal(expected: ",3", subItem.Value);
							Assert.Collection(((MatchArray)subItem).Matches,
								decimalSeparator => Assert.Equal(expected: ",", decimalSeparator.Value));
						});
				});

			var realNumbers = realNumberPattern.ToNumberList();
			Assert.Collection(realNumbers,
				item => Assert.Equal(expected: "1 554,544", item),
				item => Assert.Equal(expected: "44", item),
				item => Assert.Equal(expected: "1.234", item),
				item => Assert.Equal(expected: "34343434343,5", item),
				item => Assert.Equal(expected: "-1 222,3", item));

			var realNumbersNormalized = realNumberPattern.ToNormalizedNumberList();
			Assert.Collection(realNumbersNormalized,
				item => Assert.Equal(expected: "1t554d544", item),
				item => Assert.Equal(expected: "44", item),
				item => Assert.Equal(expected: "1t234", item),
				item => Assert.Equal(expected: "34343434343d5", item),
				item => Assert.Equal(expected: "s1t222d3", item));
		}
	}
}