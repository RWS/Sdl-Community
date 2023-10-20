using System.Collections.Generic;
using System.Data.SQLite;
using InterpretBank.GlossaryService;
using InterpretBank.Interface;
using InterpretBank.SettingsService;
using InterpretBank.SettingsService.ViewModel;
using InterpretBank.Studio;
using InterpretBank.TerminologyService;
using InterpretBank.TerminologyService.Interface;
using NSubstitute;
using Sdl.Terminology.TerminologyProvider.Core;
using Xunit;

namespace InterpretBankTests
{
	public class InterpretBankProviderTests
	{
		public InterpretBankProviderTests()
		{
			//var mockGenerator = new MockGenerator();
		}

		private ITerminologyService TermSearchService { get; }

		[Fact(Skip = "for now...")]
		public void SearchTermTest()
		{
			//var termSearchService = Substitute.For<ITermSearchService>();
			var language = Substitute.For<ILanguage>();

			//termSearchService
			//	.GetFuzzyTerms(default, default, default)
			//	.ReturnsForAnyArgs(new List<string> { "firstTerm", "secondTerm" });

			var openFileDialog = Substitute.For<IUserInteractionService>();

			var filepath = "C:\\Code\\RWS Community\\InterpretBank\\InterpretBankTests\\Resources\\InterpretBankDatabaseV6.db";
			//var sqLiteConnection = new SQLiteConnection($"Data Source={filepath}");

			var ibContext = new InterpretBankDataContext();
			ibContext.Setup(filepath);

			var termSearchService = new TerminologyService(ibContext);

			var sut = new InterpretBankProvider(termSearchService, new Settings());

			var results = sut.Search("This is a text", language, language, 100, SearchMode.Fuzzy, true);
		}
	}
}