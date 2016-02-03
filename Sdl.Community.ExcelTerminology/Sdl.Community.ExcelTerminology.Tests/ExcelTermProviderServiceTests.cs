using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var transformerService = new EntryTransformerService(parser);
            var actualTerms = excelTermLoaderService.LoadTerms();
            var excelTermProviderService =
                new ExcelTermProviderService(excelTermLoaderService, transformerService);
            var sw = Stopwatch.StartNew();
            var expected = excelTermProviderService.LoadEntries();
            sw.Stop();
            var el = sw.Elapsed;
            Assert.Equal(expected.Count, actualTerms.Count);

        }

       
    }
}
