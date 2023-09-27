using Sdl.Community.DeepLMTProvider.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sdl.Community.DeepLMTProvider.Helpers
{
    public static class GlossaryEntriesExtensions
    {
        public static List<string> GetDuplicates(this ICollection<GlossaryEntry> glossaryEntries) =>
            glossaryEntries
                .GroupBy(ge => ge.SourceTerm)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
    }
}