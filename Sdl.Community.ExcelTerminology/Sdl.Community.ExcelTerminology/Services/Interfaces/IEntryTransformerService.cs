using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.ExcelTerminology.Model;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology.Services
{
    public interface IEntryTransformerService
    {
        IList<IEntryLanguage> CreateEntryLanguages(ExcelTerm excelTerm);
        IList<IEntryTerm> CreateEntryTerms(string term, string approved = null);
        IList<IEntryField> CreateEntryTermFields(string approved);
    }
}
