using System.Collections.Generic;
using Sdl.Community.TermExcelerator.Model;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.TermExcelerator.Services.Interfaces
{
	public interface IEntryTransformerService
    {
        IList<IEntryLanguage> CreateEntryLanguages(ExcelTerm excelTerm);
        IList<IEntryTerm> CreateEntryTerms(string term, string approved = null);
        IList<IEntryField> CreateEntryTermFields(int index, IList<string> approvals);
    }
}