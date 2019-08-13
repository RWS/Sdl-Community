using System.Collections.Generic;
using ExcelTerminology.Model;
using Sdl.Terminology.TerminologyProvider.Core;

namespace ExcelTerminology.Services.Interfaces
{
	public interface IEntryTransformerService
    {
        IList<IEntryLanguage> CreateEntryLanguages(ExcelTerm excelTerm);
        IList<IEntryTerm> CreateEntryTerms(string term, string approved = null);
        IList<IEntryField> CreateEntryTermFields(int index, IList<string> approvals);
    }
}