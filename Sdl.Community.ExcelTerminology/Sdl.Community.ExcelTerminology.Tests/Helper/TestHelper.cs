using System;
using System.Globalization;
using System.IO;
using OfficeOpenXml;
using Sdl.Community.ExcelTerminology.Model;

namespace Sdl.Community.ExcelTerminology.Tests.Helper
{
    public class TestHelper
    {
        public static ProviderSettings CreateProviderSettings()
        {
            return new ProviderSettings
            {
                SourceColumn = "A",
                TargetColumn = "B",
                ApprovedColumn = "C",
                HasHeader = true,
                Separator = '|',
                SourceLanguage = CultureInfo.CreateSpecificCulture("en-US"),
                TargetLanguage = CultureInfo.CreateSpecificCulture("de-DE"),
                TermFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    @"Resources\glossary_example.xlsx")
                // TermFilePath = @"C:\Temp\en-nl (large glossary example).xlsx"
                // TermFilePath = @"C:\Users\rocrisan\Documents\My Received Files\IATE - (en-nl).xlsx"

            };
        }

        public static ExcelPackage CreateSampleExcelPackage()
        {
            return new ExcelPackage(new FileInfo(CreateProviderSettings().TermFilePath));
        }

        public static ProviderSettings CreateProviderSettingsWithouHeaderAndApproved()
        {
            return new ProviderSettings
            {
                SourceColumn = "A",
                TargetColumn = "B",
                HasHeader = false,
                Separator = '|',
                SourceLanguage = CultureInfo.CreateSpecificCulture("en-US"),
                TargetLanguage = CultureInfo.CreateSpecificCulture("de-DE"),
                TermFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    @"Resources\glossary_example_without_header_and_approved.xlsx")
            };
        }
    }
}
