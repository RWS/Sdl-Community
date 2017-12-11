using Sdl.Community.ProjectTerms.Controls.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.ProjectTerms.Controls.Utils
{
    public static class TermExtensions
    {
        public static IEnumerable<ITerm> CountOccurences(this IEnumerable<string> terms)
        {
            return
                terms.GroupBy(term => term, (term, equivalentTerms) => new Term(term, equivalentTerms.Count()), StringComparer.InvariantCulture)
                     .Cast<ITerm>();
        }

        public static IEnumerable<ITerm> FilterByOccurrences(this IEnumerable<ITerm> terms, int occurrences)
        {
            return terms.Where(term => term.Occurrences >= occurrences);
        }

        public static IEnumerable<ITerm> FilterByLength(this IEnumerable<ITerm> terms, int length)
        {
            return terms.Where(term => term.Text.Length >= length);
        }

        public static IEnumerable<string> FilterByBlackList(this IEnumerable<string> terms, List<string> blackList)
        {
            return terms.Where(term => !blackList.Contains(term));
        }

        public static IEnumerable<ITerm> FilterByBlackList(this IEnumerable<ITerm> terms, List<string> blackList)
        {
            return terms.Where(term => !blackList.Contains(term.Text));
        }

        public static IOrderedEnumerable<T> SortByOccurences<T>(this IEnumerable<T> words) where T : ITerm
        {
            return words.OrderByDescending(word => word.Occurrences);
        }
    }
}
