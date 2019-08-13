using Sdl.Community.ExcelTerminology.Services;
using Sdl.Community.ExcelTerminology.Tests.Helper;
using Xunit;

namespace Sdl.Community.ExcelTerminology.Tests
{
	public class TransformerServiceTests
	{
		[Fact]
		public void Create_Entry_Languages()
		{
			//arrange
			var sampleExcelTerm = TestExcelLoader.SampleExcelTerm;
			var providerSettings = TestHelper.CreateProviderSettings();
			var parser = new Parser(providerSettings);
			var transformerService = new EntryTransformerService(parser);

			//act
			var entryLanguages = transformerService.CreateEntryLanguages(sampleExcelTerm);

			//assert
			Assert.Equal(entryLanguages.Count, 2);
			Assert.Equal(entryLanguages[0].Locale, sampleExcelTerm.SourceCulture);
			Assert.Equal(entryLanguages[1].Locale, sampleExcelTerm.TargetCulture);

		}

		[Theory]
		[InlineData("Approved|Not approved|Approved", 2, "Approved")]
		[InlineData("Approved|Not approved", 1, "Not approved")]
		[InlineData("Approved", 0, "Approved")]
		public void Create_Entry_Term_Fields(string approved, int fieldNumber, string expected)
		{
			//arrange
			var providerSettings = TestHelper.CreateProviderSettings();
			var parser = new Parser(providerSettings);
			var transformerService = new EntryTransformerService(parser);
			var approvals = parser.Parse(approved);
			//act
			var fields = transformerService.CreateEntryTermFields(fieldNumber, approvals);
			//assert
			Assert.Equal(fields[0].Value, expected);
		}

		[Theory]
		[InlineData("hobby-horse|hobbyhorse", 2)]
		[InlineData("schlechte Behandlung|Misshandlung", 2)]
		public void Create_Entry_Term(string terms, int numberOfTerms)
		{
			//arrange
			var providerSettings = TestHelper.CreateProviderSettings();
			var parser = new Parser(providerSettings);
			var transformerService = new EntryTransformerService(parser);

			//act
			var entryTerms = transformerService.CreateEntryTerms(terms);
			//assert
			Assert.Equal(entryTerms.Count, numberOfTerms);
		}

		[Theory]
		[InlineData("hobby-horse|hobbyhorse"
			, "Approved|Not approved"
			, 2)]
		public void Create_Entry_Terms_With_Fields(string terms
			, string approved
			, int numberOfTerms)
		{
			//arrange
			var providerSettings = TestHelper.CreateProviderSettings();
			var parser = new Parser(providerSettings);
			var transformerService = new EntryTransformerService(parser);

			//act
			var entryTerms = transformerService.CreateEntryTerms(terms, approved);
			//assert
			Assert.Equal(entryTerms.Count, numberOfTerms);
			var entryTerm = entryTerms[entryTerms.Count - 1];
			Assert.True(entryTerm.Fields.Count > 0);
		}
	}
}