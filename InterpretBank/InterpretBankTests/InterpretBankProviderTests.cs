using System.Collections.Generic;
using InterpretBank.Studio;
using InterpretBank.TermSearch;
using NSubstitute;
using Sdl.Terminology.TerminologyProvider.Core;
using Xunit;

namespace InterpretBankTests
{
	public class InterpretBankProviderTests
	{
		public InterpretBankProviderTests()
		{
			var mockGenerator = new MockGenerator();
			TermSearchService = mockGenerator.GetTermSearchService();
		}

		private ITermSearchService TermSearchService { get; }

		[Fact]
		public void SearchTermTest()
		{
			//var termSearchService = Substitute.For<ITermSearchService>();
			var language = Substitute.For<ILanguage>();

			//termSearchService
			//	.GetFuzzyTerms(default, default, default)
			//	.ReturnsForAnyArgs(new List<string> { "firstTerm", "secondTerm" });

			var sut = new InterpretBankProvider(TermSearchService);

			var results = sut.Search("This is a text", language, language, 100, SearchMode.Fuzzy, true);
		}
	}
}