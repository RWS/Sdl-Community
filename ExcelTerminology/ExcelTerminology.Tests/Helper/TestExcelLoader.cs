using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using ExcelTerminology.Model;
using ExcelTerminology.Services.Interfaces;

namespace ExcelTerminology.Tests.Helper
{
	public class TestExcelLoader : IExcelTermLoaderService
    {
        public async Task<Dictionary<int, ExcelTerm>> LoadTerms()
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

        public Task AddOrUpdateTerm(int entryId, ExcelTerm excelTerm)
        {
            throw new NotImplementedException();
        }

        public Task AddOrUpdateTerms(Dictionary<int, ExcelTerm> excelTerms)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTerm(int id)
        {
            throw new NotImplementedException();
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

