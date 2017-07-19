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
    }
}
