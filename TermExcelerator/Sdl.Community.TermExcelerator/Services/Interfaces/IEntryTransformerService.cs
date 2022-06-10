using System.Collections.Generic;
using System.Globalization;
using Sdl.Community.TermExcelerator.Model;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.TermExcelerator.Services.Interfaces
{
	public interface IEntryTransformerService
    {
        IList<IEntryLanguage> CreateEntryLanguages(ExcelTerm excelTerm);
        IList<IEntryTerm> CreateEntryTerms(string term, string approved = null);
        IList<IEntryField> CreateEntryTermFields(int index, IList<string> approvals);
		ExcelEntry CreateExcelEntry(string source, string target, CultureInfo sourceLanguage, CultureInfo targetLanguage, int id = 0);
		ExcelEntry CreateExcelEntry(ExcelTerm excelTerm, int id = 0);
	}
}