using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.ExcelTerminology.Services;
using Xunit;

namespace Sdl.Community.ExcelTerminology.Tests
{
    public class ReadExcelTerminologyTests
    {

        [Fact]
        public void Select_WorkSheet_By_Name()
        {
            var providerSettings = TestHelper.CreateProviderSettings();
            providerSettings.WorksheetName = "Glossary";
            var excelTerminologyService =
                new ReadExcelTerminologyService(providerSettings);

            var worksheet = excelTerminologyService.GetTerminologyWorksheet();

            Assert.Equal(worksheet.Name, providerSettings.WorksheetName);
        }

        [Fact]
        public void Select_WorkSheet_By_Index()
        {
            var providerSettings = TestHelper.CreateProviderSettings();
            var excelTerminologyService =
                new ReadExcelTerminologyService(providerSettings);

            var worksheet = excelTerminologyService.GetTerminologyWorksheet();

            Assert.Equal(worksheet.Name, "Glossary");
        }

        [Fact]
        public void Select_WorkSheet_Which_Doesnt_Exists()
        {
            var providerSettings = TestHelper.CreateProviderSettings();
            providerSettings.WorksheetName = "Worksheet";

            var excelTerminologyService =
                new ReadExcelTerminologyService(providerSettings);

            var worksheet = excelTerminologyService.GetTerminologyWorksheet();

            Assert.Equal(worksheet, null);
        }

        [Fact]
        public void Get_All_Source_Terms_With_Header()
        {
            var providerSettings = TestHelper.CreateProviderSettings();

            var excelTerminologyService =
                new ReadExcelTerminologyService(providerSettings);

            var worksheet = excelTerminologyService.GetTerminologyWorksheet();
            var expected = excelTerminologyService.GetTerms(worksheet);

            Assert.Equal(expected.Count, 26);
        }

        [Fact]
        public void Get_All_Source_Terms_Without_Header()
        {
            var providerSettings = TestHelper.CreateProviderSettings();
            providerSettings.HasHeader = false;
            var excelTerminologyService =
                new ReadExcelTerminologyService(providerSettings);

            var worksheet = excelTerminologyService.GetTerminologyWorksheet();
            var expected = excelTerminologyService.GetTerms(worksheet);

            Assert.Equal(expected.Count, 27);
        }
    }
}
