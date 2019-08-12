using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelTerminology.Model;
using ExcelTerminology.Services;
using ExcelTerminology.Tests.Helper;
using Sdl.Terminology.TerminologyProvider.Core;
using Xunit;

namespace ExcelTerminology.Tests
{
    public class NormalTermSearchServiceTest
    {
        [Theory]
        [InlineData("unaccountable", 22)]
        [InlineData("jodhpurs", 11)]

        public async void Search_Term_One_Result(string text, int expectedId)
        {
            //arrange
            var providerSettings = TestHelper.CreateProviderSettings();
            var parser = new Parser(providerSettings);
            var excelTermLoaderService = new ExcelTermLoaderService(providerSettings);
            var entryTransformer = new EntryTransformerService(parser);
            var excelTermProviderService = new ExcelTermProviderService(excelTermLoaderService, entryTransformer);

            var termEntries = await excelTermProviderService.LoadEntries();
           

            var termSearchService = new NormalTermSeachService(providerSettings);

            //act
            var results = termSearchService.Search(text, termEntries, 1);

            //assert
            Assert.Equal(results.Count, 1);
            var actualResult = results.First();
            Assert.Equal(expectedId, actualResult.Id );
            Assert.Equal(providerSettings.SourceLanguage, 
                actualResult.Language.Locale);

        }

        [Theory]
        [InlineData("Register today   and obtain a 20% discount!",3)]
        public async void Search_Term_Phrase_Multiple_Results(string text,
            int expectedNumberResults)
        {
            //arrange
            var providerSettings = TestHelper.CreateProviderSettings();
            var parser = new Parser(providerSettings);
            var excelTermLoaderService = new ExcelTermLoaderService(providerSettings);
            var entryTransformer = new EntryTransformerService(parser);
            var excelTermProviderService = new ExcelTermProviderService(excelTermLoaderService, entryTransformer);

            var termEntries = await excelTermProviderService.LoadEntries();


            var termSearchService = new NormalTermSeachService(providerSettings);

            //act
            var results = termSearchService.Search(text, termEntries, expectedNumberResults);

            //assert
            Assert.Equal(results.Count, expectedNumberResults);


        }

        [Theory]
        [InlineData("I have no idea what hobby-horse is", 1)]
        public async void Search_Term_Phrase_With_Separator(string text,
            int expectedNumberResults)
        {
            //arrange
            var providerSettings = TestHelper.CreateProviderSettings();
            var parser = new Parser(providerSettings);
            var excelTermLoaderService = new ExcelTermLoaderService(providerSettings);
            var entryTransformer = new EntryTransformerService(parser);
            var excelTermProviderService = new ExcelTermProviderService(excelTermLoaderService, entryTransformer);

            var termEntries = await excelTermProviderService.LoadEntries();


            var termSearchService = new NormalTermSeachService(providerSettings);

            //act
            var results = termSearchService.Search(text, termEntries, expectedNumberResults);

            //assert
            Assert.Equal(results.Count, expectedNumberResults);


        }
    }
}
