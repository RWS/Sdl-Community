using System.Collections.Generic;
using Sdl.Community.TermExcelerator.Model;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.TermExcelerator.Services.Interfaces
{
	public interface ITermSearchService
    {
        IList<ISearchResult> Search(string text, List<ExcelEntry> entries, int maxResultsCount);
    }
}