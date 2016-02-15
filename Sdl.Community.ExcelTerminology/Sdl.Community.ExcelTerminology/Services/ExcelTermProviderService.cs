using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.ExcelTerminology.Insights;
using Sdl.Community.ExcelTerminology.Model;
using Sdl.Community.ExcelTerminology.Services.Interfaces;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology.Services
{
    public class ExcelTermProviderService
    {
        private readonly IExcelTermLoaderService _excelTermLoaderService;
        private readonly IEntryTransformerService _transformerService;

        public ExcelTermProviderService(IExcelTermLoaderService excelTermLoaderService
            , IEntryTransformerService transformerService)
        {
            _excelTermLoaderService = excelTermLoaderService;
            _transformerService = transformerService;
        }


        public async Task<List<ExcelEntry>> LoadEntries()
        {
            var excelTerms = await _excelTermLoaderService.LoadTerms();

            return excelTerms
                .Where(et => !string.IsNullOrEmpty(et.Value.Source))
                .Select(excelTerm => new ExcelEntry
                {
                    Id = excelTerm.Key,
                    Fields = new List<IEntryField>(),
                    Languages = _transformerService.CreateEntryLanguages(excelTerm.Value),
                    SearchText = excelTerm.Value.Source

                }).ToList();
        }

        public async Task AddOrUpdateEntry(int entryId, ExcelTerm excelEntry)
        {

            if (!string.IsNullOrWhiteSpace(excelEntry.Source) && !string.IsNullOrWhiteSpace(excelEntry.Target))
            {
                await _excelTermLoaderService.AddOrUpdateTerm(entryId, excelEntry);
            }
        }

        public async Task DeleteEntry(int entryId)
        {

            await _excelTermLoaderService.DeleteTerm(entryId);

        }

    }
}
