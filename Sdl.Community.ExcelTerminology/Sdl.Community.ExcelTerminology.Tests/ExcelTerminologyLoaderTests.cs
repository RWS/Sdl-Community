using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using Sdl.Community.ExcelTerminology.Services;
using Sdl.Community.ExcelTerminology.Tests.Helper;
using Xunit;

namespace Sdl.Community.ExcelTerminology.Tests
{
    public class ExcelTerminologyLoaderTests
    {

        [Fact]
        public async void Select_WorkSheet_By_Name()
        {
            var providerSettings = TestHelper.CreateProviderSettings();
            var excelPackage = TestHelper.CreateSampleExcelPackage(providerSettings);
            providerSettings.WorksheetName = "Glossary";
            var excelTerminologyService =
                new ExcelTermLoaderService(providerSettings);

            var worksheet = await excelTerminologyService.GetTerminologyWorksheet(excelPackage);

            Assert.Equal(worksheet.Name, providerSettings.WorksheetName);
        }

        [Fact]
        public async void Select_WorkSheet_By_Index()
        {
            var providerSettings = TestHelper.CreateProviderSettings();
            var excelPackage = TestHelper.CreateSampleExcelPackage(providerSettings);

            var excelTerminologyService =
                new ExcelTermLoaderService(providerSettings);

            var worksheet = await excelTerminologyService.GetTerminologyWorksheet(excelPackage);

            Assert.Equal(worksheet.Name, "Glossary");
        }

        [Fact]
        public async void Select_WorkSheet_Which_Doesnt_Exists()
        {
            var providerSettings = TestHelper.CreateProviderSettings();
            var excelPackage = TestHelper.CreateSampleExcelPackage(providerSettings);

            providerSettings.WorksheetName = "Worksheet";

            var excelTerminologyService =
                new ExcelTermLoaderService(providerSettings);

            var worksheet = await excelTerminologyService.GetTerminologyWorksheet(excelPackage);

            Assert.Equal(worksheet, null);
        }

        [Theory]
        [InlineData(10,
            "ill-treatment",
            "schlechte Behandlung|Misshandlung",
            "Approved|Not approved"
            ,31)]
        [InlineData(22,
            "unaccountable",
            "unerklärbar|unerfindlich",
            "Approved|Not approved"
            , 31)]
        public async void Get_All_Terms_With_Header(int id
            , string expectedSource
            , string expectedTarget
            , string expectedApproved
            , int expectedCount)
        {
            var providerSettings = TestHelper.CreateProviderSettings();
            var excelPackage = TestHelper.CreateSampleExcelPackage(providerSettings);

            var excelTerminologyService =
                new ExcelTermLoaderService(providerSettings);

            var worksheet = await excelTerminologyService.GetTerminologyWorksheet(excelPackage);
            var actual = await excelTerminologyService.GetTermsFromExcel(worksheet);
            Assert.Equal(actual.Count, expectedCount);
            var actualExcelTerm = actual[id];
            Assert.Equal(actualExcelTerm.Source, expectedSource);
            Assert.Equal(actualExcelTerm.SourceCulture.Name,
                providerSettings.SourceLanguage.Name);
            Assert.Equal(actualExcelTerm.Target, expectedTarget);
            Assert.Equal(actualExcelTerm.TargetCulture.Name,
                providerSettings.TargetLanguage.Name);
            Assert.Equal(actualExcelTerm.Approved, expectedApproved);
        }

        [Theory]
        [InlineData(9,
           "ill-treatment",
           "schlechte Behandlung|Misshandlung",
           null
           , 26)]
        [InlineData(21,
           "unaccountable",
           "unerklärbar|unerfindlich",
           null
           , 26)]
        public async void Get_All_Source_Terms_Without_Header(int id
           , string expectedSource
           , string expectedTarget
           , string expectedApproved
           , int expectedCount)
        {
            var providerSettings = TestHelper
                .CreateProviderSettingsWithouHeaderAndApproved();
            var excelPackage = TestHelper.CreateSampleExcelPackage(providerSettings);

            var excelTerminologyService =
                new ExcelTermLoaderService(providerSettings);

            var worksheet = await excelTerminologyService.GetTerminologyWorksheet(excelPackage);
            var actual = await excelTerminologyService.GetTermsFromExcel(worksheet);
            Assert.Equal(actual.Count, expectedCount);
            var actualExcelTerm = actual[id];
            Assert.Equal(actualExcelTerm.Source, expectedSource);
            Assert.Equal(actualExcelTerm.SourceCulture.Name,
                providerSettings.SourceLanguage.Name);
            Assert.Equal(actualExcelTerm.Target, expectedTarget);
            Assert.Equal(actualExcelTerm.TargetCulture.Name,
                providerSettings.TargetLanguage.Name);
            Assert.Equal(actualExcelTerm.Approved, expectedApproved);
        }

        [Theory]
        [InlineData(21, "test",
            "test")]
        [InlineData(22, "test1",
            "test1|test2")]
        [InlineData(45, "test1",
            "test1|test2")]
        public async void Add_Or_Update_Term(int id, string expectedSource, string expectedTarget)
        {
            var providerSettings = TestHelper
                .CreateProviderSettings();


            var excelTerminologyService =
                new ExcelTermLoaderService(providerSettings);
            var excelTerm = TestHelper.CreateExcelTerm(expectedSource,
                expectedTarget,
                "Approved",
                providerSettings);

           await excelTerminologyService.AddOrUpdateTerm(id, excelTerm);

            var actualTerms =await excelTerminologyService.LoadTerms();

            Assert.Equal(actualTerms[id].Source, expectedSource);

        }

        [Theory]
        [InlineData(45)]
        
        public async void Delete_Term(int id)
        {
            var providerSettings = TestHelper
                .CreateProviderSettings();


            var excelTerminologyService =
                new ExcelTermLoaderService(providerSettings);

            await excelTerminologyService.DeleteTerm(id);

            var actualTerms = await excelTerminologyService.LoadTerms();

            Assert.False(actualTerms.ContainsKey(id));

        }
    }
}
