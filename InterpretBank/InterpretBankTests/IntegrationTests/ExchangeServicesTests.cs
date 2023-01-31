using System.Linq;
using InterpretBank.GlossaryExchangeService;
using Xunit;
using Xunit.Abstractions;

namespace InterpretBankTests.IntegrationTests
{
	public class GlossaryExchangeServiceTests
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public GlossaryExchangeServiceTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
			GlossaryExchangeService = new GlossaryExchangeService();
		}

		//TODO: Test with an actual import into the db
		//TODO: Try to handle/catch all exceptions in GES

		private GlossaryExchangeService GlossaryExchangeService { get; }

		[Fact]
		public void ImportExportBetweenFormats()
		{
			var path = "..\\..\\Resources\\Glossary1.xlsx";
			var savePath = "..\\..\\Resources\\SavedGlossary.tbx";

			var terms = GlossaryExchangeService.ImportTerms(path).ToList();

			GlossaryExchangeService.ExportTerms(Format.Tbx, savePath, terms);

			var terms2 = GlossaryExchangeService.ImportTerms(savePath).ToList();

			Assert.Equal(terms.Count, terms2.Count);

			//Ignore table header since they are not necessarily equal
			for (var i = 1; i < terms.Count; i++)
			{
				Assert.Equal(terms[i], terms2[i]);
			}
		}

		[Fact]
		public void LoadSaveExcelGlossary()
		{
			var path = "..\\..\\Resources\\Glossary1.xlsx";
			var savePath = "..\\..\\Resources\\SavedGlossary.xlsx";

			var terms = GlossaryExchangeService.ImportTerms(path).ToList();

			GlossaryExchangeService.ExportTerms(Format.Excel, savePath, terms);

			var terms2 = GlossaryExchangeService.ImportTerms(savePath).ToList();

			Assert.Equal(terms.Count, terms2.Count);

			for (var i = 0; i < terms.Count; i++)
			{
				Assert.Equal(terms[i], terms2[i]);
			}
		}

		[Fact]
		public void LoadSaveTbxGlossary()
		{
			var path = "..\\..\\Resources\\exp.tbx";
			var savePath = "..\\..\\Resources\\exp2.tbx";

			var terms = GlossaryExchangeService.ImportTerms(path).ToList();

			GlossaryExchangeService.ExportTerms(Format.Tbx, savePath, terms, "Glossary", "SubGlossary");

			var terms2 = GlossaryExchangeService.ImportTerms(savePath).ToList();

			Assert.Equal(terms.Count, terms2.Count);

			for (var i = 0; i < terms.Count; i++)
			{
				Assert.Equal(terms[i], terms2[i]);
			}
		}
	}
}