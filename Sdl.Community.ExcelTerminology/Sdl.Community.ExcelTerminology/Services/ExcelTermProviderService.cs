using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void AddEntry(ExcelTerm excelEntry,int entryId)
        {
            //TODO: Make it do something
        }

        public void UpdateEntry(ExcelTerm excelTerm,int entryId)
        {        
            //var excelEntry = new ExcelEntry
            //{
            //    Id = entryId,
            //    Fields = new List<IEntryField>(),
            //    Languages = _transformerService.CreateEntryLanguages(excelTerm),
            //    SearchText = excelTerm.Source
            //};

        }

        public void DeleteEntry(int entryId)
        {
            //TODO: Make it do something
        }
       
    }
}
