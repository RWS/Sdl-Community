using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.ExcelTerminology.Model;

namespace Sdl.Community.ExcelTerminology.Tests
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
                SourceLanguage = "en-US",
                TargetLanguage = "de-DE",
                TermFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    @"Resources\glossary_example.xlsx")
            };
        }

        public static ProviderSettings CreateProviderSettingsWithouHeaderAndApproved()
        {
            return new ProviderSettings
            {
                SourceColumn = "A",
                TargetColumn = "B",
                HasHeader = false,
                Separator = '|',
                SourceLanguage = "en-US",
                TargetLanguage = "de-DE",
                TermFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    @"Resources\glossary_example_without_header_and_approved.xlsx")
            };
        }
    }
}
