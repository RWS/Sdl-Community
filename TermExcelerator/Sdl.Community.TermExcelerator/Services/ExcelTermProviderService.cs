using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sdl.Community.TermExcelerator.Model;
using Sdl.Community.TermExcelerator.Services.Interfaces;

namespace Sdl.Community.TermExcelerator.Services
{
	public class ExcelTermProviderService
	{
		private readonly IExcelTermLoaderService _excelTermLoaderService;
		private readonly EntryTransformerService _transformerService;

		public ExcelTermProviderService(IExcelTermLoaderService excelTermLoaderService, EntryTransformerService transformerService)
		{
			_excelTermLoaderService = excelTermLoaderService;
			_transformerService = transformerService;
		}

		public async Task AddOrUpdateEntries(Dictionary<int, ExcelTerm> excelEntries)
		{
			await _excelTermLoaderService.AddOrUpdateTerms(excelEntries);
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
			if (entryId == 0) return;
			await _excelTermLoaderService.DeleteTerm(entryId);
		}

		public async Task<List<ExcelEntry>> LoadEntries()
		{
			var excelTerms = await _excelTermLoaderService.LoadTerms();

			return excelTerms
				.Where(et => !string.IsNullOrEmpty(et.Value.Source))
				.Select(excelTerm => _transformerService.CreateExcelEntry(excelTerm.Value, excelTerm.Key))
				.ToList();
		}
	}
}