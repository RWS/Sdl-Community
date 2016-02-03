using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.ExcelTerminology.Model;
using Sdl.Community.ExcelTerminology.Services.Interfaces;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology.Services
{
    public class NormalTermSeachService : ITermSearchService
    {
        public IList<ISearchResult> Search(string text, List<ExcelEntry> entries, int maxResultsCount)
        {
            return (from entry in entries
                where text
                    .ToLower()
                    .Contains(entry.SearchText.ToLower())
                select new SearchResult
                {
                    Id = entry.Id,
                    Language = entry.Languages.Cast<ExcelEntryLanguage>().First(lang => lang.IsSource),
                    Score = 100,
                    Text = entry.SearchText
                })
                .Take(maxResultsCount)
                .Cast<ISearchResult>()
                .ToList();
        }
    }
}
