using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.ExcelTerminology.Model;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology.Services
{
    public interface ITermSearchService
    {
        SearchResult Search(string text, List<ExcelEntry> entries, int maxResultsCount);
    }
}
