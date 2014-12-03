using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.WordCloud.Controls.TextAnalyses.Blacklist;
using Sdl.Community.WordCloud.Controls.TextAnalyses.Stemmers;

namespace Sdl.Community.WordCloud.Controls.TextAnalyses.Processing
{
    public static class WordExtensions
    {
        public static IEnumerable<WordGroup> GroupByStem(this IEnumerable<IWord> words, IWordStemmer stemmer)
        {
            return
                words.GroupBy(
                    word => stemmer.GetStem(word.Text), 
                    (stam, sameStamWords) => new WordGroup(stam, sameStamWords));
            
        }

        public static IOrderedEnumerable<T> SortByOccurences<T>(this IEnumerable<T> words) where T : IWord
        {
            return 
                words.OrderByDescending(
                    word => word.Occurrences);
        }

        public static IEnumerable<IWord> CountOccurences(this IEnumerable<string> terms)
        {
            return 
                terms.GroupBy(
                    term => term,
                    (term, equivalentTerms) => new Word(term, equivalentTerms.Count()), 
                    StringComparer.InvariantCultureIgnoreCase)
                    .Cast<IWord>();
        }

        public static IEnumerable<string> Filter(this IEnumerable<string> terms, IBlacklist blacklist)
        {
            return
                terms.Where(
                    term => !blacklist.Countains(term));
        }
    }
}