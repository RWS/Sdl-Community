using System.Collections.Generic;
using System.Globalization;
using Sdl.Community.TermExcelerator.Model;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.TermExcelerator.Services.Interfaces
{
	public interface IEntryTransformerService
	{
		IList<EntryLanguage> CreateEntryLanguages(ExcelTerm excelTerm);

		IList<EntryField> CreateEntryTermFields(int index, IList<string> approvals);

		IList<EntryTerm> CreateEntryTerms(string term, string approved = null);

		ExcelEntry CreateExcelEntry(string source, string target, CultureInfo sourceLanguage, CultureInfo targetLanguage, int id = 0);

		ExcelEntry CreateExcelEntry(ExcelTerm excelTerm, int id = 0);
		ExcelTerm GetExcelTerm(string source, string target, CultureInfo sourceLanguage, CultureInfo targetLanguage);
	}
}