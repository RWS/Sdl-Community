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
            var sampleExcelTerm = TestExcelLoader.SampleExcelTerm;

        }
    }
}
