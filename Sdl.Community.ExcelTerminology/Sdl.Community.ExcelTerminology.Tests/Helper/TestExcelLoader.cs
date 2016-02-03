using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.ExcelTerminology.Model;
using Sdl.Community.ExcelTerminology.Services;
using Sdl.Community.ExcelTerminology.Services.Interfaces;

namespace Sdl.Community.ExcelTerminology.Tests.Helper
{
    public class TestExcelLoader : IExcelTermLoaderService
    {
        public Dictionary<int, ExcelTerm> LoadTerms()
        {
            return new Dictionary<int, ExcelTerm>
            {
                {10,  SampleExcelTerm},
                {22, new ExcelTerm{
                    Source= "unaccountable",
                    SourceCulture = CultureInfo.CreateSpecificCulture("en-US"),
                    Target ="unerklärbar|unerfindlich",
                    TargetCulture = CultureInfo.CreateSpecificCulture("de-De"),
                    Approved = "Approved|Not approved"
                } }
            };

        }

        public static ExcelTerm SampleExcelTerm => new ExcelTerm
        {
            Source = "ill-treatment",
            SourceCulture = CultureInfo.CreateSpecificCulture("en-US"),
            Target = "schlechte Behandlung|Misshandlung",
            TargetCulture = CultureInfo.CreateSpecificCulture("de-De"),
            Approved = "Approved|Not approved"
        };
    }
}

