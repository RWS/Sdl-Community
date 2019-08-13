using System.Collections.Generic;
using Sdl.Community.ExcelTerminology.Model;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology.Services.Interfaces
{
	public interface IEntryTransformerService
    {
        IList<IEntryLanguage> CreateEntryLanguages(ExcelTerm excelTerm);
        IList<IEntryTerm> CreateEntryTerms(string term, string approved = null);
        IList<IEntryField> CreateEntryTermFields(int index, IList<string> approvals);
    }
}