using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.ExcelTerminology.Services;
using Sdl.Community.ExcelTerminology.Tests.Helper;
using Xunit;

namespace Sdl.Community.ExcelTerminology.Tests
{
    public class ExcelTermProviderServiceTests
    {
        [Fact]
        public void Load_Entries()
        {
            var providerSettings = TestHelper.CreateProviderSettings();
            var excelTermLoaderService = new TestExcelLoader();
            var parser = new Parser(providerSettings);
            var actualTerms = excelTermLoaderService.LoadTerms();
            var excelTermProviderService =
                new ExcelTermProviderService(excelTermLoaderService, parser);

            var expected = excelTermProviderService.LoadEntries();

            Assert.Equal(expected.Count, actualTerms.Count);

        }

        [Fact]
        public void Create_Entry_Languages()
        {
            //arrange
            var sampleExcelTerm = TestExcelLoader.SampleExcelTerm;
            var providerSettings = TestHelper.CreateProviderSettings();
            var excelTermLoaderService = new TestExcelLoader();
            var parser = new Parser(providerSettings);
            var excelTermProviderService =
                new ExcelTermProviderService(excelTermLoaderService, parser);
            //act
            var entryLanguages = excelTermProviderService
                .CreateEntryLanguages(sampleExcelTerm);

            //assert
            Assert.Equal(entryLanguages.Count, 2);
            Assert.Equal(entryLanguages[0].Locale, sampleExcelTerm.SourceCulture);
            Assert.Equal(entryLanguages[1].Locale, sampleExcelTerm.TargetCulture);

        }

        [Theory]
        [InlineData("Approved|Not approved|Approved",3)]
        [InlineData("Approved|Not approved",2)]
        [InlineData("Approved",1)]
        [InlineData(null,0)]
        public void Create_Entry_Term_Fields(string approved, int numberOfFields)
        {
            //arrange
            var providerSettings = TestHelper.CreateProviderSettings();
            var excelTermLoaderService = new TestExcelLoader();
            var parser = new Parser(providerSettings);
            var excelTermProviderService =
                new ExcelTermProviderService(excelTermLoaderService, parser);
            //act
            var fields = excelTermProviderService.CreateEntryTermFields(approved);
            //assert
            Assert.Equal(fields.Count, numberOfFields);
        }

        [Theory]
        [InlineData("hobby-horse|hobbyhorse",2)]
        [InlineData("schlechte Behandlung|Misshandlung", 2)]
        public void Create_Entry_Term(string terms, int numberOfTerms)
        {
            //arrange
            var providerSettings = TestHelper.CreateProviderSettings();
            var excelTermLoaderService = new TestExcelLoader();
            var parser = new Parser(providerSettings);
            var excelTermProviderService =
                new ExcelTermProviderService(excelTermLoaderService, parser);
            //act
            var entryTerms = excelTermProviderService.CreateEntryTerms(terms);
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
            var excelTermLoaderService = new TestExcelLoader();
            var parser = new Parser(providerSettings);
            var excelTermProviderService =
                new ExcelTermProviderService(excelTermLoaderService, parser);
            //act
            var entryTerms = excelTermProviderService.CreateEntryTerms(terms,approved);
            //assert
            Assert.Equal(entryTerms.Count, numberOfTerms);
            var entryTerm = entryTerms[entryTerms.Count - 1];
            Assert.True(entryTerm.Fields.Count > 0);

        }
    }
}
