using System.Collections.Generic;
using System.Threading.Tasks;
using Sdl.Community.TermExcelerator.Model;

namespace Sdl.Community.TermExcelerator.Services.Interfaces
{
	public interface IExcelTermLoaderService
    {
        Task<Dictionary<int, ExcelTerm>> LoadTerms();
        Task AddOrUpdateTerm(int entryId, ExcelTerm excelTerm);
        Task AddOrUpdateTerms(Dictionary<int, ExcelTerm> excelTerms);
        Task DeleteTerm(int id);
    }
}