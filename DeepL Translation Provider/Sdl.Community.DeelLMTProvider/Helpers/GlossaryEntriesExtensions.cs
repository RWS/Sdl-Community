using Sdl.Community.DeepLMTProvider.Model;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.DeepLMTProvider.Helpers
{
    public static class GlossaryEntriesExtensions
    {
        public static IEnumerable<GlossaryEntry> GetDuplicates(this ICollection<GlossaryEntry> glossaryEntries) =>
            glossaryEntries
                .GroupBy(e => new { e.SourceTerm })
                .Where(g => g.Count() > 1)
                .SelectMany(g => g);
    }
}