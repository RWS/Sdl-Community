using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.Studio;
using InterpretBank.TermSearch;
using NSubstitute;
using Sdl.Core.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;
using Xunit;

namespace InterpretBankTests
{
	public class InterpretBankProviderTests
	{
		[Fact]
		public void SearchTermTest()
		{
			var termSearchService = Substitute.For<ITermSearchService>();
			var language = Substitute.For<ILanguage>();

			termSearchService
				.GetFuzzyTerms(default, default, default)
				.ReturnsForAnyArgs(new List<string> { "firstTerm", "secondTerm" });

			var sut = new InterpretBankProvider(termSearchService);

			var results = sut.Search("This is a text", language, language, 100, SearchMode.Fuzzy, true);


		}

	}
}
