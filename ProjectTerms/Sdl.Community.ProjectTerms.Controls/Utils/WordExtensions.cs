using Sdl.Community.ProjectTerms.Controls.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.ProjectTerms.Controls.Utils
{
    public static class WordExtensions
    {
        public static IEnumerable<IWord> CountOccurences(this IEnumerable<string> terms)
        {
            return
                terms.GroupBy(
                    term => term,
                    (term, equivalentTerms) => new Word(term, equivalentTerms.Count()),
                    StringComparer.InvariantCultureIgnoreCase)
                    .Cast<IWord>();
        }

        public static IEnumerable<IWord> FilterByOccurrences(this IEnumerable<IWord> terms, int occurrences)
        {
            return terms.Where(term => term.Occurrences >= occurrences);
        }

        public static IEnumerable<IWord> FilterByLength(this IEnumerable<IWord> terms, int length)
        {
            return terms.Where(term => term.Text.Length >= length);
        }

        public static IEnumerable<string> FilterByBlackList(this IEnumerable<string> terms, List<string> blackList)
        {
            return terms.Where(term => !blackList.Contains(term));
        }
    }
}
