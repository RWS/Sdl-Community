using System.Collections.Generic;
using ExcelTerminology.Model;
using Sdl.Terminology.TerminologyProvider.Core;

namespace ExcelTerminology.Services.Interfaces
{
	public interface ITermSearchService
    {
        IList<ISearchResult> Search(string text, List<ExcelEntry> entries, int maxResultsCount);
    }
}